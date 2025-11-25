using UnityEngine;

namespace Dialog
{
    public class ActorLines : ObserverSubject
    {
        [SerializeField] private bool invokeOnStart;
        [SerializeField] private NarrationDialogLine[] lines;

        private void Start()
        {
            if (invokeOnStart && lines.Length > 0)
                Invoke(nameof(InvokeLine), 2);
        }

        private void InvokeLine()
        {
            Notify(GameEvents.TriggerSpecificDialogLine, lines[0]);
        }

        public void OnNotify(GameEventData eventData)
        {
            if (eventData.Name == GameEvents.ClickOnItem)
            {
                var itemName = (string)eventData.Data;
                print($"Item name: {itemName}, game object name: {gameObject.name}");
                if (itemName == gameObject.name)
                    InvokeLine();
            }

            if (eventData.Name == GameEvents.PlayerInteraction)
            {
                // var instanceId = (Interaction)eventData.Data;
            }
        }
    }
}