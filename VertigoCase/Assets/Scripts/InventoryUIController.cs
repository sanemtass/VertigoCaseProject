using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class InventoryUIController : MonoBehaviour
{
    public static InventoryUIController Instance { get; private set; }

    public Image[] inventorySlots; //It includes Image UI elements for inventory slots.
    public Image centerScreenItemDisplay;
    public TextMeshProUGUI[] inventoryQuantity_value;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("More than one instance of Inventory UI Controller!");
        }
        Instance = this;
    }

    private void Update()
    {
        HideEmptySlots();
    }

    private void HideEmptySlots()
    {
        foreach (var slot in inventorySlots)
        {
            if (slot.sprite == null)
            {
                slot.color = new Color(1, 1, 1, 0);
            }
            else
            {
                slot.color = new Color(1, 1, 1, 1);
            }
        }
    }

    public IEnumerator ShowItemInInventory(SliceItemSO sliceItem)
    {
        yield return StartCoroutine(AddItemToInventoryAfterDelay(sliceItem, 2f));
    }

    private IEnumerator AddItemToInventoryAfterDelay(SliceItemSO sliceItem, float delay)
    {
        yield return new WaitForSeconds(delay);

        //Create a copy of the item
        GameObject itemCopy = new GameObject("ItemCopy");
        Image itemCopyImage = itemCopy.AddComponent<Image>();
        itemCopyImage.sprite = sliceItem.itemIcon_value;

        //Start the animation sequence
        RectTransform itemRectTransform = itemCopy.GetComponent<RectTransform>();
        itemRectTransform.SetParent(centerScreenItemDisplay.transform.parent);
        itemRectTransform.position = centerScreenItemDisplay.transform.position;
        itemRectTransform.localScale = Vector3.zero;

        Sequence sequence = DOTween.Sequence();
        sequence.Append(itemRectTransform.DOScale(1, 0.5f).SetEase(Ease.OutBack))
            .Join(itemRectTransform.DORotate(new Vector3(0, 0, 360), 0.5f, RotateMode.FastBeyond360))
            .Append(itemRectTransform.DOScale(0, 0.5f).SetEase(Ease.InBack))
            .Join(itemRectTransform.DORotate(new Vector3(0, 0, 360), 0.5f, RotateMode.FastBeyond360))
            .Append(itemRectTransform.DOMove(GetFirstEmptyInventorySlot().transform.position, 1f))
            .OnComplete(() => {
                
                AddItemToInventory(sliceItem, itemCopyImage);
                Destroy(itemCopy);
            });

        yield return sequence.WaitForCompletion();
    }

    private Image GetFirstEmptyInventorySlot()
    {
        foreach (var slot in inventorySlots)
        {
            if (slot.sprite == null)
            {
                return slot;
            }
        }
        return null;
    }

    private void AddItemToInventory(SliceItemSO sliceItem, Image itemCopyImage)
    {
        int existingItemIndex = GetExistingItemSlotIndex(sliceItem.itemIcon_value);

        if (existingItemIndex != -1)
        {
            int currentQuantity = int.Parse(inventoryQuantity_value[existingItemIndex].text);
            inventoryQuantity_value[existingItemIndex].text = "" + (currentQuantity + sliceItem.itemQuantity_value);
        }
        else
        {
            int firstEmptySlotIndex = GetFirstEmptyInventorySlotIndex();

            if (firstEmptySlotIndex != -1)
            {
                inventorySlots[firstEmptySlotIndex].sprite = itemCopyImage.sprite;
                inventorySlots[firstEmptySlotIndex].preserveAspect = true; // Preserve Aspect is set to true
                inventoryQuantity_value[firstEmptySlotIndex].text = "" + sliceItem.itemQuantity_value;
            }
        }
    }

    private int GetExistingItemSlotIndex(Sprite itemIcon)
    {
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if (inventorySlots[i].sprite == itemIcon)
            {
                return i;
            }
        }
        return -1;
    }

    private int GetFirstEmptyInventorySlotIndex()
    {
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if (inventorySlots[i].sprite == null)
            {
                return i;
            }
        }
        return -1;
    }


}
