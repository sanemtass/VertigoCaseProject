using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace WheelGame
{
    public class IndicatorController : MonoBehaviour
    {
        [SerializeField]
        private Transform _indicator; // indikatör objesine dair referans

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
