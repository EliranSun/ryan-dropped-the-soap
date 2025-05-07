using System;
using System.Collections.Generic;
using Dialog;
using UnityEngine;

namespace Character.Scripts
{
    [Serializable]
    internal enum DialogLineTriggerName
    {
        None,
        MuseumEntrance
    }

    [Serializable]
    internal class DialogLineTrigger
    {
        public GameObject triggerObject;
        public int duplicateCount;
        public NarrationDialogLine dialogLine;
        public DialogLineTriggerName name;
        public GameEvents eventToTrigger;
    }

    [RequireComponent(typeof(Collider2D))]
    public class PlayerWorldInteractions : ObserverSubject
    {
        [SerializeField] private List<DialogLineTrigger> dialogLineMap;
        private readonly List<DialogLineTriggerName> _triggeredEvents = new();
        private readonly HashSet<DialogLineTriggerName> _triggeredEventSet = new();
        private Camera _camera;

        private void Start()
        {
            _camera = Camera.main;
        }

        private void Update()
        {
            if (!Input.GetMouseButtonDown(0) || !_camera)
                return; // Detect left mouse button click

            var ray = _camera.ScreenPointToRay(Input.mousePosition);
            var layerMask = LayerMask.GetMask("PlayerChoiceDialogBubbles");

            if (!Physics.Raycast(ray, out var hit, Mathf.Infinity, layerMask))
                return;

            print(hit.collider.gameObject);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            var dialogLine = dialogLineMap
                .Find(trigger =>
                {
                    if (_triggeredEvents == null) return trigger.triggerObject.CompareTag(other.tag);

                    var triggeredEvents = _triggeredEvents
                        .FindAll(triggerName => triggerName == trigger.name);

                    if (triggeredEvents.Count > 0)
                        return triggeredEvents.Count == trigger.duplicateCount &&
                               trigger.triggerObject.CompareTag(other.tag);

                    return trigger.triggerObject.CompareTag(other.tag);
                });

            if (dialogLine != null && !_triggeredEventSet.Contains(dialogLine.name))
            {
                _triggeredEvents.Add(dialogLine.name);
                _triggeredEventSet.Add(dialogLine.name);
                Notify(dialogLine.eventToTrigger, dialogLine.dialogLine);
            }
            else
            {
                switch (other.tag)
                {
                    case "Inside Elevator":
                        Notify(GameEvents.PlayerInsideElevator);
                        break;
                }
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            switch (other.tag)
            {
                case "Inside Elevator":
                    var triggerBounds = GetComponent<Collider2D>().bounds;
                    Vector2 exitPoint = other.transform.position;

                    if (Mathf.Abs(exitPoint.x - triggerBounds.min.x) < Mathf.Abs(exitPoint.y - triggerBounds.min.y))
                        if (exitPoint.x > triggerBounds.center.x)
                            Notify(GameEvents.ExitElevatorToFloors);
                        else if (exitPoint.y > triggerBounds.center.y)
                            Notify(GameEvents.ExitElevatorToLobby);
                    break;
            }
        }
    }
}