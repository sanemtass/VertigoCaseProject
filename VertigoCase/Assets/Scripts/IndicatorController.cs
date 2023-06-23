using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace WheelGame
{
    public class IndicatorController : MonoBehaviour
    {
        [SerializeField]
        private Transform indicator_value;

        public Image indicatorImage_value;
        public Sprite silverSprite_value;
        public Sprite bronzeSprite_value;
        public Sprite goldSprite_value;

        public void ChangeIndicator(WheelType wheelType)
        {
            //Change the sprite based on the wheel type
            switch (wheelType)
            {
                case WheelType.SilverWheel:
                    indicatorImage_value.sprite = silverSprite_value;
                    break;
                case WheelType.BronzeWheel:
                    indicatorImage_value.sprite = bronzeSprite_value;
                    break;
                case WheelType.GoldWheel:
                    indicatorImage_value.sprite = goldSprite_value;
                    break;
            }
        }

        public void MoveIndicator()
        {
            indicator_value.DOLocalRotate(new Vector3(0f, 0f, 30f), .2f)
                .SetEase(Ease.Linear)
                .SetLoops(-1, LoopType.Yoyo);
        }

        public void ResetIndicator()
        {
            indicator_value.DOKill();
        }
    }
}
