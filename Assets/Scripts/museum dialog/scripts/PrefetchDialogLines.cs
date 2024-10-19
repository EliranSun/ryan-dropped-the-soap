using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Dialog.Scripts;
using UnityEngine;

namespace museum_dialog.scripts
{
    [RequireComponent(typeof(TextToSpeech))]
    public class PrefetchDialogLines : MonoBehaviour
    {
        public bool isFetched;
        [SerializeField] private readonly List<NarrationDialogLine> linesWithPlayerName = new();
        private TextToSpeech _textToSpeechUtil;
        public static PrefetchDialogLines Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
                Destroy(this);
            else
                Instance = this;
        }

        private void Start()
        {
            _textToSpeechUtil = GetComponent<TextToSpeech>();
        }

        public IEnumerator FetchAndPopulatePlayerLines()
        {
            print("FetchAndPopulatePlayerLines PENDING");

            if (linesWithPlayerName.Count == 0)
            {
                print("FetchAndPopulatePlayerLines SUCCESS - no lines with player name");
                yield break;
            }

            var coroutines = new IEnumerator[linesWithPlayerName.Count];
            var playerName = PlayerData.GetPlayerName();
            var partnerName = PlayerData.GetPartnerName();
            var playerGender = PlayerData.GetPlayerGender();

            for (var i = 0; i < linesWithPlayerName.Count; i++)
            {
                var dialogLine = linesWithPlayerName[i];
                coroutines[i] = ConvertAndPopulateLine(dialogLine, playerGender, playerName, partnerName);
            }

            yield return WaitForAll(coroutines);
            isFetched = true;

            print("FetchAndPopulatePlayerLines SUCCESS - API ended");
        }

        private IEnumerator ConvertAndPopulateLine(NarrationDialogLine dialogLine, CharacterType characterType,
            string playerName, string partnerName)
        {
            var line = GetLineByGender(dialogLine, characterType);
            var textWithNames = line.text
                .Replace("{playerName}", playerName)
                .Replace("{partnerName}", partnerName);

            yield return _textToSpeechUtil.ConvertText(textWithNames, characterType, playerName, partnerName);

            dialogLine.playerName = playerName;
            line.clip = VoiceAPI.PlayerNameAudioClip;

            print($"line.clip populated {line.clip} {playerName} {characterType}");
        }

        private IEnumerator WaitForAll(params IEnumerator[] coroutines)
        {
            foreach (var coroutine in coroutines)
                while (coroutine.MoveNext())
                    yield return coroutine.Current;
        }

        private static VoicedLine GetLineByGender(NarrationDialogLine dialogueLineObject,
            CharacterType characterType)
        {
            return dialogueLineObject.voicedLines.First(voicedLine =>
            {
                if (voicedLine.gender == CharacterType.NonBinary || voicedLine.gender == CharacterType.None)
                    return true;

                return voicedLine.gender == characterType;
            });
        }
    }
}