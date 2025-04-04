using System.Collections;
using System.Collections.Generic;
using Dialog.Scripts;
using UnityEngine;

public class BackpackController : ObserverSubject
{
    [SerializeField] private HashSet<GameObject> itemsInBackpack = new HashSet<GameObject>();
    [SerializeField] private GameObject[] items;
    [SerializeField] private NarrationDialogLine itemDoesNotFitDialogLine;
    [SerializeField] private KeyCode toggleKey = KeyCode.F;

    private bool isItemActive;

    void Update()
    {
        // on right mouse click, remove the flashlight from the backpack
        if (Input.GetMouseButtonDown(1) && isItemActive)
        {
            foreach (var activeItem in items) activeItem.SetActive(false);
            foreach (var item in itemsInBackpack) item.GetComponent<SpriteRenderer>().enabled = true;

            isItemActive = false;
        }


        if (Input.GetKeyDown(toggleKey))
        {
            var flashLight = GetActiveItemByName("flashlight");
            if (!flashLight.activeSelf)
            {
                // notify only for non flashlight items
                Notify(GameEvents.TriggerSpecificDialogLine, itemDoesNotFitDialogLine);
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Collectible Item"))
        {
            if (itemsInBackpack.Contains(collision.gameObject))
                return;

            itemsInBackpack.Add(collision.gameObject);
            collision.gameObject.layer = LayerMask.NameToLayer("UI");
            collision.gameObject.transform.SetParent(transform);
            var rigidbody = collision.gameObject.GetComponent<Rigidbody2D>();

            rigidbody.velocity = Vector2.zero;
            rigidbody.mass = 0.1f;
            rigidbody.gravityScale = 10;
        }
    }

    public GameObject GetItemInBackpack(string itemName)
    {
        foreach (var item in itemsInBackpack)
        {
            if (item.name == itemName) return item;
        }

        return null;
    }

    public GameObject GetActiveItemByName(string itemName)
    {
        foreach (var item in items)
        {
            if (item.name == itemName) return item;
        }

        return null;
    }

    public void OnNotify(GameEventData eventData)
    {
        if (eventData.Name == GameEvents.ClickOnItem)
        {
            var itemName = eventData.Data as string;
            var item = GetItemInBackpack(itemName);

            print("Item name: " + itemName);

            if (itemName.ToLower() == "flashlight (outie)")
            {
                var activeItem = GetActiveItemByName("flashlight (innie)");
                activeItem.SetActive(false);
                item = GetItemInBackpack("flashlight (innie)");
                item.GetComponent<SpriteRenderer>().enabled = false;
                return;
            }

            if (item)
            {
                item.GetComponent<SpriteRenderer>().enabled = false;

                foreach (var backpackItem in itemsInBackpack)
                {
                    if (backpackItem.name != itemName)
                    {
                        backpackItem.GetComponent<SpriteRenderer>().enabled = true;
                    }

                    if (item.name != "")
                    {
                        var activeItem = GetActiveItemByName(item.name);
                        activeItem.SetActive(true);
                        isItemActive = true;
                    }
                }
            }
        }
    }
}
