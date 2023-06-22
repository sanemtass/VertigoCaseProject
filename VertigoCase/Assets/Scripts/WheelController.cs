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
            RectTransform rectTransform = transform.parent.GetComponent<RectTransform>(); // Parent of Wheel and Indicator
            rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, -300f);

            // Animate position to the original
            rectTransform.DOAnchorPosY(0f, 0.5f).SetEase(Ease.OutBack);
        }

        private SliceItemSO[] CreateSlices()
        {
            float radius = 105f;
            float angleStep = 360f / 8;
            float currentAngle = 90f;

            List<SliceItemSO> sliceItemList = new List<SliceItemSO>(wheel.sliceItems);
            SliceItemSO[] createdSlices = new SliceItemSO[8];

            Dictionary<ItemType, int> itemMaxCount = new Dictionary<ItemType, int>
    {
        {ItemType.GoldItem, 2},
        {ItemType.CashItem, 2},
        {ItemType.ChestItems, 2},
        {ItemType.GrenadeItems, wheel.wheelType == WheelType.SilverWheel ? 0 : 1},
        {ItemType.HealthShotItems, 2},
        {ItemType.TierItems, 2},
        {ItemType.PointItems, 2},
        {ItemType.OtherItems, 2}
    };

            Dictionary<ItemType, int> itemTypeCounter = new Dictionary<ItemType, int>
    {
        {ItemType.GoldItem, 0},
        {ItemType.CashItem, 0},
        {ItemType.ChestItems, 0},
        {ItemType.GrenadeItems, 0},
        {ItemType.HealthShotItems, 0},
        {ItemType.TierItems, 0},
        {ItemType.PointItems, 0},
        {ItemType.OtherItems, 0}
    };

            // Force grenade item for bronze wheel
            if (wheel.wheelType == WheelType.BronzeWheel)
            {
                SliceItemSO grenadeSlice = sliceItemList.Find(item => item.itemType == ItemType.GrenadeItems);
                if (grenadeSlice != null)
                {
                    CreateSlice(grenadeSlice, currentAngle, radius);
                    currentAngle += angleStep;
                    itemTypeCounter[ItemType.GrenadeItems]++;
                    sliceItemList.Remove(grenadeSlice);
                    createdSlices[0] = grenadeSlice;
                }
                else
                {
                    Debug.LogError("Grenade item not found for Bronze Wheel");
                    return createdSlices;
                }
            }

            for (int i = itemTypeCounter[ItemType.GrenadeItems]; i < 8; i++)
            {
                SliceItemSO sliceItem;
                int randomIndex;

                do
                {
                    if (sliceItemList.Count == 0)
                    {
                        Debug.LogError("Not enough slice items available for this wheel type.");
                        return createdSlices;
                    }

                    randomIndex = UnityEngine.Random.Range(0, sliceItemList.Count);
                    sliceItem = sliceItemList[randomIndex];

                    if (itemTypeCounter[sliceItem.itemType] < itemMaxCount[sliceItem.itemType])
                    {
                        itemTypeCounter[sliceItem.itemType]++;
                        break;
                    }
                    else
                    {
                        sliceItemList.RemoveAt(randomIndex);
                    }
                } while (true);

                CreateSlice(sliceItem, currentAngle, radius);
                currentAngle += angleStep;

                createdSlices[i] = sliceItem;
            }

            return createdSlices;
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
            if (isRotating)
                return;

            int numberOfRotations = UnityEngine.Random.Range(wheel.minNumberOfRotations, wheel.maxNumberOfRotations);
            int sliceToStopOn = UnityEngine.Random.Range(0, sliceItems.Length);
            float totalDegreesToRotate = -360f * numberOfRotations - (360f / 8) * sliceToStopOn;

            Debug.Log("Stopping on slice: " + sliceToStopOn);

            transform.DORotate(new Vector3(0, 0, totalDegreesToRotate), wheel.rotationDuration, RotateMode.FastBeyond360)
                .SetEase(Ease.OutCubic)
                .SetUpdate(true)
                .OnStart(() =>
                {
                    indicatorController.MoveIndicator();
                    OnWheelRotationStarted?.Invoke();
                    isRotating = true;
                })
                .OnComplete(() =>
                {
                    indicatorController.ResetIndicator();

            // Check the item type before showing itemDescription and adding it to the inventory
            if (sliceItems[sliceToStopOn].itemType != ItemType.GrenadeItems)
                    {
                        string itemDescription = "Up to x" + sliceItems[sliceToStopOn].itemQuantity + " " + sliceItems[sliceToStopOn].baseItemName;
                        UIManager.Instance.ShowGainedItemText(itemDescription);

                        StartCoroutine(InventorySystem.Instance.AddItem(sliceItems[sliceToStopOn]));
                    }

            // If the slice is a grenade, show the grenade panel
            if (sliceItems[sliceToStopOn].itemType == ItemType.GrenadeItems)
                    {
                        UIManager.Instance.ShowBombPanel();
                    }

                    isRotating = false;
                });
        }
    }
}