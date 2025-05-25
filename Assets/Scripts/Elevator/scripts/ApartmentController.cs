using Dialog;
using UnityEngine;

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
                        Instantiate(tenant.tenantPrefab, transform.position, Quaternion.identity);
        }
    }
}