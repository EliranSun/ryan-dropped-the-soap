using UnityEngine;
using UnityEngine.SceneManagement;

namespace Common.scripts
{
    public class SceneChangeOnTrigger : MonoBehaviour
    {
        [SerializeField] private UnityEngine.Object sceneAsset;
        private string _sceneName;

        private void OnMouseDown()
        {
            if (!string.IsNullOrEmpty(_sceneName))
                SceneManager.LoadScene(_sceneName);
            else
                Debug.LogWarning("No scene selected to load!");
        }

        private void OnValidate()
        {
            if (sceneAsset != null)
                _sceneName = sceneAsset.name;
        }
    }
}