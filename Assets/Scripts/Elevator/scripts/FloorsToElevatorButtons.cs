using System.Linq;
using common.scripts;
using UnityEngine;

namespace Elevator.scripts
{
    public class FloorsToElevatorButtons : ObserverSubject
    {
        [SerializeField] private Transform elevatorButtonsPanelTransform;
        [SerializeField] private GameObject floorPrefab;
        private int _currentFloorNumber;

        public void OnNotify(GameEventData eventData)
        {
            if (eventData.name == GameEvents.FloorChange)
            {
                // suppose to happen once before floors update, then for every floor change
                print("Floor Change event - Current floor number: " + _currentFloorNumber);
                _currentFloorNumber = (int)eventData.data;
                FindAndHighlightCurrentFloorButtons();
            }

            if (eventData.name == GameEvents.FloorsUpdate)
            {
                print(eventData.data);

                if (eventData.data is not FloorsData floorsData)
                {
                    print("Data is not FloorsData");
                    return;
                }

                print($"Number of floors: {floorsData.data.Length}");

                for (var i = 0; i < floorsData.data.Length; i++)
                for (var j = 0; j < floorsData.data[i].apartmentsCount; j++)
                {
                    var floorButton = Instantiate(floorPrefab, elevatorButtonsPanelTransform);
                    floorButton.gameObject.name = _currentFloorNumber == i
                        ? $"apt_active-{i + 1}:{j + 1}"
                        : $"apt_inactive-{i + 1}:{j + 1}";

                    floorButton.transform.localPosition = new Vector2(j / 10f, i / 20f);
                    floorButton.GetComponent<SpriteRenderer>().color = _currentFloorNumber == i
                        ? new Color(1, 1, 1, 1)
                        : new Color(1, 1, 1, 0.5f);
                }
            }
        }

        private void FindAndHighlightCurrentFloorButtons()
        {
            var currentHighlight =
                FindObjectsByType<GameObject>(FindObjectsSortMode.None)
                    .Where(item => item.name.StartsWith("apt_active-"))
                    .ToList();
            var nextHighlight =
                FindObjectsByType<GameObject>(FindObjectsSortMode.None)
                    .Where(item => item.name.StartsWith($"apt_inactive-{_currentFloorNumber + 1}:"))
                    .ToList();

            foreach (var item in currentHighlight)
            {
                item.name = item.name.Replace("active", "inactive");
                item.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.5f);
            }

            foreach (var item in nextHighlight)
            {
                item.name = item.name.Replace("inactive", "active");
                item.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
            }
        }
    }
}