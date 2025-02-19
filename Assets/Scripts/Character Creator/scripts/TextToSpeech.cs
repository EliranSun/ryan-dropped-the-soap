using System.Collections;
using Dialog.Scripts;
using UnityEngine;

namespace Character_Creator.scripts
{
    public class TextToSpeech : MonoBehaviour
    {
        public IEnumerator ConvertText(string text, CharacterType gender, string playerName, string partnerName,
            ActorName actorName)
        {
            yield return StartCoroutine(VoiceAPI.VoiceGetRequest(text, gender, playerName, partnerName, actorName));
        }
    }
}