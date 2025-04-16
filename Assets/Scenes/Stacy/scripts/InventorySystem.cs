using System;
using Mini_Games.Organize_Desk.scripts;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public enum ItemName
{
    Flashlight,
    Knife,
}

[Serializable]
public class InventoryItem
{
    public ItemName name;
    public Image uiImage;
    public GameObject gameObject;
    public bool isHeldByPlayer;
}

public class InventorySystem : MonoBehaviour
{
    [SerializeField] private InventoryItem flashlightItem;
    [SerializeField] private InventoryItem knifeItem;
    [SerializeField] private InventoryItem rope;
    [SerializeField] private InventoryItem sandwich;

    private bool isItemHeldByPlayer;


    public void OnImageClick(string imageName)
    {
        var isFlashLightClicked = imageName == flashlightItem.name.ToString().ToLower();
        var isKnifeClicked = imageName == knifeItem.name.ToString().ToLower();

        if (isItemHeldByPlayer) return;

        if (isFlashLightClicked) TakeOutItem(flashlightItem);
        if (isKnifeClicked) TakeOutItem(knifeItem);
    }

    public void PutItemInBag(InventoryItem item)
    {
        EnableUI(item.uiImage);
        item.gameObject.SetActive(false);
        item.isHeldByPlayer = false;
        isItemHeldByPlayer = false;
    }

    public void TakeOutItem(InventoryItem item)
    {
        DisableUI(item.uiImage);
        item.gameObject.SetActive(true);
        item.isHeldByPlayer = true;
        isItemHeldByPlayer = true;
    }

    private void EnableUI(Image itemImage)
    {
        itemImage.color = new Color(1, 1, 1, 1f);
    }

    private void DisableUI(Image itemImage)
    {
        itemImage.color = new Color(1, 1, 1, 0.1f);
    }


    public void OnNotify(GameEventData eventData)
    {
        if (eventData.Name == GameEvents.ClickOnItem)
        {
            var isFlashlight = (string)eventData.Data == ItemName.Flashlight.ToString().ToLower();
            var isKnife = (string)eventData.Data == ItemName.Knife.ToString().ToLower();

            if (isFlashlight) PutItemInBag(flashlightItem);
            if (isKnife) PutItemInBag(knifeItem);
        }
    }
}
