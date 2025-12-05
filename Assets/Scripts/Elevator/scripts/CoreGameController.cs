using Dialog;
using Player;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Elevator.scripts
{
    public class CoreGameController : MonoBehaviour
    {
        private ActorName _npcControlled;

        public void OnNotify(GameEventData eventData)
        {
            if (eventData.Name == GameEvents.PlayerControlNpc)
            {
                var actorNameProperty = eventData.Data.GetType().GetProperty("actorName");
                if (actorNameProperty == null) return;
                var actorName = (ActorName)actorNameProperty.GetValue(eventData.Data);
                _npcControlled = actorName;
            }

            if (eventData.Name == GameEvents.PlayerInteraction)
            {
                var interaction = (Interaction)eventData.Data;
                if (interaction.objectName == ObjectNames.ElevatorExitDoors && _npcControlled == ActorName.Zeke)
                    SceneManager.LoadScene("0a. Zeke scene");
            }
        }
    }
}