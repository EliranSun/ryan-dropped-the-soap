using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Elevator.scripts
{
    public class FloorController : ObserverSubject
    {
        public int floorNumber;
        [SerializeField] private TextMeshPro floorNumberText;
        [SerializeField] private GameObject outsideView;
        [SerializeField] public BuildingController buildingController;
        [SerializeField] private ApartmentController[] apartments;

        private void Start()
        {
            outsideView.SetActive(false);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Player"))
                Notify(GameEvents.FloorChange, floorNumber);
        }

        public void SetFloorNumber(int newFloorNumber, BuildingController _buildingController)
        {
            floorNumber = newFloorNumber;
            buildingController = _buildingController;

            if (floorNumberText)
                floorNumberText.text = floorNumber.ToString();

            for (var i = 0; i <= apartments.Length - 1; i++)
                apartments[i].SetData(floorNumber, i, buildingController);
        }

        public void SetObserver(UnityEvent<GameEventData> observer)
        {
            observers = observer;
        }

        public void OnNotify(GameEventData eventData)
        {
            if (eventData.Name == GameEvents.PlayerOutsideTrigger)
            {
                outsideView.SetActive(true);
                foreach (var apartment in apartments)
                {
                    // TODO: Disable all apartments?
                    apartment.gameObject.SetActive(false);
                }
            }
        }
        private void PopulateApartments()
        {
            // for (var i = 0; i < apartments.Length - 1; i++)
            //     apartments[i].SetData(floorNumber, i);

            // (var apartment in apartments)
            //     // var floor = _floors.Find(f =>
            //     //     f.GetComponent<FloorController>().floorNumber == significantApartment.floorNumber);
            //     // if (floor == null) continue;
            //     // var apartment = floor.GetComponent<FloorController>().apartments
            //     //     .First(apartment => apartment.apartmentNumber == significantApartment.apartmentNumber);
            //     if (apartment.prefab != null && !apartment.isPopulated)
            //     {
            //         // Instantiate(significantApartment.prefab, apartment.prefab.transform);
            //         // apartments should already be in scene, for making triggers and connections easier
            //         // significantApartment.prefab.transform.position = apartment.prefab.transform.position;
            //         // significantApartment.prefab.SetActive(true);
            //         // apartment.isPopulated = true;
            //
            //     }
        }
    }
}