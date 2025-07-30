using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Common.scripts
{
    public class SceneChangeOnTrigger : MonoBehaviour
    {
        [SerializeField] private Object sceneAsset = null;
        private string sceneName;

        private void OnValidate()
        {
            if (sceneAsset != null)
                sceneName = sceneAsset.name;
        }

        private void OnMouseDown()
        {
            if (!string.IsNullOrEmpty(sceneName))
                SceneManager.LoadScene(sceneName);
            else
                Debug.LogWarning("No scene selected to load!");
        }
    }
}