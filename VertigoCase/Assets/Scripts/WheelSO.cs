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
        public Sprite wheelSprite_value;
        public int rotationDuration_value;
        public int minNumberOfRotations_value;
        public int maxNumberOfRotations_value;
        public SliceItemSO[] sliceItems_value;
        public WheelType wheelType;
    }
}