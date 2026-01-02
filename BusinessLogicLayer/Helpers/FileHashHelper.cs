using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Helpers
{
    public static class FileHashHelper
    {
        public static async Task<string> ComputeSha256Async(Stream stream)
        {
            if (!stream.CanSeek)
            {
                using var ms = new MemoryStream();
                await stream.CopyToAsync(ms);
                ms.Position = 0;
                using var sha = SHA256.Create();
                var hash = await sha.ComputeHashAsync(ms);
                return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            }
            else
            {
                var originalPos = stream.Position;
                stream.Position = 0;
                using var sha = SHA256.Create();
                var hash = await sha.ComputeHashAsync(stream);
                stream.Position = originalPos;
                return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            }
        }
    }
}
