using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WheelGame
{
    public enum WheelType
    {
        SilverWheel,
        BronzeWheel,
        GoldWheel
    }

    [CreateAssetMenu(fileName = "New Wheel", menuName = "Wheel Game/Wheel")]
    public class WheelSO : ScriptableObject
    {
        public Sprite wheelSprite;
        public int rotationDuration;
        public int minNumberOfRotations;
        public int maxNumberOfRotations;
        public SliceItemSO[] sliceItems;
        public WheelType wheelType;
        // Her çark için diğer özellikler buraya eklenebilir
    }
}
