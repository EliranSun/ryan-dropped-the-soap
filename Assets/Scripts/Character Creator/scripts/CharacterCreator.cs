using System;
using System.Collections.Generic;
using Dialog.Scripts;
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

    [Serializable]
    internal class CharacterSprite
    {
        public GameObject face;
        public GameObject hair;
        public GameObject body;
    }

    public class CharacterCreator : ObserverSubject
    {
        [SerializeField] private CharacterSprite characterToReveal;
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

        private readonly HashSet<string> _playerChoices = new();

        private void Start()
        {
            HideFaceUI();
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

        public void OnNotify(GameEventData gameData)
        {
            switch (gameData.Name)
            {
                case GameEvents.VaseChosen:
                    var vase = (EnrichedPlayerChoice)gameData.Data;
                    OnVaseChoice(vase.OriginalInteraction.Name);
                    break;

                case GameEvents.ArmchairChosen:
                    var armchair = (EnrichedPlayerChoice)gameData.Data;
                    OnArmchairChoice(armchair.OriginalInteraction.Name);
                    break;

                case GameEvents.DoorChosen:
                    var door = (EnrichedPlayerChoice)gameData.Data;
                    OnDoorChoice(door.OriginalInteraction.Name);
                    break;

                case GameEvents.MirrorChosen:
                    var mirror = (EnrichedPlayerChoice)gameData.Data;
                    OnMirrorChoice(mirror.OriginalInteraction.Name);
                    break;

                case GameEvents.PaintingChosen:
                    var painting = (EnrichedPlayerChoice)gameData.Data;
                    OnPaintingChoice(painting.OriginalInteraction.Name);
                    break;

                case GameEvents.CharacterRevealTrigger:
                    OnCharacterReveal();
                    break;
            }
        }

        public void OnPaintingChoice(string paintingName)
        {
            var eye = Array.Find(eyes, e => e.painting.name == paintingName);
            if (eye != null)
            {
                _playerChoices.Add("eyes");
                eyesContainer.GetComponent<Image>().sprite = eye.eye;
                CheckCharacterReveal();
            }
        }

        public void OnMirrorChoice(string mirrorName)
        {
            var face = Array.Find(faces, f => f.mirror.name == mirrorName);

            if (face.face != null)
            {
                _playerChoices.Add("face");
                facesContainer.GetComponent<Image>().sprite = face.face;
                spriteCreatorContainer.faceContainer.GetComponent<SpriteRenderer>().sprite = face.faceSprite;
                CheckCharacterReveal();
            }
        }

        public void OnDoorChoice(string doorName)
        {
            var mouth = Array.Find(mouths, m => m.door.name == doorName);

            if (mouth != null)
            {
                _playerChoices.Add("mouth");
                mouthsContainer.GetComponent<Image>().sprite = mouth.mouth;
                CheckCharacterReveal();
            }
        }

        public void OnVaseChoice(string vaseName)
        {
            var hair = Array.Find(hairs, h => h.vase.name == vaseName);

            if (hair != null)
            {
                hairBackContainer.GetComponent<Image>().sprite = hair.hairBack;
                hairFrontContainer.GetComponent<Image>().sprite = hair.hairFront;
                spriteCreatorContainer.hairContainer.GetComponent<SpriteRenderer>().sprite = hair.hairSprite;
                _playerChoices.Add("hair");
                CheckCharacterReveal();
            }
        }

        public void OnArmchairChoice(string armchairName)
        {
            var nose = Array.Find(noses, n => n.armChairs.name == armchairName);
            if (nose != null)
            {
                _playerChoices.Add("nose");
                nosesContainer.GetComponent<Image>().sprite = nose.nose;
                CheckCharacterReveal();
            }
        }

        private void CheckCharacterReveal()
        {
            if (_playerChoices.Count == 5)
                Notify(GameEvents.CharacterRevealDialogTrigger);
        }

        // TODO: Trigger after dialog
        private void OnCharacterReveal()
        {
            eyesContainer.GetComponent<Image>().color = new Color(1, 1, 1, 1);
            facesContainer.GetComponent<Image>().color = new Color(1, 1, 1, 1);
            mouthsContainer.GetComponent<Image>().color = new Color(1, 1, 1, 1);
            nosesContainer.GetComponent<Image>().color = new Color(1, 1, 1, 1);
            hairFrontContainer.GetComponent<Image>().color = new Color(1, 1, 1, 1);
            hairBackContainer.GetComponent<Image>().color = new Color(1, 1, 1, 1);

            // set active each part of the character
            characterToReveal.face.GetComponent<SpriteRenderer>().enabled = true;
            characterToReveal.hair.GetComponent<SpriteRenderer>().enabled = true;
            characterToReveal.body.GetComponent<SpriteRenderer>().enabled = true;
        }
    }
}