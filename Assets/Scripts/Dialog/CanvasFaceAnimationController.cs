using UnityEngine;
using UnityEngine.UI;

namespace Dialog
{
    public enum ActorName
    {
        None,
        Charlotte, // voiced by: Charlotte (XB0fDUnXU5powFXDhCwa)
        Morgan, // voiced by: Grace (4zDsWfgtAP9O9F9kJlUk)
        Zeke, // voiced by: Roger (CwhRBWXzGAHq8TQ4Fs17)
        Ryan, // voiced by: Thomas (GBv7mTt0atIp3Br8iCZE)
        Stacy, // voiced by: Violet (j7KV53NgP8U4LRS2k2Gs)
        Pigeon, // 
        Boss, // voiced by: Bubba Marshel (YEkUdc7PezGaXaRslSHB)
        OldMan // voiced by: Beezle Wheezleby (BBfN7Spa3cqLPH1xAS22)
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
        
        // 5, 1, 3, 4, 2

        public void OnNotify(GameEventData gameEventData)
        {
            switch (gameEventData.Name)
            {
                case GameEvents.LineNarrationStart:
                    var narrationDialogLine = gameEventData.Data as NarrationDialogLine;
                    if (narrationDialogLine == null) return;
                    if (narrationDialogLine.actorName == actorName)
                    {
                        _animator.SetBool(IsTalking, true);
                        _image.color = Color.white;
                    }

                    break;

                case GameEvents.LineNarrationEnd:
                    var actorNameProperty = gameEventData.Data.GetType().GetProperty("actorName");
                    if (actorNameProperty == null)
                        break;

                    var dataActorName = (ActorName)actorNameProperty.GetValue(gameEventData.Data);
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