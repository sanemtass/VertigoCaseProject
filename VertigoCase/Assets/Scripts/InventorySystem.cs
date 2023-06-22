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

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            if (Instance != this)
            {
                // If a different instance is already assigned, destroy this one
                Destroy(gameObject);
            }
        }
    }

    public IEnumerator AddItem(SliceItemSO item)
    {
        if (item.itemType != ItemType.GrenadeItems)
        {
            items.Add(item);
        }

        // Olayı tetikle
        OnItemAdded?.Invoke(item);

        // Item'ı ekranın ortasında göster
        yield return StartCoroutine(InventoryUIController.Instance.ShowItemInInventory(item));

        // Burada paneli kaydır
        UIPanelController.Instance.SlidePanel();
    }

    public void ResetInventory()
    {
        items.Clear();
    }
}
