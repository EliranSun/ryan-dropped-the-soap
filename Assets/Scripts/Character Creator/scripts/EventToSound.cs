using System;
using UnityEngine;

namespace Character_Creator.scripts
{
    [Serializable]
    public class EventSound
    {
        public AudioClip sound;
        public GameEvents eventName;
    }

    public class EventToSound : MonoBehaviour
    {
        [SerializeField] public EventSound[] eventSounds;
    }
}