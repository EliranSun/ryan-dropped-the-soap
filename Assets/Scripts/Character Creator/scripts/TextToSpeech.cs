using System.Collections;
using Dialog.Scripts;
using UnityEngine;

namespace museum_dialog.scripts
{
    public class TextToSpeech : MonoBehaviour
    {
        public IEnumerator ConvertText(string text, CharacterType gender, string playerName, string partnerName)
        {
            yield return StartCoroutine(VoiceAPI.VoiceGetRequest(text, gender, playerName, partnerName));
        }
    }
}