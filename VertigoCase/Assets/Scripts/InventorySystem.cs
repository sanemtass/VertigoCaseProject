using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventorySystem : MonoBehaviour
{
    public List<SliceItemSO> items = new List<SliceItemSO>();
    public static InventorySystem Instance { get; private set; }
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

        OnItemAdded?.Invoke(item);

        //Display the item in the center of the screen.
        yield return StartCoroutine(InventoryUIController.Instance.ShowItemInInventory(item));

        UIPanelController.Instance.SlidePanel();
    }

    public void ResetInventory()
    {
        items.Clear();
    }
}
