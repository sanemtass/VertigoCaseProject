using DG.Tweening;
using UnityEngine;
using System;

namespace WheelGame
{
    public class WheelController : MonoBehaviour
    {
        public WheelSO wheel;
        public IndicatorController indicatorController; // Indicatörün referansını ekliyoruz.
        public SliceItemSO[] sliceItems; // Tekerlekteki dilimlerin içeriğini belirler

        // Olayı tanımlama
        public Action OnWheelRotationStarted;

        private void Start()
        {
            // Tekerlekteki dilimleri oluştur
            CreateSlices();
        }

        private void CreateSlices()
        {
            float radius = 105f; // Çarkın merkezinden itemin ne kadar uzakta olacağını belirler
            float angleStep = 360f / sliceItems.Length;
            float currentAngle = 0f;
            Debug.Log("Angle Step: " + angleStep); // This should print 45 if you have 8 slices
            Debug.Log("Number of slices: " + sliceItems.Length);

            foreach (var sliceItem in sliceItems)
            {
                Debug.Log("Current Angle: " + currentAngle);
                CreateSlice(sliceItem, currentAngle, radius);
                currentAngle += angleStep;
            }
        }

        private void CreateSlice(SliceItemSO sliceItem, float currentAngle, float radius)
        {
            // Dilimin içeriği için yeni bir GameObject oluştur
            GameObject newSlice = Instantiate(sliceItem.itemPrefab);
            newSlice.transform.SetParent(transform);

            // RectTransform'u kullanarak dilimi çarkın çevresine yerleştir
            RectTransform rectTransform = newSlice.GetComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.anchoredPosition = Vector2.zero;

            // Dilimin rotasyonunu ayarla
            newSlice.transform.localRotation = Quaternion.Euler(0f, 0f, currentAngle);

            // Dilimin konumunu ayarla
            float itemXPosition = radius * Mathf.Cos(currentAngle * Mathf.Deg2Rad);
            float itemYPosition = radius * Mathf.Sin(currentAngle * Mathf.Deg2Rad);
            newSlice.transform.localPosition = new Vector3(itemXPosition, itemYPosition, 0);
        }


        public void RotateWheel()
        {
            // Sağa doğru döndürmek için -360 kullanılıyor
            // Toplamda dönecek derece sayısı hesaplanıyor
            float totalDegreesToRotate = -360 * wheel.numberOfRotations;

            // Çarkın hızla başlaması ve yavaşça durması için OutCubic kullanılıyor
            transform.DORotate(new Vector3(0, 0, totalDegreesToRotate), wheel.rotationDuration, RotateMode.FastBeyond360)
                .SetEase(Ease.OutCubic)
                .SetUpdate(true)
                .OnStart(() =>
                {
                    // Dönüş başladığında indicatörün hareketini başlatıyoruz.
                    indicatorController.MoveIndicator();

                    // Olayı tetikle
                    OnWheelRotationStarted?.Invoke();
                })
                .OnComplete(() =>
                {
                    // Dönüş tamamlandığında indicatörün hareketini durduruyoruz.
                    indicatorController.ResetIndicator();
                });
        }
    }
}
