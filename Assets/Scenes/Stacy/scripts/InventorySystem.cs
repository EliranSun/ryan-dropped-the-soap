using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySystem : MonoBehaviour
{
    [SerializeField] Image flashlightImage;

    private void Start()
    {
        flashlightImage.color = new Color(1, 1, 1, 0.1f);
    }

    public void AddFlashlight()
    {
        flashlightImage.color = new Color(1, 1, 1, 1);
    }

    public void RemoveFlashlight()
    {
        flashlightImage.color = new Color(1, 1, 1, 0.1f);
    }

}
