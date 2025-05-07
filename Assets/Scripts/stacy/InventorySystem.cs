using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public enum ItemName
{
    None,
    Flashlight,
    Knife,
    Sandwich,
    Rope
}

[Serializable]
public class InventoryItem
{
    public ItemName name;
    public Image uiImage;
    public GameObject gameObject;
    public bool isHeldByPlayer;
}

public class InventorySystem : ObserverSubject
{
    [SerializeField] private InventoryItem flashlightItem;
    [SerializeField] private InventoryItem knifeItem;
    [SerializeField] private InventoryItem sandwichItem;
    [SerializeField] private InventoryItem ropeItem;

    private bool isRopeUsed;
    private bool isItemHeldByPlayer;

    public void Start()
    {
        // for testing
        PutItemInBag(ropeItem);
    }

    public void OnImageClick(string imageName)
    {
        var isFlashLightClicked = imageName == flashlightItem.name.ToString().ToLower();
        var isKnifeClicked = imageName == knifeItem.name.ToString().ToLower();
        var isSandwichClicked = imageName == sandwichItem.name.ToString().ToLower();
        var isRopeClicked = imageName == ropeItem.name.ToString().ToLower();

        if (isItemHeldByPlayer) return;

        if (isRopeClicked)
        {
            Notify(GameEvents.RopeInHand);
            TakeOutItem(ropeItem);
        }

        if (isFlashLightClicked) 
            TakeOutItem(flashlightItem);

        if (isKnifeClicked) 
            TakeOutItem(knifeItem);

        if (isSandwichClicked)
        {
            Notify(GameEvents.SandwichFed);
            DisableUI(sandwichItem.uiImage);
        }
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
            var isRope = (string)eventData.Data == ItemName.Rope.ToString().ToLower();

            if (isFlashlight) 
                PutItemInBag(flashlightItem);

            if (isKnife) 
                PutItemInBag(knifeItem);

            if (isRope)
            {
                Notify(GameEvents.RopeInBag);
                PutItemInBag(ropeItem);
            }
        }

        if (eventData.Name == GameEvents.RopeAttached)
        {
            isRopeUsed = true;
            ropeItem.gameObject.SetActive(false);
        }

        if (eventData.Name == GameEvents.RopeDetached)
        {
            isRopeUsed = false;
            ropeItem.gameObject.SetActive(true);
        }
    }
}