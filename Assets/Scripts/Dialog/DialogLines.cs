using System;
using UnityEngine;

namespace Dialog
{
    [Serializable]
    public class Options
    {
        public string text;
        public DialogLine response;
    }

    [Serializable]
    public class DialogLine
    {
        public string text;
        public AudioClip audioClip;

        public ActorName actor;

        // public DialogLine nextLine;
        public Options playerOptions;
    }

    public class DialogLines : MonoBehaviour
    {
        [SerializeField] private DialogLine[] lines;
    }
}