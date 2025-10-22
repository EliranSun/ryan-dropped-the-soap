using System;
using System.Collections;
using System.Collections.Generic;
using Object.Scripts;
using Player;
using UnityEngine;

namespace Elevator.scripts
{
    public enum BuildingLayerType
    {
        Outside,
        InBuilding,
        Staircase,
        Elevator
    }

    [Serializable]
    public class BuildingLayers
    {
        [SerializeField] private GameObject outsideLayer;
        [SerializeField] private GameObject inBuildingLayer;
        [SerializeField] private GameObject staircaseLayer;
        [SerializeField] private GameObject elevatorLayer;

        public GameObject GetLayer(BuildingLayerType layerType)
        {
            return layerType switch
            {
                BuildingLayerType.Outside => outsideLayer,
                BuildingLayerType.InBuilding => inBuildingLayer,
                BuildingLayerType.Staircase => staircaseLayer,
                BuildingLayerType.Elevator => elevatorLayer,
                _ => throw new ArgumentOutOfRangeException(nameof(layerType), layerType, null)
            };
        }
    }

    public class TheBuildingLayerController : MonoBehaviour
    {
        [SerializeField] private BuildingLayerType initialLayer = BuildingLayerType.Outside;
        [SerializeField] private BuildingLayers layers;
        [SerializeField] private Camera mainCamera;
        [SerializeField] private Color skyColor;
        [SerializeField] private Color halfDarkHalfSkyColor;
        [SerializeField] private Color fullDarkColor;

        private BuildingLayerType _currentActiveLayer;

        private void Start()
        {
            // Initialize all layers and set the initial layer as active
            // InitializeAllLayers();
            // SetActiveLayer(initialLayer);
        }

        private void InitializeAllLayers()
        {
            // Set all layers active and fade them out, except the initial layer
            foreach (BuildingLayerType layerType in Enum.GetValues(typeof(BuildingLayerType)))
            {
                var layerObject = layers.GetLayer(layerType);
                layerObject.SetActive(true);

                if (layerType != initialLayer)
                {
                    StartCoroutine(FadeOutLayer(layerObject));
                }
                else
                {
                    StartCoroutine(FadeInLayer(layerObject));
                    _currentActiveLayer = layerType;
                }
            }
        }

        private void SetActiveLayer(BuildingLayerType targetLayer)
        {
            if (_currentActiveLayer != targetLayer)
                StartCoroutine(FadeOutLayer(layers.GetLayer(_currentActiveLayer)));

            StartCoroutine(FadeInLayer(layers.GetLayer(targetLayer)));
            StartCoroutine(UpdateLayersActiveState(targetLayer));
        }

        private IEnumerator UpdateLayersActiveState(BuildingLayerType targetLayer)
        {
            yield return new WaitForEndOfFrame();

            foreach (BuildingLayerType layerType in Enum.GetValues(typeof(BuildingLayerType)))
            {
                var obj = layers.GetLayer(layerType);
                obj.SetActive(layerType == targetLayer);
            }

            _currentActiveLayer = targetLayer;
        }

        public void OnNotify(GameEventData eventData)
        {
            if (eventData.Name == GameEvents.ChangeActiveLayer)
            {
                var newLayer = (BuildingLayerType)eventData.Data;
                // if (newLayer != _currentActiveLayer)
                SetActiveLayer(newLayer);
                return;
            }

            if (eventData.Name == GameEvents.EnterElevator)
            {
                SetActiveLayer(BuildingLayerType.Elevator);
                return;
            }

            if (eventData.Name == GameEvents.PlayerInteraction)
            {
                var interactedObject = (Interaction)eventData.Data;

                switch (interactedObject.objectName)
                {
                    case ObjectNames.BuildingExit:
                        SetActiveLayer(BuildingLayerType.Outside);
                        break;

                    case ObjectNames.BuildingEntrance:
                        SetActiveLayer(BuildingLayerType.InBuilding);
                        break;

                    case ObjectNames.StaircaseEntrance:
                        SetActiveLayer(BuildingLayerType.Staircase);
                        break;

                    case ObjectNames.StaircaseExit:
                        SetActiveLayer(BuildingLayerType.InBuilding);
                        break;

                    case ObjectNames.ElevatorExitDoors:
                        SetActiveLayer(BuildingLayerType.InBuilding);
                        break;

                    case ObjectNames.ElevatorEnterDoors:
                        SetActiveLayer(BuildingLayerType.Elevator);
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
            print($"Fade out {layer}");

            var spriteRenderers = GetAllSpriteRenderers(layer);
            // var colliders = layer.GetComponentsInChildren<Collider2D>();
            //
            // foreach (var col in colliders) col.enabled = false;

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
            // yield return new WaitForSeconds(1);

            print($"Fade in {layer}");

            var spriteRenderers = GetAllSpriteRenderers(layer);
            // var colliders = layer.GetComponentsInChildren<Collider2D>();
            //
            // foreach (var col in colliders) col.enabled = true;

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