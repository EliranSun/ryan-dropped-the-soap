using UnityEngine;

namespace Mini_Games.Organize_Desk.scripts
{
    public class UIItemCollisionData
    {
        public GameObject itemGameObject;
        public UIItem itemName;
        public GameObject otherGameObject;

        public UIItemCollisionData(UIItem itemName, GameObject itemGameObject, GameObject otherGameObject)
        {
            this.itemName = itemName;
            this.itemGameObject = itemGameObject;
            this.otherGameObject = otherGameObject;
        }
    }

    [RequireComponent(typeof(Collider))]
    public class DetectOrganizedItemCollisionWithTag : MonoBehaviour
    {
        [SerializeField] private UIItem itemName;
        [SerializeField] private string tagName;
        private OrganizeMiniGame _organizeMiniGameManager;

        private void OnEnable()
        {
            _organizeMiniGameManager = GameObject.Find("Organize Desk").GetComponent<OrganizeMiniGame>();
        }

        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.CompareTag(tagName))
                _organizeMiniGameManager.OnNotify(new GameEventData(
                    GameEvents.CollisionDetected,
                    new UIItemCollisionData(itemName, gameObject, other.gameObject)
                ));
        }
    }
}