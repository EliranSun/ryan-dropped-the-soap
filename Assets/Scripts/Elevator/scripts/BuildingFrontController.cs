using UnityEngine;
using UnityEngine.SceneManagement;

namespace Elevator.scripts
{
    public class BuildingFrontController : MonoBehaviour
    {
        public void OnNotify(GameEventData eventData)
        {
            if (eventData.Name == GameEvents.ClickOnItem)
                SceneManager.LoadScene("3b. inside apartment");
        }
    }
}