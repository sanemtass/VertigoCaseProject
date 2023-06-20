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
    public string itemName;
    public Sprite itemIcon;
    public GameObject itemPrefab; // Bu dilimin içerdiği objenin prefab'ı
    public ItemType itemType;
    public int itemQuantity;
    public string quantityTextObjectName;
    //public int index;
    // Daha fazla özellikler buraya eklenebilir.
}

