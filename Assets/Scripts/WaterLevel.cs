using System.Collections;
using UnityEngine;

public class WaterLevel : MonoBehaviour {
    [SerializeField] private float maxHeight = 3;
    [SerializeField] private float perspectiveChangeHeight = 1.5f;

    // [SerializeField] private float maxWaterLevelHeight = 3;
    [SerializeField] private float pumpingLevelChange = 0.05f;
    [SerializeField] private float waterLevelChange = 0.01f;
    [SerializeField] private float waterVerticalTransition;


    // [SerializeField] private float drowningWaterLevelHeight = 2f;
    // [SerializeField] private float waterLevelWaterLevelHeight = 3f;
    [SerializeField] private GameObject normalPerspectiveGameObject;
    [SerializeField] private GameObject perspectiveChangeGameObject;
    [SerializeField] private GameObject outsideViewGameObject;
    private float _minHeight;
    private float _screenHeight;
    private Transform _waterTransform;

    private void Start() {
        normalPerspectiveGameObject.SetActive(true);
        _waterTransform = transform;
        _minHeight = transform.position.y;

        if (Camera.main)
            _screenHeight = Camera.main.orthographicSize * 2f;
    }

    private void Update() {
        if (waterVerticalTransition == 0)
            return;

        if (_waterTransform.position.y >= perspectiveChangeHeight && !perspectiveChangeGameObject.activeSelf) {
            normalPerspectiveGameObject.SetActive(false);
            perspectiveChangeGameObject.SetActive(true);
        }

        if (_waterTransform.position.y >= maxHeight) {
            outsideViewGameObject.SetActive(true);
            _waterTransform = outsideViewGameObject.transform;
        }

        // if (_waterTransform.position.y >= drowningWaterLevelHeight && !GameState.WaterFilledShower)
        //     GameState.WaterFilledShower = true;
        //
        // if (_waterTransform.position.y >= waterLevelWaterLevelHeight && !GameState.WaterFilledRoom)
        //     // this is the outside water _waterTransform which should affect the entire level
        //     GameState.WaterFilledRoom = true;
        //
        // if (_waterTransform.position.y <= drowningWaterLevelHeight && GameState.WaterFilledShower)
        //     GameState.WaterFilledShower = false;
        //
        // if (_waterTransform.position.y <= waterLevelWaterLevelHeight && GameState.WaterFilledRoom)
        //     GameState.WaterFilledRoom = false;


        if (_waterTransform.position.y >= _screenHeight)
            return;

        _waterTransform.Translate(new Vector2(0, waterVerticalTransition * Time.deltaTime));
    }

    public void OnNotify(GameEventData eventData) {
        switch (eventData.name) {
            case GameEvents.Pumping:
                if (transform.localPosition.y <= _minHeight)
                    break;

                StartCoroutine(TemporaryWaterChange(-pumpingLevelChange));
                break;

            case GameEvents.FaucetOpening:
                waterVerticalTransition += waterLevelChange;
                break;

            case GameEvents.FaucetClosing:
                if (waterVerticalTransition <= 0)
                    return;

                waterVerticalTransition -= waterLevelChange;
                break;
        }
    }

    private IEnumerator TemporaryWaterChange(float levelChange) {
        waterVerticalTransition += levelChange;
        yield return new WaitForSeconds(1);
        waterVerticalTransition -= levelChange;
    }
}