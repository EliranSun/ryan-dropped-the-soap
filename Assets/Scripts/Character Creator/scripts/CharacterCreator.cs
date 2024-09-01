using UnityEngine;
using System;

namespace Character_Creator.scripts
{

    [Serializable]
    class Eyes {
        [SerializeField] private Sprite eye;
        [SerializeField] private GameObject painting;
    }

    public class CharacterCreator : MonoBehaviour
    {
        [SerializeField] private Eyes[] eyes;

        private void Start()
        {
        }

        // Update is called once per frame
        private void Update()
        {
        }
    }
}