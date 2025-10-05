using System;
using UnityEngine;

namespace Dialog
{
    [Serializable]
    internal class InGameActors
    {
        public ActorName actorName;
        public GameObject actor;
    }

    public class ZoomActorByLine : MonoBehaviour
    {
        [SerializeField] private InGameActors[] actors;
    }
}