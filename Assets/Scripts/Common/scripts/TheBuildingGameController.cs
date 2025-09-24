using UnityEngine;

namespace Common.scripts
{
    public class TheBuildingGameController : MonoBehaviour
    {
        [SerializeField] private GameObject mainTitle;
        [SerializeField] private CameraPan cameraPan;

        public void StartGame()
        {
            mainTitle.SetActive(false);
            cameraPan.GoToGroundFloor();
        }
    }
}