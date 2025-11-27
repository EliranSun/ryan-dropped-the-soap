using System;
using Dialog;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Building
{
    [Serializable]
    public class Apartment
    {
        public bool isActive;
        public Scene sceneName;
        public ActorName actorName;
        public GameObject apartment;
    }

    public class BuildingSceneSelector : MonoBehaviour
    {
        [SerializeField] private GameObject selection;
        [SerializeField] private Apartment[] apartments;
        [SerializeField] private int selectedApartment;
        [SerializeField] private float yOffset = 1;

        private void Update()
        {
            // TODO: Map to joystick and actual keyboard?
            if (Input.GetKeyDown(KeyCode.P))
            {
                selectedApartment = (selectedApartment + 1) % apartments.Length;
                PositionSelectionAtApartment();
            }

            if (Input.GetKeyDown(KeyCode.I))
            {
                selectedApartment = (selectedApartment - 1 + apartments.Length) % apartments.Length;
                PositionSelectionAtApartment();
            }
        }

        private void PositionSelectionAtApartment()
        {
            selection.transform.position =
                apartments[selectedApartment].apartment.transform.position + new Vector3(0, yOffset, 0);
        }
    }
}