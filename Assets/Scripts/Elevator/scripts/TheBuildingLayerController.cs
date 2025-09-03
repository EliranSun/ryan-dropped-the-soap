using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Elevator.scripts
{
    public class TheBuildingLayerController : MonoBehaviour
    {
        [SerializeField] private GameObject[] layers;

        public void OnNotify(GameEventData eventData)
        {
            if (eventData.Name == GameEvents.EnterTheBuilding)
            {
                StartCoroutine(FadeOutLayer(0));
                StartCoroutine(FadeInLayer(1));
            }

            if (eventData.Name == GameEvents.ExitTheBuilding)
            {
                StartCoroutine(FadeOutLayer(1));
                StartCoroutine(FadeInLayer(0));
            }
        }

        private IEnumerator FadeOutLayer(int layerIndex)
        {
            var layer = layers[layerIndex];
            var spriteRenderers = GetAllSpriteRenderers(layer);

            if (spriteRenderers.Count == 0)
            {
                layer.SetActive(false);
                yield break;
            }

            float currentAlpha = spriteRenderers[0].color.a;

            while (currentAlpha > 0)
            {
                currentAlpha -= 0.1f;
                currentAlpha = Mathf.Max(0, currentAlpha);

                SetAlphaForAllSprites(spriteRenderers, currentAlpha);
                yield return new WaitForSeconds(0.1f);
            }

            layer.SetActive(false);
        }

        private IEnumerator FadeInLayer(int layerIndex)
        {
            var layer = layers[layerIndex];
            layer.SetActive(true);

            var spriteRenderers = GetAllSpriteRenderers(layer);

            if (spriteRenderers.Count == 0)
                yield break;

            float currentAlpha = spriteRenderers[0].color.a;

            while (currentAlpha < 1)
            {
                currentAlpha += 0.1f;
                currentAlpha = Mathf.Min(1, currentAlpha);

                SetAlphaForAllSprites(spriteRenderers, currentAlpha);
                yield return new WaitForSeconds(0.1f);
            }
        }

        private List<SpriteRenderer> GetAllSpriteRenderers(GameObject parent)
        {
            var spriteRenderers = new List<SpriteRenderer>();
            GetSpriteRenderersRecursive(parent.transform, spriteRenderers);
            return spriteRenderers;
        }

        private void GetSpriteRenderersRecursive(Transform parent, List<SpriteRenderer> spriteRenderers)
        {
            // Check if current object has a SpriteRenderer
            var spriteRenderer = parent.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderers.Add(spriteRenderer);
            }

            // Recursively check all children
            for (int i = 0; i < parent.childCount; i++)
            {
                GetSpriteRenderersRecursive(parent.GetChild(i), spriteRenderers);
            }
        }

        private void SetAlphaForAllSprites(List<SpriteRenderer> spriteRenderers, float alpha)
        {
            foreach (var spriteRenderer in spriteRenderers)
            {
                var color = spriteRenderer.color;
                color.a = alpha;
                spriteRenderer.color = color;
            }
        }
    }
}
