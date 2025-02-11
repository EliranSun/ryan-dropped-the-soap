using UnityEngine;
using UnityEngine.UI;

namespace Dialog.Scripts
{
    public enum ActorName
    {
        None,
        Charlotte
    }


    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(Image))]
    public class CanvasFaceAnimationController : MonoBehaviour
    {
        private static readonly int IsTalking = Animator.StringToHash("IsTalking");
        [SerializeField] private ActorName actorName;
        private Animator _animator;
        private Image _image;

        private void Start()
        {
            _animator = GetComponent<Animator>();
            _image = GetComponent<Image>();

            _image.color = Color.clear;
        }

        public void OnNotify(GameEventData gameEventData)
        {
            switch (gameEventData.name)
            {
                case GameEvents.LineNarrationStart:
                    if ((ActorName)gameEventData.data == actorName)
                    {
                        _animator.SetBool(IsTalking, true);
                        _image.color = Color.white;
                    }

                    break;

                case GameEvents.LineNarrationEnd:
                    var actorNameProperty = gameEventData.data.GetType().GetProperty("actorName");
                    if (actorNameProperty == null)
                        break;

                    var dataActorName = (ActorName)actorNameProperty.GetValue(gameEventData.data);
                    if (dataActorName == actorName)
                    {
                        _animator.SetBool(IsTalking, false);
                        Invoke(nameof(ClearImage), 2f);
                    }

                    break;
            }
        }

        private void ClearImage()
        {
            if (!_animator.GetBool(IsTalking)) _image.color = Color.clear;
        }
    }
}