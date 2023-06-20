using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System.Collections;

public class InventoryUIController : MonoBehaviour
{
    public static InventoryUIController Instance { get; private set; }

    public Image[] inventorySlots; // Envanter slotları için Image UI elementlerini içerir
    public Image centerScreenItemDisplay;

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
        //HideCenterScreenItem();
    }

    private void HideEmptySlots()
    {
        foreach (var slot in inventorySlots)
        {
            if (slot.sprite == null) // Eğer bu slot boşsa
            {
                slot.color = new Color(1, 1, 1, 0); // Image'i görünmez yap
            }
            else
            {
                slot.color = new Color(1, 1, 1, 1); // Image'i görünür yap
            }
        }
    }

    public IEnumerator ShowItemInInventory(SliceItemSO sliceItem)
    {
        yield return StartCoroutine(AddItemToInventoryAfterDelay(sliceItem, 2f)); // 2 saniye bekledikten sonra kazanılan item'ı envanter paneline ekler
    }

    private IEnumerator AddItemToInventoryAfterDelay(SliceItemSO sliceItem, float delay)
    {
        yield return new WaitForSeconds(delay);

        // Create a copy of the item
        GameObject itemCopy = new GameObject("ItemCopy");
        Image itemCopyImage = itemCopy.AddComponent<Image>();
        itemCopyImage.sprite = sliceItem.itemIcon;

        // Start the animation sequence
        RectTransform itemRectTransform = itemCopy.GetComponent<RectTransform>();
        itemRectTransform.SetParent(centerScreenItemDisplay.transform.parent); // Set the parent to the same parent as the centerScreenItemDisplay
        itemRectTransform.position = centerScreenItemDisplay.transform.position; // Start the animation at the center of the screen
        itemRectTransform.localScale = Vector3.zero; // Start with the item scaled to 0

        // Scale up the item, add rotation and glow, then scale it down, then move it to the inventory panel
        Sequence sequence = DOTween.Sequence();
        sequence.Append(itemRectTransform.DOScale(1, 0.5f).SetEase(Ease.OutBack)) // Scale up the item
            .Join(itemRectTransform.DORotate(new Vector3(0, 0, 360), 0.5f, RotateMode.FastBeyond360))
            .Append(itemRectTransform.DOScale(0, 0.5f).SetEase(Ease.InBack)) // Scale down the item
            .Join(itemRectTransform.DORotate(new Vector3(0, 0, 360), 0.5f, RotateMode.FastBeyond360))
            .Append(itemRectTransform.DOMove(GetFirstEmptyInventorySlot().transform.position, 1f)) // Move the item to the inventory panel
            .OnComplete(() => {
                // After the animation is complete, add the item to the inventory
                AddItemToInventory(sliceItem, itemCopyImage);
                Destroy(itemCopy);
            });

        yield return sequence.WaitForCompletion();
    }

    private Image GetFirstEmptyInventorySlot()
    {
        foreach (var slot in inventorySlots)
        {
            if (slot.sprite == null) // Eğer bu slot boşsa
            {
                return slot;
            }
        }
        return null; // No empty slots found
    }

    private void AddItemToInventory(SliceItemSO sliceItem, Image itemCopyImage)
    {
        Image firstEmptySlot = GetFirstEmptyInventorySlot();
        if (firstEmptySlot != null)
        {
            firstEmptySlot.sprite = itemCopyImage.sprite; // Kazanılan item'ın görselini bu slota ekle
        }
    }
}
