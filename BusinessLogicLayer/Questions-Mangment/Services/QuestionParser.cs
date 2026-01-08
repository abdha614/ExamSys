using BusinessLogicLayer.Dtos;
using BusinessLogicLayer.Questions_Mangment.Interfaces;
using DataAccessLayer.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace BusinessLogicLayer.Questions_Mangment.Services
{
    public class QuestionParser : IQuestionParser
    {
        public ParsedQuestionsDto Parse(string aiResponse)
        {
            var result = new ParsedQuestionsDto();

            // Normalize line endings
            aiResponse = aiResponse.Replace("\r\n", "\n").Replace("\r", "\n");

            // Match each question group section and preserve its type and content
            var matches = Regex.Matches(
                aiResponse,
                @"(?<type>mcqEasy|mcqMedium|mcqHard|tfEasy|tfMedium|tfHard):\s*(?<content>(.*?))(?=(mcqEasy|mcqMedium|mcqHard|tfEasy|tfMedium|tfHard):|\z)",
                RegexOptions.IgnoreCase | RegexOptions.Singleline);

            foreach (Match match in matches)
            {
                var type = match.Groups["type"].Value.Trim().ToLower();
                var content = match.Groups["content"].Value;

                // Split lines but preserve formatting
                var lines = content.Split('\n')
                                   .Where(l => !string.IsNullOrWhiteSpace(l))
                                   .ToList();

                var questions = new List<QuestionAIDto>();

                for (int i = 0; i < lines.Count;)
                {
                    var line = lines[i];

                    // -------------------------
                    // MCQ Parsing
                    // -------------------------
                    if (type.StartsWith("mcq") && Regex.IsMatch(line, @"^Q\d+\)", RegexOptions.IgnoreCase))
                    {
                        var q = new QuestionAIDto
                        {
                            QuestionText = line,
                            Choices = new List<string>(),
                            LectureId = 0,
                            LectureName = string.Empty
                        };
                        i++;

                        // Collect question text lines until first choice/correct answer/lecture info
                        var questionTextLines = new List<string> { q.QuestionText };
                        while (i < lines.Count &&
                               !Regex.IsMatch(lines[i], @"^[A-Da-d]\)", RegexOptions.IgnoreCase) &&
                               !lines[i].ToLower().StartsWith("correct answer") &&
                               !Regex.IsMatch(lines[i], @"^\(lectureId\s*:\s*\d+\)", RegexOptions.IgnoreCase) &&
                               !Regex.IsMatch(lines[i], @"^\(lectureName\s*:\s*.*\)", RegexOptions.IgnoreCase))
                        {
                            questionTextLines.Add(lines[i]);
                            i++;
                        }
                        q.QuestionText = string.Join("\n", questionTextLines);

                        // Collect choices (A-D)
                        while (i < lines.Count && Regex.IsMatch(lines[i], @"^[A-Da-d]\)", RegexOptions.IgnoreCase))
                        {
                            q.Choices.Add(lines[i]);
                            i++;
                        }

                        // Get correct answer (A-D)
                        if (i < lines.Count && lines[i].ToLower().StartsWith("correct answer"))
                        {
                            var matchAns = Regex.Match(lines[i], @"correct answer\s*:\s*([A-Da-d])", RegexOptions.IgnoreCase);
                            if (matchAns.Success)
                                q.Answer = matchAns.Groups[1].Value.ToUpper();

                            i++;
                        }

                        // Extract optional lectureId / lectureName lines
                        while (i < lines.Count &&
                               (Regex.IsMatch(lines[i], @"^\(lectureId\s*:\s*\d+\)", RegexOptions.IgnoreCase) ||
                                Regex.IsMatch(lines[i], @"^\(lectureName\s*:\s*.*\)", RegexOptions.IgnoreCase)))
                        {
                            var lectureIdMatch = Regex.Match(lines[i], @"^\(lectureId\s*:\s*(\d+)\)", RegexOptions.IgnoreCase);
                            if (lectureIdMatch.Success)
                            {
                                if (int.TryParse(lectureIdMatch.Groups[1].Value, out int parsedId))
                                    q.LectureId = parsedId;
                                i++;
                                continue;
                            }

                            var lectureNameMatch = Regex.Match(lines[i], @"^\(lectureName\s*:\s*(.+?)\)", RegexOptions.IgnoreCase);
                            if (lectureNameMatch.Success)
                            {
                                q.LectureName = lectureNameMatch.Groups[1].Value.Trim();
                                i++;
                                continue;
                            }

                            break;
                        }

                        // Shuffle MCQ choices to randomize correct answer position
                        ShuffleMcqChoices(q);

                        questions.Add(q);
                        continue;
                    }

                    // -------------------------
                    // True/False Parsing
                    // -------------------------
                    if (type.StartsWith("tf") && Regex.IsMatch(line, @"^Q\d+\)", RegexOptions.IgnoreCase))
                    {
                        var q = new QuestionAIDto
                        {
                            QuestionText = line,
                            LectureId = 0,
                            LectureName = string.Empty
                        };
                        i++;

                        // Skip blank lines
                        while (i < lines.Count && string.IsNullOrWhiteSpace(lines[i]))
                            i++;

                        // Match "Answer: True/False"
                        if (i < lines.Count)
                        {
                            var answerLine = lines[i];
                            var tfMatch = Regex.Match(answerLine, @"(?i)^answer\s*:\s*(true|false)\s*$");

                            if (tfMatch.Success)
                            {
                                var normalized = tfMatch.Groups[1].Value;
                                q.Answer = normalized.Substring(0, 1).ToUpper() + normalized.Substring(1).ToLower();
                            }

                            i++;
                        }

                        // Extract optional lectureId / lectureName lines
                        while (i < lines.Count &&
                               (Regex.IsMatch(lines[i], @"^\(lectureId\s*:\s*\d+\)", RegexOptions.IgnoreCase) ||
                                Regex.IsMatch(lines[i], @"^\(lectureName\s*:\s*.*\)", RegexOptions.IgnoreCase)))
                        {
                            var lectureIdMatch = Regex.Match(lines[i], @"^\(lectureId\s*:\s*(\d+)\)", RegexOptions.IgnoreCase);
                            if (lectureIdMatch.Success)
                            {
                                if (int.TryParse(lectureIdMatch.Groups[1].Value, out int parsedId))
                                    q.LectureId = parsedId;
                                i++;
                                continue;
                            }

                            var lectureNameMatch = Regex.Match(lines[i], @"^\(lectureName\s*:\s*(.+?)\)", RegexOptions.IgnoreCase);
                            if (lectureNameMatch.Success)
                            {
                                q.LectureName = lectureNameMatch.Groups[1].Value.Trim();
                                i++;
                                continue;
                            }

                            break;
                        }

                        questions.Add(q);
                        continue;
                    }

                    i++;
                }

                result.QuestionGroups.Add(new QuestionGroupDto
                {
                    Type = type,
                    Questions = questions
                });
            }

            return result;
        }

        // -------------------------
        // Helper: Shuffle MCQ choices
        // -------------------------
        private void ShuffleMcqChoices(QuestionAIDto question)
        {
            if (question.Choices == null || question.Choices.Count != 4 || string.IsNullOrEmpty(question.Answer))
                return;

            // Map current answer to index
            int correctIndex = question.Answer switch
            {
                "A" => 0,
                "B" => 1,
                "C" => 2,
                "D" => 3,
                _ => -1
            };

            if (correctIndex == -1) return;

            var correctChoice = question.Choices[correctIndex];

            // Remove correct choice temporarily
            var otherChoices = question.Choices.Where((c, idx) => idx != correctIndex).ToList();

            // Shuffle other choices
            var rng = new Random();
            otherChoices = otherChoices.OrderBy(x => rng.Next()).ToList();

            // Pick a new random index for the correct answer
            int newIndex = rng.Next(0, 4);

            // Insert correct choice at new index
            otherChoices.Insert(newIndex, correctChoice);

            // Update question choices
            question.Choices = otherChoices;

            // Update Answer property (A, B, C, D)
            question.Answer = newIndex switch
            {
                0 => "A",
                1 => "B",
                2 => "C",
                3 => "D",
                _ => question.Answer
            };
        }
    }
}
