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

    [Serializable]
    internal class Faces
    {
        [SerializeField] public Sprite face;
        [SerializeField] public GameObject mirror;
    }


    public class CharacterCreator : MonoBehaviour
    {
        [SerializeField] private GameObject eyesContainer;
        [SerializeField] private GameObject facesContainer;
        [SerializeField] private Eyes[] eyes;
        [SerializeField] private Faces[] faces;

        public void OnPaintingClick(GameEventData gameData)
        {
            if (gameData.name == GameEvents.PaintingClicked)
            {
                var eye = Array.Find(eyes, e => e.painting.name == (string)gameData.data);
                if (eye != null) eyesContainer.GetComponent<Image>().sprite = eye.eye;
            }
        }

        public void OnMirrorClick(GameEventData gameData)
        {
            if (gameData.name == GameEvents.MirrorClicked)
            {
                var face = Array.Find(faces, f => f.mirror.name == (string)gameData.data);
                if (face != null) facesContainer.GetComponent<Image>().sprite = face.face;
            }
        }
    }
}