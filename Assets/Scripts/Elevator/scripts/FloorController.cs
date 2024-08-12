using System.Collections;
using UnityEngine;

namespace Elevator.scripts
{
    public class FloorController : MonoBehaviour
    {
        [SerializeField] private BoxCollider2D doorTrigger;
        [SerializeField] private SpriteRenderer floorSprite;
        [SerializeField] private SpriteRenderer elevatorSprite;
        [SerializeField] private int floorNumber;

        public void OnNotify(GameEventData eventData)
        {
            if (eventData.name == GameEvents.PlayerExitElevator)
                StartCoroutine(FadeOutElevator());
        }

        private IEnumerator FadeOutElevator()
        {
            var elevatorColor = elevatorSprite.color;
            var floorColor = floorSprite.color;
            var elevatorAlpha = elevatorColor.a;
            var floorAlpha = floorColor.a;
            var elevatorAlphaStep = elevatorAlpha / 100;
            var floorAlphaStep = floorAlpha / 100;
            for (var i = 0; i < 100; i++)
            {
                elevatorColor.a -= elevatorAlphaStep;
                floorColor.a += floorAlphaStep;
                elevatorSprite.color = elevatorColor;
                floorSprite.color = floorColor;
                yield return new WaitForSeconds(0.01f);
            }
        }
    }
}