using BusinessLogicLayer.RAG.Interfaces;
using DataAccessLayer.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BusinessLogicLayer.RAG.Services
{
    public class RagService : IRagService
    {
        private readonly HttpClient _httpClient;

        public RagService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task CallRagBackendAsync(LectureFile lectureFile, int lectureId, string courseName, string lectureName)
        {
            using var multipart = new MultipartFormDataContent();

            // 1) Load file stream
            string physicalPath = Path.Combine("wwwroot", lectureFile.FilePath.TrimStart('/'));
            using var fileStream = File.OpenRead(physicalPath);
            multipart.Add(new StreamContent(fileStream), "file", lectureFile.FileName);

            // 2) Add metadata fields
            multipart.Add(new StringContent(lectureId.ToString()), "lecture_id");
            multipart.Add(new StringContent(courseName), "course_name");
            multipart.Add(new StringContent(lectureName), "lecture_name");

            // 3) Call RAG backend
            var response = await _httpClient.PostAsync("http://127.0.0.1:8000/process-lecture/", multipart);
            response.EnsureSuccessStatusCode();

            // 4) Read response
            var json = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<dynamic>(json);

            Console.WriteLine($"RAG processed chunks count: {result.chunks_count}");
        }


        //public async Task CallRagBackendBatchAsync(LectureFile[] lectureFiles, int lectureId)
        //{
        //    foreach (var file in lectureFiles)
        //    {
        //        await CallRagBackendAsync(file, lectureId);
        //    }
        //}

        public async Task<string> GetRagProcessingStatusAsync(int lectureId)
        {
            var response = await _httpClient.GetAsync($"http://127.0.0.1:8000/rag-status/{lectureId}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> GenerateQuestionsAsync(
     List<int> lectureIds,
     Dictionary<string, int> distribution,
     string semanticQuery
 )
        {
            var payload = new
            {
                LecturesToProcess = lectureIds,
                Distribution = distribution,
                SemanticQuery = semanticQuery
            };

            // Send request
            var response = await _httpClient.PostAsJsonAsync(
                "http://127.0.0.1:8000/generate-questions/",
                payload
            );

            var content = await response.Content.ReadAsStringAsync();
            var contentType = response.Content.Headers.ContentType?.MediaType;

            // ==============================
            // ✅ IMPROVED ERROR HANDLING
            // ==============================
            if (!response.IsSuccessStatusCode)
            {
                string errorMessage = content;

                try
                {
                    using var errorJson = JsonDocument.Parse(content);
                    if (errorJson.RootElement.TryGetProperty("detail", out var detail))
                    {
                        errorMessage = detail.GetString() ?? content;
                    }
                }
                catch
                {
                    // ignore JSON parse errors, fallback to raw content
                }

                throw new Exception(
                    $"RAG error ({(int)response.StatusCode}): {errorMessage}"
                );
            }

            if (contentType != null && contentType.Contains("html"))
            {
                throw new Exception(
                    $"RAG service returned HTML instead of expected JSON:\n{content}"
                );
            }

            // -----------------------------
            // Extract the "questions" field
            // -----------------------------
            try
            {
                using var jsonDoc = JsonDocument.Parse(content);
                var root = jsonDoc.RootElement;

                if (root.TryGetProperty("questions", out var questionsElement))
                {
                    return questionsElement.GetString() ?? "";
                }
                else
                {
                    throw new Exception(
                        $"Response JSON did not contain 'questions' field:\n{content}"
                    );
                }
            }
            catch
            {
                throw new Exception(
                    $"Invalid JSON returned from service:\n{content}"
                );
            }
        }

    }
}
//questions_raw