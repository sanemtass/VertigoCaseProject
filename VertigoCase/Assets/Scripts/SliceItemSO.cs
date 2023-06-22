using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    GoldItem,
    CashItem,
    ChestItems,
    GrenadeItems,
    HealthShotItems,
    TierItems,
    PointItems,
    OtherItems
}

[CreateAssetMenu(fileName = "Item", menuName = "WheelGame/Item", order = 1)]
public class SliceItemSO : ScriptableObject
{
    public string baseItemName; // Base item name like "Glasses"
    public Sprite itemIcon;
    public GameObject itemPrefab;
    public ItemType itemType;
    public int itemQuantity;
    public string quantityTextObjectName;

    public string ItemName
    {
        get
        {
            return $"Up To x{itemQuantity} {baseItemName}";
        }
    }
}

