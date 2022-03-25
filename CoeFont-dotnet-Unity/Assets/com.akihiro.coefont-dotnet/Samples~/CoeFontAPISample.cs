using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace com.akihiro.coefont_dotnet
{
    [RequireComponent(typeof(AudioSource))]
    public class CoeFontAPISample : MonoBehaviour
    {
        /// <summary>
        /// CoeFontのAPI情報から取得した値を設定
        /// https://coefont.cloud/account/api
        /// </summary>
        private const string accessKey = "<Access Key>";
        private const string clientSecret = "<Client Secret>";

        /// <summary>
        /// CoeFont設定パラメータ
        /// https://docs.coefont.cloud/#tag/Text2speech
        /// </summary>
        public string coefontID = "46a81787-af54-4a91-8c5b-3b597066294e";
        public string synthText = "あらゆる現実を全て自分の方へ捻じ曲げたのだ";
        [Range(0.1f, 10.0f)] public float speed = 1.0f;
        [Range(-3000.0f, 3000.0f)] public float pitch = 0.0f;
        [Range(0.0f, 5.0f)] public float kuten = 0.7f;
        [Range(0.2f, 2.0f)] public float toten = 0.4f;
        [Range(0.2f, 2.0f)] public float volume = 1.0f;
        [Range(0.0f, 2.0f)] public float intonation = 1.0f;

        private CoeFontAPI coeFont = new CoeFontAPI(accessKey, clientSecret);

        public async void SynthAudioClip()
        {
            var clip = await coeFont.SynthToAudioClip(this, synthText, coefontID, speed, pitch, kuten, toten, volume, intonation);
            GetComponent<AudioSource>().PlayOneShot(clip);
        }

        public async void SynthWAV()
        {
            var data = await coeFont.SynthToWAV(synthText, coefontID, speed, pitch, kuten, toten, volume, intonation);
            var path = Path.Combine(Application.dataPath, "test.wav");
            File.WriteAllBytes(path, data);
            Debug.Log($"Save {path}");
        }
    }
}
