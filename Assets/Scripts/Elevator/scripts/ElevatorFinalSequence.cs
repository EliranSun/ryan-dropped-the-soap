using System;
using System.Linq;
using Dialog;
using Mini_Games;
using UnityEngine;

namespace Elevator.scripts
{
    [Serializable]
    public class NpcAtFloor
    {
        public GameObject npc;
        public NarrationDialogLine line;
        public int awaitAtFloor;
        public float xPosition;
        public bool isDead;
    }

    public class ElevatorFinalSequence : ObserverSubject
    {
        [SerializeField] private bool testSequence;
        [SerializeField] private GameObject player;
        [SerializeField] private GameObject sequence;
        [SerializeField] private GameObject elevator;
        [SerializeField] private ElevatorController elevatorController;
        [SerializeField] private NarrationDialogLine allInElevatorLine;
        [SerializeField] private NarrationDialogLine sequenceStartLine;
        [SerializeField] private GameObject takeOutTheGunText;
        [SerializeField] private NpcAtFloor[] npcs;
        private int _currentFloorNumber;
        private bool _isActivated;
        private int _npcIndex;

        private void Start()
        {
            if (PlayerPrefs.GetInt("CharlotteAwaitGoingRooftop") == 1) _isActivated = true;
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
                TakeOutTheGun();

            if (eventData.Name == GameEvents.FloorChange && _isActivated)
            {
                _currentFloorNumber = (int)eventData.Data;

                if (_npcIndex >= npcs.Length)
                {
                    _isActivated = false;
                    Invoke(nameof(StartFinalSequence), 2f);
                    return;
                }

                var floorNumber = (int)eventData.Data;
                if (floorNumber == npcs[_npcIndex].awaitAtFloor)
                {
                    Notify(GameEvents.StopElevator);
                    Invoke(nameof(InstantiateNpc), 1.5f); // shaft light takes 1s
                    Invoke(nameof(ResumeElevator), 3);
                }
            }

            if (eventData.Name == GameEvents.GunIsOut)
                Invoke(nameof(StartKillSequence), 3f);

            if (eventData.Name == GameEvents.MurderedNpc)
            {
                var npcName = (string)eventData.Data;

                foreach (var npc in npcs)
                    if (npc.npc.gameObject.name == npcName)
                        npc.isDead = true;

                var alive = npcs.Where(npc => !npc.isDead);
                var aliveNpcs = alive as NpcAtFloor[] ?? alive.ToArray();
                if (aliveNpcs.Count() == 1)
                    return;

                // last npc standing
                Invoke(nameof(ExitKillSequence), 6f);

                player.GetComponent<Animator>().enabled = false;
                player.GetComponent<SpriteRenderer>().sprite =
                    aliveNpcs[0].npc.GetComponent<SpriteRenderer>().sprite;
                player.AddComponent<WobblyMovement>();

                aliveNpcs[0].npc.SetActive(false);
            }
        }

        private void InstantiateNpc()
        {
            var npc = npcs[_npcIndex];
            npc.npc.transform.position = new Vector2(npcs[_npcIndex].xPosition, -1.39f);
            if (npc.line) Notify(GameEvents.TriggerSpecificDialogLine, npc.line);

            _npcIndex++;
        }

        private void ResumeElevator()
        {
            Notify(GameEvents.ResumeElevator);
            ChangeFloorNumber();
        }

        private void StartFinalSequence()
        {
            Notify(GameEvents.TriggerSpecificDialogLine, allInElevatorLine);
        }

        private void TakeOutTheGun()
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