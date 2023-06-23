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
    public string baseItemName_value;
    public Sprite itemIcon_value;
    public GameObject itemPrefab_value;
    public ItemType itemType;
    public int itemQuantity_value;
    public string quantityTextObjectName_value;

    public string ItemName
    {
        get
        {
            return $"Up To x{itemQuantity_value} {baseItemName_value}";
        }
    }
}