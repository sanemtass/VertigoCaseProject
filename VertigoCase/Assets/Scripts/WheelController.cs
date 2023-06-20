using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace WheelGame
{
    public class WheelController : MonoBehaviour
    {
        public WheelSO wheel;
        public IndicatorController indicatorController; // Indicatörün referansını ekliyoruz.
        
        public Image wheelImage;
        // Olayı tanımlama
        public Action OnWheelRotationStarted;

        private SliceItemSO[] sliceItems; // Tekerlekteki dilimlerin içeriğini belirler
        private bool isRotating = false;

        private void Start()
        {
            // Tekerlekteki dilimleri oluştur
            ChangeWheel(wheel);
        }

        public void ChangeWheel(WheelSO newWheel)
        {
            wheel = newWheel;

            if (wheelImage == null)
            {
                Debug.LogError("Wheel image is not set");
                return;
            }

            // Update the wheel sprite
            wheelImage.sprite = newWheel.wheelSprite;

            // Clear existing slices
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }

            // Update sliceItems with the new wheel's sliceItems
            sliceItems = CreateSlices();

            // Add animation
            // Set initial position
            RectTransform rectTransform = GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, -300f);

            // Animate position to the original
            rectTransform.DOAnchorPosY(0f, 0.5f).SetEase(Ease.OutBack);
        }

        private SliceItemSO[] CreateSlices()
        {
            float radius = 105f;
            float angleStep = 360f / 8; // 8 dilim olduğunu belirttik.
            float currentAngle = 90f;

            List<SliceItemSO> sliceItemList = new List<SliceItemSO>(wheel.sliceItems); // Create a copy of sliceItems
            SliceItemSO[] createdSlices = new SliceItemSO[8]; // An array to hold the created slices

            // counters for different item types
            int goldCashCount = 0;
            int grenadeCount = 0;

            for (int i = 0; i < 8; i++) // 8 dilim olduğunu belirttik.
            {
                SliceItemSO sliceItem;
                int randomIndex;

                do
                {
                    randomIndex = UnityEngine.Random.Range(0, sliceItemList.Count); // rastgele bir index seçeriz
                    sliceItem = sliceItemList[randomIndex];
                    // Control for WheelType and ItemType
                    if (wheel.wheelType == WheelType.SilverWheel && (sliceItem.itemType == ItemType.CashItem || sliceItem.itemType == ItemType.GoldItem))
                    {
                        if (goldCashCount >= 2)
                            sliceItemList.RemoveAt(randomIndex);
                        else
                            goldCashCount++;
                    }
                    else if (wheel.wheelType == WheelType.BronzeWheel && sliceItem.itemType == ItemType.GrenadeItems)
                    {
                        if (grenadeCount >= 1)
                            sliceItemList.RemoveAt(randomIndex);
                        else
                            grenadeCount++;
                    }
                } while (sliceItemList.Count > 0 && !(goldCashCount >= 2 || grenadeCount >= 1));


                if (sliceItemList.Count == 0)
                {
                    Debug.LogError("Not enough slice items available for this wheel type.");
                    break;
                }

                CreateSlice(sliceItem, currentAngle, radius);
                currentAngle += angleStep;

                createdSlices[i] = sliceItem; // Store the created slice
            }

            return createdSlices; // Return the created slices
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

            float sliceRotation = 90 - currentAngle; // or any other calculation you find suitable
            newSlice.transform.localRotation = Quaternion.Euler(0f, 0f, -sliceRotation);

            // Dilimin konumunu ayarla
            float itemXPosition = radius * Mathf.Cos(currentAngle * Mathf.Deg2Rad);
            float itemYPosition = radius * Mathf.Sin(currentAngle * Mathf.Deg2Rad);
            newSlice.transform.localPosition = new Vector3(itemXPosition, itemYPosition, 0);

            Transform quantityTextObject = newSlice.transform.Find(sliceItem.quantityTextObjectName);
            if (quantityTextObject != null)
            {
                TextMeshProUGUI textComponent = quantityTextObject.GetComponent<TextMeshProUGUI>();
                if (textComponent != null)
                {
                    textComponent.text = "x" + sliceItem.itemQuantity;
                }
                else
                {
                    Debug.LogError("No TextMeshPro component found on the quantityTextObject");
                }
            }
            else
            {
                Debug.LogError("No quantityTextObject found in the newSlice prefab");
            }

            //// Get the TMPro component from the prefab and set the text
            //var textComponent = newSlice.transform.Find(sliceItem.quantityTextObjectName).GetComponent<TMPro.TextMeshPro>();
            //textComponent.text = "x" + sliceItem.itemQuantity;
        }

        public void RotateWheel()
        {
            if (isRotating) // Çark zaten dönerken yeni bir döndürme işlemi başlatmamalıyız
                return;

            // Sağa doğru döndürmek için -360 kullanılıyor
            // Toplamda dönecek derece sayısı rastgele belirleniyor
            int numberOfRotations = UnityEngine.Random.Range(wheel.minNumberOfRotations, wheel.maxNumberOfRotations);
            int sliceToStopOn = UnityEngine.Random.Range(0, sliceItems.Length); // Choose a random slice to stop on
            float totalDegreesToRotate = -360f * numberOfRotations - (360f / 8) * sliceToStopOn; // Use 8 instead of sliceItems.Length

            Debug.Log("Stopping on slice: " + sliceToStopOn); // log the slice number we are stopping on

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
                    isRotating = true; // Döndürme işlemi başladığında isRotating'i true yap
                })
                .OnComplete(() =>
                {
                    // Dönüş tamamlandığında indicatörün hareketini durduruyoruz.
                    indicatorController.ResetIndicator();

                    StartCoroutine(InventorySystem.Instance.AddItem(sliceItems[sliceToStopOn]));
                    isRotating = false;
                });
        }

    }
}