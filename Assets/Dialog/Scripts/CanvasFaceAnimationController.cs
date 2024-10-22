using UnityEngine;

namespace Dialog.Scripts
{
    public enum ActorName
    {
        None,
        Charlotte
    }


    [RequireComponent(typeof(Animator))]
    public class CanvasFaceAnimationController : MonoBehaviour
    {
        private static readonly int IsTalking = Animator.StringToHash("IsTalking");
        [SerializeField] private ActorName actorName;
        private Animator _animator;

        private void Start()
        {
            _animator = GetComponent<Animator>();
        }

        public void OnNotify(GameEventData gameEventData)
        {
            switch (gameEventData.name)
            {
                case GameEvents.LineNarrationStart:
                    if ((ActorName)gameEventData.data == actorName) _animator.SetBool(IsTalking, true);
                    break;

                case GameEvents.LineNarrationEnd:
                    if ((ActorName)gameEventData.data == actorName) _animator.SetBool(IsTalking, false);
                    break;
            }
        }
    }
}