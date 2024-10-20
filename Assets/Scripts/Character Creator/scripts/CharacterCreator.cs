using System;
using System.Collections.Generic;
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
        [SerializeField] public Sprite faceSprite;
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
        [SerializeField] public Sprite hairSprite;
    }

    [Serializable]
    internal class SpriteCreatorContainer
    {
        [SerializeField] public GameObject faceContainer;
        [SerializeField] public GameObject hairContainer;
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
        [SerializeField] private SpriteCreatorContainer spriteCreatorContainer;

        private readonly Dictionary<string, bool> _playerChoices = new();

        private void Start()
        {
            // HideFaceUI();
        }

        private void HideFaceUI()
        {
            eyesContainer.GetComponent<Image>().color = new Color(1, 1, 1, 0);
            facesContainer.GetComponent<Image>().color = new Color(1, 1, 1, 0);
            mouthsContainer.GetComponent<Image>().color = new Color(1, 1, 1, 0);
            nosesContainer.GetComponent<Image>().color = new Color(1, 1, 1, 0);
            hairFrontContainer.GetComponent<Image>().color = new Color(1, 1, 1, 0);
            hairBackContainer.GetComponent<Image>().color = new Color(1, 1, 1, 0);
        }

        public void OnPaintingClick(GameEventData gameData)
        {
            // TODO: Don't really need double check here,
            // but consider refactoring to use a single method
            if (gameData.name == GameEvents.PaintingClicked)
            {
                var eye = Array.Find(eyes, e =>
                {
                    var interactionData = (InteractionData)gameData.data;
                    return e.painting.name == interactionData.Name;
                });

                if (eye != null && !_playerChoices.ContainsKey("eyes"))
                {
                    eyesContainer.GetComponent<Image>().sprite = eye.eye;
                    _playerChoices.Add("eyes", true);
                }
            }
        }

        public void OnMirrorClick(GameEventData gameData)
        {
            if (gameData.name == GameEvents.MirrorClicked)
            {
                var face = Array.Find(faces, f =>
                {
                    var interactionData = (InteractionData)gameData.data;
                    return f.mirror.name == interactionData.Name;
                });

                print($"Has obtained face? {_playerChoices.ContainsKey("face")}, face? {face.face}");
                if (face.face != null)
                {
                    facesContainer.GetComponent<Image>().sprite = face.face;
                    spriteCreatorContainer.faceContainer.GetComponent<SpriteRenderer>().sprite = face.faceSprite;
                }
            }
        }

        public void OnDoorClick(GameEventData gameData)
        {
            if (gameData.name == GameEvents.DoorClicked)
            {
                var mouth = Array.Find(mouths, m =>
                {
                    var interactionData = (InteractionData)gameData.data;
                    return m.door.name == interactionData.Name;
                });

                if (mouth != null && !_playerChoices.ContainsKey("mouth"))
                {
                    mouthsContainer.GetComponent<Image>().sprite = mouth.mouth;
                    _playerChoices.Add("mouth", true);
                }
            }
        }

        public void OnVaseClick(GameEventData gameData)
        {
            if (gameData.name == GameEvents.VaseClicked)
            {
                var hair = Array.Find(hairs, h =>
                {
                    var interactionData = (InteractionData)gameData.data;
                    return h.vase.name == interactionData.Name;
                });
                // !_playerChoices.ContainsKey("hair")
                if (hair != null)
                {
                    hairBackContainer.GetComponent<Image>().sprite = hair.hairBack;
                    hairFrontContainer.GetComponent<Image>().sprite = hair.hairFront;
                    spriteCreatorContainer.hairContainer.GetComponent<SpriteRenderer>().sprite = hair.hairSprite;
                    // _playerChoices.Add("hair", true);
                }
            }
        }

        public void OnArmchairClick(GameEventData gameData)
        {
            if (gameData.name == GameEvents.ArmchairClicked)
            {
                var nose = Array.Find(noses, n =>
                {
                    var interactionData = (InteractionData)gameData.data;
                    return n.armChairs.name == interactionData.Name;
                });
                if (nose != null && !_playerChoices.ContainsKey("nose"))
                {
                    nosesContainer.GetComponent<Image>().sprite = nose.nose;
                    _playerChoices.Add("nose", true);
                }
            }
        }

        // TODO: Trigger after dialog
        public void OnCharacterReveal()
        {
            if (_playerChoices.Count == 5)
            {
                eyesContainer.GetComponent<Image>().color = new Color(1, 1, 1, 1);
                facesContainer.GetComponent<Image>().color = new Color(1, 1, 1, 1);
                mouthsContainer.GetComponent<Image>().color = new Color(1, 1, 1, 1);
                nosesContainer.GetComponent<Image>().color = new Color(1, 1, 1, 1);
                hairFrontContainer.GetComponent<Image>().color = new Color(1, 1, 1, 1);
                hairBackContainer.GetComponent<Image>().color = new Color(1, 1, 1, 1);
            }
        }
    }
}