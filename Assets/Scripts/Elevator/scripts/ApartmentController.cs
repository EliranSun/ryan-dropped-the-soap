using Dialog;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Elevator.scripts
{
    public class ApartmentController : MonoBehaviour
    {
        public int floorNumber;
        public int apartmentNumber;
        [SerializeField] private DoorController door;
        [SerializeField] public GameObject[] tenants;
        public BuildingController buildingController;
        public GameObject prefab;
        public bool isPopulated;
        public NarrationDialogLine narrationLine;

        public void SetData(int floorNum, int apartmentNum, BuildingController buildingControl)
        {
            floorNumber = floorNum;
            apartmentNumber = apartmentNum;
            buildingController = buildingControl;

            door.SetDoorNumber($"{floorNum}{apartmentNum}");

            if (buildingController.tenants is not { Length: > 0 }) return;

            foreach (var tenant in buildingController.tenants)
                if (tenant.floorNumber == floorNumber && tenant.apartmentNumber == apartmentNumber)
                    if (tenant.name == Tenant.Zeke)
                    {
                        if (tenant.tenantPrefab)
                        {
                            tenant.tenantPrefab.transform.position = transform.position;
                            tenant.tenantPrefab.SetActive(true);
                        }

                        if (tenant.apartmentContents)
                        {
                            tenant.apartmentContents.SetActive(true);
                            tenant.apartmentContents.transform.position = transform.position;
                        }

                        tenant.currentApartmentNumber = floorNumber * 10 + apartmentNumber;
                        tenant.door = door.gameObject;
                    }
        }

        public void OnNotify(GameEventData eventData)
        {
            if (eventData.Name == GameEvents.ClickOnItem)
                SceneManager.LoadScene("3a. building front scene");
        }
    }
}