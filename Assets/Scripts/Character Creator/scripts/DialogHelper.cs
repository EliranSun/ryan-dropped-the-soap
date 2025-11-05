using Dialog;
using UnityEngine;

namespace Character_Creator.scripts
{
    public class NarrationDialogProperties
    {
        public int ActionNumber;
        public ActorName ActorName;
    }

    public class DialogHelper : MonoBehaviour
    {
        public static NarrationDialogProperties GetDialogNotificationProperties(GameEventData eventData)
        {
            var actionNumberDataProperty = eventData.Data.GetType().GetProperty("actionNumberData");
            var actorNameProperty = eventData.Data.GetType().GetProperty("actorName");

            var apartmentNumber = actionNumberDataProperty != null
                ? (int)actionNumberDataProperty.GetValue(eventData.Data)
                : -1;
            var npcName = actorNameProperty != null
                ? (ActorName)actorNameProperty.GetValue(eventData.Data)
                : ActorName.None;

            return new NarrationDialogProperties
            {
                ActorName = npcName,
                ActionNumber = apartmentNumber
            };
        }
    }
}