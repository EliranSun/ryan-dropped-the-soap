using UnityEngine;

namespace common.scripts
{
    public class ChangeCursorOnTrigger : MonoBehaviour
    {
        [SerializeField] private Texture2D texture;

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