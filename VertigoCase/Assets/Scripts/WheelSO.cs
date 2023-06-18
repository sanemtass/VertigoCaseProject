using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WheelGame
{
    [CreateAssetMenu(fileName = "New Wheel", menuName = "Wheel Game/Wheel")]
    public class WheelSO : ScriptableObject
    {
        public float rotationDuration;
        public float numberOfRotations;
        // Her çark için diğer özellikler buraya eklenebilir
    }
}
