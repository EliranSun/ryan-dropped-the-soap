using System.Collections;
using Dialog.Scripts;
using UnityEngine;
using UnityEngine.Networking;

namespace Character_Creator.scripts
{
    public static class VoiceAPI
    {
        private static bool _isVoiceRequestSent;
        public static AudioClip PlayerNameAudioClip { get; private set; }

        public static IEnumerator VoiceGetRequest(string text, CharacterType gender, string playerName,
            string partnerName)
        {
            var url =
                $"https://walak.vercel.app/voice?name={playerName}&gender={gender}&text={text}&partner={partnerName}";
            Debug.Log($"@ VoiceGetRequest URL {url}");

            using var webRequest = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.MPEG);
            yield return webRequest.SendWebRequest();

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(webRequest.error);
            }
            else
            {
                // VoiceApiResponseBody body = JsonUtility.FromJson<VoiceApiResponseBody>(webRequest.downloadHandler.text);
                PlayerNameAudioClip = DownloadHandlerAudioClip.GetContent(webRequest);
                Debug.Log($"PlayerNameAudioClip populated {PlayerNameAudioClip}");
            }
        }
    }
}