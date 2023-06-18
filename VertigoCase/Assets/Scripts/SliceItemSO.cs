using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "WheelGame/Item", order = 1)]
public class SliceItemSO : ScriptableObject
{
    public GameObject itemPrefab; // Bu dilimin içerdiği objenin prefab'ı
    // Daha fazla özellikler buraya eklenebilir.
}
