using System;
using UnityEngine;
using UnityEngine.UI;

namespace Character_Creator.scripts
{
    [Serializable]
    internal class Eyes
    {
        [SerializeField] public Sprite eye;
        [SerializeField] public GameObject painting;
    }

    public class CharacterCreator : MonoBehaviour
    {
        [SerializeField] private GameObject eyesContainer;
        [SerializeField] private Eyes[] eyes;

        public void OnPaintingClick(GameEventData gameData)
        {
            if (gameData.name == GameEvents.PaintingClicked)
            {
                var eye = Array.Find(eyes, e => e.painting.name == (string)gameData.data);
                if (eye != null) eyesContainer.GetComponent<Image>().sprite = eye.eye;
            }
        }
    }
}