using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySystem : MonoBehaviour
{
    [SerializeField] Image flashlightImage;

    private void Start()
    {
        DisableItem(flashlightImage);
    }

    public void DisableItem(Image itemImage)
    {
        itemImage.color = new Color(1, 1, 1, 0.1f);
    }

    public void EnableItem(Image itemImage)
    {
        itemImage.color = new Color(1, 1, 1, 1f);
    }


    public void OnNotify(GameEventData eventData)
    {
        if (eventData.Name == GameEvents.FlashlightClicked)
        {
            EnableItem(flashlightImage);
        }
    }
}
