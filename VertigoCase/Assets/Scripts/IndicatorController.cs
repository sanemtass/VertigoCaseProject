using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace WheelGame
{
    public class IndicatorController : MonoBehaviour
    {
        [SerializeField]
        private Transform _indicator;

        public Image indicatorImage; // Image component of the indicator
        public Sprite silverSprite; // Silver indicator sprite
        public Sprite bronzeSprite; // Bronze indicator sprite

        public void ChangeIndicator(WheelType wheelType)
        {
            // Change the sprite based on the wheel type
            switch (wheelType)
            {
                case WheelType.SilverWheel:
                    indicatorImage.sprite = silverSprite;
                    break;
                case WheelType.BronzeWheel:
                    indicatorImage.sprite = bronzeSprite;
                    break;
            }
        }

        public void MoveIndicator()
        {
            Debug.Log("MoveIndicator called."); // Bu log mesajını Console'da görmeniz gerekiyor

            // Çark dönerken indicatörün her bir segmentte 'takılmasını' simüle eder
            _indicator.DOLocalRotate(new Vector3(0f, 0f, 30f), .2f)
                .SetEase(Ease.Linear) // Ease değeri OutCubic olarak ayarlandı
                .SetLoops(-1, LoopType.Yoyo);
        }

        public void ResetIndicator()
        {
            // Çark durduğunda tüm indicatör animasyonlarını durdurur
            _indicator.DOKill();
        }
    }
}
