using UnityEngine;
using UnityEngine.SceneManagement;

namespace Common.scripts
{
    public class PostZekeSuicideController : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer playerSpriteRenderer;
        [SerializeField] private Sprite playerSprite;

        public void OnNotify(GameEventData eventData)
        {
            switch (eventData.Name)
            {
                case GameEvents.NextScene:
                    SceneManager.LoadScene("Scenes/CORE test");
                    break;

                case GameEvents.QuitGame:
                    Application.Quit();
                    break;

                case GameEvents.SwitchToPlayerSprite:
                    playerSpriteRenderer.sprite = playerSprite;
                    playerSpriteRenderer.flipX = true;
                    break;
            }
        }
    }
}