using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventorySystem : MonoBehaviour
{
    public List<SliceItemSO> items = new List<SliceItemSO>();
    public static InventorySystem Instance { get; private set; }

    // Olayı tanımlama
    public Action<SliceItemSO> OnItemAdded;

    public InventorySystem()
    {
        if (Instance != null)
        {
            Debug.LogError("More than one instance of Inventory System!");
        }
        Instance = this;
    }

    public IEnumerator AddItem(SliceItemSO item)
    {
        items.Add(item);

        // Olayı tetikle
        OnItemAdded?.Invoke(item);

        // Item'ı ekranın ortasında göster
        yield return StartCoroutine(InventoryUIController.Instance.ShowItemInInventory(item));

        // Burada paneli kaydır
        UIPanelController.Instance.SlidePanel();
    }

}
