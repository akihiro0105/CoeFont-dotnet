using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace dotnet_core
{
    class Program
    {
        /// <summary>
        /// CoeFont APIのアクセス情報を設定
        /// </summary>
        private static readonly string accessKey = "<Access Key>";
        private static readonly string clientSecret = "<Client Secret>";

        static void Main(string[] args)
        {
            var body = new Dictionary<string, object>()
        {
            { "coefont",  "46a81787-af54-4a91-8c5b-3b597066294e" },
            { "text"   , "あらゆる現実を全て自分の方へ捻じ曲げたのだ" },
            { "format" , "wav"}
        };
            var body_json = JsonSerializer.Serialize(body);

            var request = createRequest(body_json);
            var data = postRequest(request).GetAwaiter().GetResult();
            File.WriteAllBytes(Path.Combine(Environment.CurrentDirectory, "test.wav"), data);
        }

        private static HttpRequestMessage createRequest(string body_json)
        {
            var unixTime = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString();
            var hmac_key = Encoding.UTF8.GetBytes(clientSecret);
            var hmac_body = Encoding.UTF8.GetBytes(unixTime + body_json);
            var signature = "";
            using (HMACSHA256 hmac = new HMACSHA256(hmac_key))
            {
                var hash = hmac.ComputeHash(hmac_body, 0, hmac_body.Length);
                signature = BitConverter.ToString(hash);
            }
            signature = signature.Replace("-", "").ToLower();

            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.coefont.cloud/v1/text2speech");
            request.Headers.Add("X-Coefont-Date", unixTime);
            request.Headers.Add("X-Coefont-Content", signature);
            request.Headers.Add("Authorization", accessKey);
            request.Content = new StringContent(body_json, Encoding.UTF8, "application/json");
            return request;
        }

        private static async Task<byte[]> postRequest(HttpRequestMessage request)
        {
            var handler = new HttpClientHandler();
            var client = new HttpClient(handler);
            var response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsByteArrayAsync();
            }
            else
            {
                Console.WriteLine(response.StatusCode + " " + response.ReasonPhrase);
                return null;
            }
        }
    }
}
