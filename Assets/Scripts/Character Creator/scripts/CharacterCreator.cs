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

    [Serializable]
    internal class Mouths
    {
        [SerializeField] public Sprite mouth;
        [SerializeField] public GameObject door;
    }

    [Serializable]
    internal class Noses
    {
        [SerializeField] public Sprite nose;
        [SerializeField] public GameObject armChairs;
    }

    [Serializable]
    internal class Hairs
    {
        [SerializeField] public Sprite hairFront;
        [SerializeField] public Sprite hairBack;
        [SerializeField] public GameObject vase;
    }


    public class CharacterCreator : MonoBehaviour
    {
        [SerializeField] private GameObject eyesContainer;
        [SerializeField] private GameObject facesContainer;
        [SerializeField] private GameObject mouthsContainer;
        [SerializeField] private GameObject nosesContainer;
        [SerializeField] private GameObject hairFrontContainer;
        [SerializeField] private GameObject hairBackContainer;
        [SerializeField] private Eyes[] eyes;
        [SerializeField] private Faces[] faces;
        [SerializeField] private Mouths[] mouths;
        [SerializeField] private Noses[] noses;
        [SerializeField] private Hairs[] hairs;

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

        public void OnDoorClick(GameEventData gameData)
        {
            if (gameData.name == GameEvents.DoorClicked)
            {
                var mouth = Array.Find(mouths, m => m.door.name == (string)gameData.data);
                if (mouth != null) mouthsContainer.GetComponent<Image>().sprite = mouth.mouth;
            }
        }

        public void OnVaseClick(GameEventData gameData)
        {
            if (gameData.name == GameEvents.VaseClicked)
            {
                var hair = Array.Find(hairs, h => h.vase.name == (string)gameData.data);
                if (hair != null) hairBackContainer.GetComponent<Image>().sprite = hair.hairBack;
                if (hair != null) hairFrontContainer.GetComponent<Image>().sprite = hair.hairFront;
            }
        }

        public void OnArmchairClick(GameEventData gameData)
        {
            if (gameData.name == GameEvents.ArmchairClicked)
            {
                var nose = Array.Find(noses, n => n.armChairs.name == (string)gameData.data);
                if (nose != null) nosesContainer.GetComponent<Image>().sprite = nose.nose;
            }
        }
    }
}