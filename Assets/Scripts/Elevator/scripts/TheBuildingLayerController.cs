using System.Collections;
using System.Collections.Generic;
using Object.Scripts;
using UnityEngine;

namespace Elevator.scripts
{
    public class TheBuildingLayerController : MonoBehaviour
    {
        [SerializeField] private GameObject[] layers;
        [SerializeField] private Camera mainCamera;
        [SerializeField] private Color skyColor;
        [SerializeField] private Color halfDarkHalfSkyColor;
        [SerializeField] private Color fullDarkColor;

        public void OnNotify(GameEventData eventData)
        {
            if (eventData.Name != GameEvents.PlayerInteraction)
                return;

            var interactedObject = (ObjectNames)eventData.Data;

            switch (interactedObject)
            {
                case ObjectNames.BuildingEntrance:
                    StartCoroutine(FadeOutLayer(0));
                    StartCoroutine(FadeInLayer(1));
                    StartCoroutine(DarkenCamera(halfDarkHalfSkyColor));
                    break;

                case ObjectNames.StaircaseEntrance:
                    StartCoroutine(FadeOutLayer(1));
                    StartCoroutine(FadeInLayer(2));
                    StartCoroutine(DarkenCamera(fullDarkColor));
                    break;

                case ObjectNames.StaircaseExit:
                    StartCoroutine(FadeInLayer(1));
                    StartCoroutine(FadeOutLayer(2));
                    StartCoroutine(DarkenCamera(halfDarkHalfSkyColor));
                    break;

                case ObjectNames.Elevator:
                    StartCoroutine(DarkenCamera(fullDarkColor));
                    break;
            }
        }

        private IEnumerator DarkenCamera(Color targetColor)
        {
            var startColor = mainCamera.backgroundColor;
            var t = 0f;

            while (t < 2f)
            {
                t += 0.1f;
                t = Mathf.Min(t, 1f);
                mainCamera.backgroundColor = Color.Lerp(startColor, targetColor, t);
                yield return new WaitForSeconds(0.1f);
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

            var currentAlpha = spriteRenderers[0].color.a;

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

            var currentAlpha = spriteRenderers[0].color.a;

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
            if (spriteRenderer != null) spriteRenderers.Add(spriteRenderer);

            // Recursively check all children
            for (var i = 0; i < parent.childCount; i++)
                GetSpriteRenderersRecursive(parent.GetChild(i), spriteRenderers);
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