using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace com.akihiro.coefont_dotnet
{
    public class CoeFontAPI
    {
        private readonly string accessKey;
        private readonly string clientSecret;
        private HttpClient client = null;

        public CoeFontAPI(string accessKey, string clientSecret)
        {
            this.accessKey = accessKey;
            this.clientSecret = clientSecret;
        }

        public async Task<byte[]> SynthToWAV(string synthText, string coefontID, float speed = 1.0f, float pitch = 0.0f, float kuten = 0.7f, float toten = 0.4f, float volume = 1.0f, float intonation = 1.0f)
        {
            var uri = await getSynthUri(coefontID, synthText, speed, pitch, kuten, toten, volume, intonation);
            var response = await client.GetAsync(uri);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsByteArrayAsync();
            }
            Debug.LogError(response.StatusCode + " " + response.ReasonPhrase);
            return null;
        }

        public async Task<AudioClip> SynthToAudioClip(MonoBehaviour monoBehaviour, string synthText, string coefontID, float speed = 1.0f, float pitch = 0.0f, float kuten = 0.7f, float toten = 0.4f, float volume = 1.0f, float intonation = 1.0f)
        {
            var uri = await getSynthUri(coefontID, synthText, speed, pitch, kuten, toten, volume, intonation);
            var taskCompletionSource = new TaskCompletionSource<AudioClip>();
            monoBehaviour.StartCoroutine(getAudioClip(uri, clip => taskCompletionSource?.TrySetResult(clip)));
            return await taskCompletionSource.Task;
        }

        private async Task<Uri> getSynthUri(string coefontID, string synthText, float speed, float pitch, float kuten, float toten, float volume, float intonation)
        {
            var body = new Dictionary<string, object>()
        {
            { "coefont", coefontID },
            { "text"   , synthText },
            { "speed"   , speed },
            { "pitch"   , pitch },
            { "kuten"   , kuten },
            { "toten"   , toten },
            { "volume"   , volume },
            { "intonation"   , intonation },
            { "format" , "wav"}
        };
            var body_json = JsonConvert.SerializeObject(body);
            var request = createRequest(body_json);
            return await postRequest(request);
        }

        private HttpRequestMessage createRequest(string body_json)
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

        private async Task<Uri> postRequest(HttpRequestMessage request)
        {
            var handler = new HttpClientHandler() { AllowAutoRedirect = false };
            if (client == null) client = new HttpClient(handler);
            var response = await client.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == System.Net.HttpStatusCode.Found)
                {
                    return response.Headers.Location;
                }
            }
            Debug.LogError(response.StatusCode + " " + response.ReasonPhrase);
            return null;
        }

        private IEnumerator getAudioClip(Uri uri, Action<AudioClip> action)
        {
            using (var request = UnityWebRequestMultimedia.GetAudioClip(uri, AudioType.WAV))
            {
                yield return request.SendWebRequest();
                if (request.result != UnityWebRequest.Result.ConnectionError)
                {
                    action?.Invoke(DownloadHandlerAudioClip.GetContent(request));
                }
            }
            action?.Invoke(null);
        }
    }
}
