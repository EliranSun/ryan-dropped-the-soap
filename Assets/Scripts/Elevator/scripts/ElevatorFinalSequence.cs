using UnityEngine;

namespace Elevator.scripts
{
    public class ElevatorFinalSequence : ObserverSubject
    {
        [SerializeField] private bool testSequence;
        [SerializeField] private GameObject sequence;
        [SerializeField] private GameObject elevator;
        [SerializeField] private ElevatorController elevatorController;
        [SerializeField] private GameObject takeOutTheGunText;
        [SerializeField] private int[] stopAtFloors;
        [SerializeField] private GameObject[] npcs;
        [SerializeField] private float[] xPositions = { -1, -2, 3.5f };
        private int _currentFloorNumber;
        private bool _isActivated;
        private int _npcIndex;
        private int _npcKillCount;

        private void Start()
        {
            if (testSequence) StartKillSequence();
            else ExitKillSequence();
        }

        private void ExitKillSequence()
        {
            takeOutTheGunText.SetActive(false);
            sequence.SetActive(false);
            elevator.SetActive(true);
        }

        public void OnNotify(GameEventData eventData)
        {
            if (eventData.Name == GameEvents.StartElevatorFinalSequence)
                _isActivated = true;

            if (eventData.Name == GameEvents.FloorChange && _isActivated)
            {
                _currentFloorNumber = (int)eventData.Data;

                if (_npcIndex >= stopAtFloors.Length)
                {
                    _isActivated = false;
                    Invoke(nameof(StartFinalSequence), 2f);
                    return;
                }

                var floorNumber = (int)eventData.Data;
                if (floorNumber != stopAtFloors[_npcIndex]) return;

                Notify(GameEvents.StopElevator);
                Invoke(nameof(InstantiateNpc), 1.5f); // shaft light takes 1s
                Invoke(nameof(ResumeElevator), 3);
            }

            if (eventData.Name == GameEvents.GunIsOut)
                Invoke(nameof(StartKillSequence), 3f);

            if (eventData.Name == GameEvents.MurderedNpc)
            {
                _npcKillCount++;
                if (_npcKillCount == 3) Invoke(nameof(ExitKillSequence), 6f);
            }
        }

        private void InstantiateNpc()
        {
            npcs[_npcIndex].transform.position = new Vector2(xPositions[_npcIndex], -1.39f);
            _npcIndex++;
        }

        private void ResumeElevator()
        {
            Notify(GameEvents.ResumeElevator);
            ChangeFloorNumber();
        }

        private void StartFinalSequence()
        {
            takeOutTheGunText.SetActive(true);
            Notify(GameEvents.AllowGun);
        }

        private void StartKillSequence()
        {
            sequence.SetActive(true);
            elevator.SetActive(false);
        }

        private void ChangeFloorNumber()
        {
            elevatorController.GoToFloor(_currentFloorNumber + 60 * 60); // an hour
        }
    }
}