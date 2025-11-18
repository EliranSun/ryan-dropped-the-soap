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
        Hallway,
        Staircase,
        Elevator,
        Apartment
    }

    [Serializable]
    public class BuildingLayers
    {
        [SerializeField] private GameObject[] outsideLayers;
        [SerializeField] private GameObject[] inBuildingLayers;
        [SerializeField] private GameObject[] staircaseLayers;
        [SerializeField] private GameObject[] elevatorLayers;
        [SerializeField] private GameObject[] apartmentsLayers;

        public GameObject[] GetLayer(BuildingLayerType layerType)
        {
            return layerType switch
            {
                BuildingLayerType.Outside => outsideLayers,
                BuildingLayerType.Hallway => inBuildingLayers,
                BuildingLayerType.Staircase => staircaseLayers,
                BuildingLayerType.Elevator => elevatorLayers,
                BuildingLayerType.Apartment => apartmentsLayers,
                _ => throw new ArgumentOutOfRangeException(nameof(layerType), layerType, null)
            };
        }
    }

    public class TheBuildingLayerController : ObserverSubject
    {
        [SerializeField] private GameObject player;
        [SerializeField] private Camera mainCamera;
        [SerializeField] private Color skyColor;
        [SerializeField] private Color halfDarkHalfSkyColor;
        [SerializeField] private Color fullDarkColor;
        [SerializeField] private BuildingLayerType initialLayer = BuildingLayerType.Outside;
        [SerializeField] private BuildingLayers layers;

        private BuildingLayerType _currentActiveLayer;

        private void Start()
        {
            // Initialize all layers and set the initial layer as active
            InitializeAllLayers();
            SetActiveLayer(initialLayer);
        }

        private void InitializeAllLayers()
        {
            // Set all layers active and fade them out, except the initial layer
            foreach (BuildingLayerType layerType in Enum.GetValues(typeof(BuildingLayerType)))
            {
                var layersObject = layers.GetLayer(layerType);

                foreach (var layerObject in layersObject)
                    layerObject.SetActive(true);


                if (layerType != initialLayer)
                {
                    StartCoroutine(FadeOutLayers(layersObject));
                }
                else
                {
                    StartCoroutine(FadeInLayers(layersObject));
                    _currentActiveLayer = layerType;
                }
            }
        }

        private void SetActiveLayer(BuildingLayerType targetLayer)
        {
            if (_currentActiveLayer != targetLayer)
                StartCoroutine(FadeOutLayers(layers.GetLayer(_currentActiveLayer)));

            StartCoroutine(FadeInLayers(layers.GetLayer(targetLayer)));
            StartCoroutine(UpdateLayersActiveState(targetLayer));
        }

        private IEnumerator UpdateLayersActiveState(BuildingLayerType targetLayer)
        {
            yield return new WaitForEndOfFrame();

            foreach (BuildingLayerType layerType in Enum.GetValues(typeof(BuildingLayerType)))
            {
                var objs = layers.GetLayer(layerType);
                foreach (var obj in objs) obj.SetActive(layerType == targetLayer);
            }

            _currentActiveLayer = targetLayer;

            Notify(GameEvents.LayerChange, targetLayer);
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

            // TODO: Move to appropriate place
            // if (eventData.Name == GameEvents.NpcEnterApartment)
            // {
            //     var dialogProperties = DialogHelper.GetDialogNotificationProperties(eventData);
            //
            //     if (dialogProperties.ActorName == ActorName.Ryan)
            //     {
            //         ryanNpc.transform.parent = layers.GetLayer(BuildingLayerType.Apartment).transform;
            //         charlotteNpc.transform.parent = layers.GetLayer(BuildingLayerType.Apartment).transform;
            //     }
            // }

            if (eventData.Name == GameEvents.PlayerInteraction)
            {
                var interactedObject = (Interaction)eventData.Data;

                switch (interactedObject.objectName)
                {
                    case ObjectNames.BuildingExit:
                        SetActiveLayer(BuildingLayerType.Outside);
                        break;

                    case ObjectNames.BuildingEntrance:
                        SetActiveLayer(BuildingLayerType.Hallway);
                        break;

                    case ObjectNames.StaircaseEntrance:
                        SetActiveLayer(BuildingLayerType.Staircase);
                        var staircaseX = layers.GetLayer(BuildingLayerType.Staircase)[0].transform.position.x;
                        mainCamera.GetComponent<CameraObjectFollow>().LockX(staircaseX);
                        break;

                    case ObjectNames.StaircaseExit:
                        SetActiveLayer(BuildingLayerType.Hallway);
                        mainCamera.GetComponent<CameraObjectFollow>().UnlockX();
                        break;

                    case ObjectNames.ElevatorExitDoors:
                        SetActiveLayer(BuildingLayerType.Hallway);
                        break;

                    case ObjectNames.ElevatorEnterDoors:
                        SetActiveLayer(BuildingLayerType.Elevator);
                        var elevatorX = layers.GetLayer(BuildingLayerType.Elevator)[0].transform.position.x;
                        mainCamera.GetComponent<CameraObjectFollow>().LockX(elevatorX);
                        // mainCamera.GetComponent<Zoom>().LockX(elevatorX);
                        Notify(GameEvents.EnterElevator);
                        break;

                    case ObjectNames.ApartmentEntrance:
                        SetActiveLayer(BuildingLayerType.Apartment);
                        break;

                    case ObjectNames.ApartmentExit:
                        SetActiveLayer(BuildingLayerType.Hallway);
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

        private IEnumerator FadeOutLayers(GameObject[] layerObjects)
        {
            if (layerObjects == null || layerObjects.Length == 0) yield break;

            var spriteRenderers = new List<SpriteRenderer>();
            foreach (var obj in layerObjects)
            {
                if (obj == null) continue;
                spriteRenderers.AddRange(GetAllSpriteRenderers(obj));
            }

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

        private IEnumerator FadeInLayers(GameObject[] layerObjects)
        {
            if (layerObjects == null || layerObjects.Length == 0) yield break;

            var spriteRenderers = new List<SpriteRenderer>();
            foreach (var obj in layerObjects)
            {
                if (obj == null) continue;
                spriteRenderers.AddRange(GetAllSpriteRenderers(obj));
            }

            if (spriteRenderers.Count == 0) yield break;

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