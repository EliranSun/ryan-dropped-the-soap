using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour {
    [SerializeField] private GameObject[] uiElements;
    [SerializeField] private CursorChanger soapCursor;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Camera zoomedInCamera;
    private bool _isClean;
    private bool _isFaucetClosed = true;
    private bool _isInShower;
    private bool _notifiedLevelResolution;
    private bool _timeIsUp;

    private void Update() {
        if (Input.GetKeyDown(KeyCode.R))
            RestartLevel();
    }

    public void OnNotify(GameEventData eventData) {
        switch (eventData.name) {
            case GameEvents.FaucetOpening:
                _isFaucetClosed = false;
                break;

            case GameEvents.FaucetClosed:
                _isFaucetClosed = true;
                if (_isClean && !_isInShower)
                    Invoke(nameof(HandleTimeUp), 2);

                break;

            case GameEvents.InShower:
                _isInShower = true;
                break;

            case GameEvents.OutOfShower: {
                _isInShower = false;
                if (_isClean && _isFaucetClosed)
                    Invoke(nameof(HandleTimeUp), 2);

                break;
            }

            case GameEvents.IsClean:
                _isClean = true;
                break;

            case GameEvents.Dead:
            case GameEvents.TimeIsUp:
                Invoke(nameof(HandleTimeUp), 2);
                break;

            case GameEvents.SoapMiniGameWon:
                soapCursor.TriggerGrab();
                mainCamera.enabled = false;
                zoomedInCamera.enabled = true;
                break;

            case GameEvents.SoapDropped:
                mainCamera.enabled = true;
                zoomedInCamera.enabled = false;
                break;

            case GameEvents.PickedItem: {
                if (zoomedInCamera) {
                    mainCamera.enabled = false;
                    zoomedInCamera.enabled = true;
                }

                break;
            }

            case GameEvents.DroppedItem: {
                if (zoomedInCamera) {
                    mainCamera.enabled = true;
                    zoomedInCamera.enabled = false;
                }

                break;
            }
        }
    }

    private void HandleTimeUp() {
        if (_notifiedLevelResolution)
            return;

        var isWin = _isClean && !_isInShower && _isFaucetClosed;

        // Notify(isWin ? GameEvents.LevelWon : GameEvents.LevelLost);
        EventManager.Instance.Publish(isWin ? GameEvents.LevelWon : GameEvents.LevelLost);

        foreach (var uiElement in uiElements)
            uiElement.gameObject.SetActive(false);

        _notifiedLevelResolution = true;
    }

    public static void RestartLevel() {
        GameState.Reset();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public static void NextLevel() {
        GameState.Reset();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}