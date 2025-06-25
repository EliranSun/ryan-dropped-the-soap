using UnityEngine;

namespace Elevator.scripts
{
    public class ElevatorFinalSequence : ObserverSubject
    {
        [SerializeField] private int[] stopAtFloors;
        [SerializeField] private GameObject[] npcs;
        [SerializeField] private float[] xPositions = { -1, -2, 3.5f };
        private bool _isActivated;
        private int _npcIndex;

        public void OnNotify(GameEventData eventData)
        {
            if (eventData.Name == GameEvents.StartElevatorFinalSequence)
                _isActivated = true;

            if (eventData.Name == GameEvents.FloorChange && _isActivated && _npcIndex < stopAtFloors.Length)
            {
                var floorNumber = (int)eventData.Data;
                if (floorNumber != stopAtFloors[_npcIndex]) return;

                Notify(GameEvents.StopElevator);
                Invoke(nameof(InstantiateNpc), 1.5f); // shaft light takes 1s
                Invoke(nameof(ResumeElevator), 3);
            }
        }

        private void InstantiateNpc()
        {
            Instantiate(npcs[_npcIndex], new Vector2(xPositions[_npcIndex], -1.39f), Quaternion.identity);
            _npcIndex++;
        }

        private void ResumeElevator()
        {
            Notify(GameEvents.ResumeElevator);
        }
    }
}