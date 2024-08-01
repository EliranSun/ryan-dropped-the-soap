using System;
using UnityEngine;

namespace Elevator.scripts
{
    public class ElevatorController : MonoBehaviour
    {
        [SerializeField] private GameObject panel;

        private void Start()
        {
            panel.SetActive(false);
        }
    }
}