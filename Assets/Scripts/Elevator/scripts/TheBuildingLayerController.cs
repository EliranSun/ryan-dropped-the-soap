using System;
using System.Collections;
using System.Collections.Generic;
using Object.Scripts;
using UnityEngine;

namespace Elevator.scripts
{
    [Serializable]
    public class BuildingLayers
    {
        public GameObject outsideLayer;
        public GameObject inBuildingLayer;
        public GameObject staircaseLayer;
        public GameObject elevatorLayer;
    }

    public class TheBuildingLayerController : MonoBehaviour
    {
        [SerializeField] private Camera mainCamera;
        [SerializeField] private Color skyColor;
        [SerializeField] private Color halfDarkHalfSkyColor;
        [SerializeField] private Color fullDarkColor;
        [SerializeField] private BuildingLayers layers;

        private void Start()
        {
            StartCoroutine(FadeOutLayer(layers.outsideLayer));
            layers.outsideLayer.SetActive(true);
            StartCoroutine(FadeOutLayer(layers.inBuildingLayer));
            layers.inBuildingLayer.SetActive(true);
            StartCoroutine(FadeOutLayer(layers.staircaseLayer));
            layers.staircaseLayer.SetActive(true);
            StartCoroutine(FadeInLayer(layers.elevatorLayer));
            layers.elevatorLayer.SetActive(true);
        }

        public void OnNotify(GameEventData eventData)
        {
            if (eventData.Name == GameEvents.EnterElevator)
            {
                StartCoroutine(FadeOutLayer(layers.inBuildingLayer));
                StartCoroutine(FadeInLayer(layers.elevatorLayer));
            }

            if (eventData.Name != GameEvents.PlayerInteraction)
                return;

            var interactedObject = (ObjectNames)eventData.Data;

            switch (interactedObject)
            {
                case ObjectNames.BuildingEntrance:
                    StartCoroutine(FadeOutLayer(layers.outsideLayer));
                    StartCoroutine(FadeInLayer(layers.inBuildingLayer));
                    break;

                case ObjectNames.StaircaseEntrance:
                    StartCoroutine(FadeOutLayer(layers.inBuildingLayer));
                    StartCoroutine(FadeInLayer(layers.staircaseLayer));
                    break;

                case ObjectNames.StaircaseExit:
                    StartCoroutine(FadeInLayer(layers.inBuildingLayer));
                    StartCoroutine(FadeOutLayer(layers.staircaseLayer));
                    break;

                // TODO: Confusing. That's the exit object trigger
                case ObjectNames.Elevator:
                {
                    StartCoroutine(FadeOutLayer(layers.elevatorLayer));
                    StartCoroutine(FadeInLayer(layers.inBuildingLayer));
                    break;
                }
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

        private IEnumerator FadeOutLayer(GameObject layer)
        {
            var spriteRenderers = GetAllSpriteRenderers(layer);

            var colliders = layer.GetComponentsInChildren<Collider2D>();
            foreach (var col in colliders) col.enabled = false;

            if (spriteRenderers.Count == 0) yield break;

            var currentAlpha = spriteRenderers[0].color.a;

            while (currentAlpha > 0)
            {
                currentAlpha -= 0.1f;
                currentAlpha = Mathf.Max(0, currentAlpha);

                SetAlphaForAllSprites(spriteRenderers, currentAlpha);
                yield return new WaitForSeconds(0.1f);
            }
        }

        private IEnumerator FadeInLayer(GameObject layer)
        {
            var spriteRenderers = GetAllSpriteRenderers(layer);

            var colliders = layer.GetComponentsInChildren<Collider2D>();
            foreach (var col in colliders) col.enabled = true;

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

        private static List<SpriteRenderer> GetAllSpriteRenderers(GameObject parent)
        {
            return new List<SpriteRenderer>(parent.GetComponentsInChildren<SpriteRenderer>());
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