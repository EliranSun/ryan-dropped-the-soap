using System;
using UnityEngine;

namespace Dialog
{
    [Serializable]
    public class Option
    {
        public string text;
        [SerializeField] public DialogLine response;
    }

    [Serializable]
    public class DialogLine
    {
        public string text;
        public AudioClip audioClip;
        public ActorName actor;
        public Option[] playerOptions;

        [SerializeField] public DialogLine nextLine;
    }

    public class DialogLines : MonoBehaviour
    {
        [SerializeField] private DialogLine[] lines;

        // FIXME: Maximum depth exceeded
        public DialogLine[] GetLines()
        {
            return lines;
        }
    }
}