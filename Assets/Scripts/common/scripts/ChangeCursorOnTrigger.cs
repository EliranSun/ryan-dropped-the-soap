using UnityEngine;

namespace common.scripts
{
    // [Serializable]
    // public enum InteractableObjectType
    // {
    //     Look,
    //     Grab,
    //     Talk
    // }

    public class ChangeCursorOnTrigger : MonoBehaviour
    {
        [SerializeField] private Texture2D texture;
        // [SerializeField] private InteractableObjectType type;

        private void OnMouseEnter()
        {
            Cursor.SetCursor(texture, Vector2.zero, CursorMode.Auto);
        }

        private void OnMouseExit()
        {
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }
    }
}