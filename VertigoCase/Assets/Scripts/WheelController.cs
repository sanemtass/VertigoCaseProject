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
        public WheelSO wheel_value;
        public IndicatorController indicatorController_value;

        public Image wheelImage_value;
        // Olayı tanımlama
        public Action OnWheelRotationStarted;

        private SliceItemSO[] sliceItems_value; //To determine the content of the slices on the wheel.
        public bool isRotating = false;

        private void Start()
        {
            //Create the slices on the wheel
            ChangeWheel(wheel_value);
        }

        public void ChangeWheel(WheelSO newWheel)
        {
            wheel_value = newWheel;

            if (wheelImage_value == null)
            {
                Debug.LogError("Wheel image is not set");
                return;
            }

            //Update the wheel sprite
            wheelImage_value.sprite = newWheel.wheelSprite_value;

            //Clear existing slices
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }

            //Update sliceItems with the new wheel's sliceItems
            sliceItems_value = CreateSlices();

            RectTransform rectTransform = transform.parent.GetComponent<RectTransform>(); //Parent of Wheel and Indicator
            rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, -300f);

            //Animate position to the original
            rectTransform.DOAnchorPosY(0f, 0.5f).SetEase(Ease.OutBack);

            if (newWheel.wheelType == WheelType.SilverWheel || newWheel.wheelType == WheelType.GoldWheel)
            {
                GameManager.Instance.isSafeZone = true;
            }
            else
            {
                GameManager.Instance.isSafeZone = false;
            }
        }

        private SliceItemSO[] CreateSlices()
        {
            float radius = 105f;
            float angleStep = 360f / 8;
            float currentAngle = 90f;

            List<SliceItemSO> sliceItemList = new List<SliceItemSO>(wheel_value.sliceItems_value);
            SliceItemSO[] createdSlices = new SliceItemSO[8];

            Dictionary<ItemType, int> itemMaxCount = new Dictionary<ItemType, int>
    {
        {ItemType.GoldItem, 2},
        {ItemType.CashItem, 2},
        {ItemType.ChestItems, 2},
        {ItemType.GrenadeItems, wheel_value.wheelType == WheelType.SilverWheel ? 0 : 1},
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

            //Force grenade item for bronze wheel
            if (wheel_value.wheelType == WheelType.BronzeWheel)
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
            GameObject newSlice = Instantiate(sliceItem.itemPrefab_value);
            newSlice.transform.SetParent(transform);

            //Place the slice on the circumference of the wheel using RectTransform.
            RectTransform rectTransform = newSlice.GetComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.anchoredPosition = Vector2.zero;

            float sliceRotation = 90 - currentAngle;
            newSlice.transform.localRotation = Quaternion.Euler(0f, 0f, -sliceRotation);

            //Set the position of the slice.
            float itemXPosition = radius * Mathf.Cos(currentAngle * Mathf.Deg2Rad);
            float itemYPosition = radius * Mathf.Sin(currentAngle * Mathf.Deg2Rad);
            newSlice.transform.localPosition = new Vector3(itemXPosition, itemYPosition, 0);

            Transform quantityTextObject = newSlice.transform.Find(sliceItem.quantityTextObjectName_value);
            if (quantityTextObject != null)
            {
                TextMeshProUGUI textComponent = quantityTextObject.GetComponent<TextMeshProUGUI>();
                if (textComponent != null)
                {
                    textComponent.text = "x" + sliceItem.itemQuantity_value;
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

        }

        public void RotateWheel()
        {
            if (isRotating)
                return;

            int numberOfRotations = UnityEngine.Random.Range(wheel_value.minNumberOfRotations_value, wheel_value.maxNumberOfRotations_value);
            int sliceToStopOn = UnityEngine.Random.Range(0, sliceItems_value.Length);
            float totalDegreesToRotate = -360f * numberOfRotations - (360f / 8) * sliceToStopOn;

            Debug.Log("Stopping on slice: " + sliceToStopOn);

            transform.DORotate(new Vector3(0, 0, totalDegreesToRotate), wheel_value.rotationDuration_value, RotateMode.FastBeyond360)
                .SetEase(Ease.OutCubic)
                .SetUpdate(true)
                .OnStart(() =>
                {
                    indicatorController_value.MoveIndicator();
                    OnWheelRotationStarted?.Invoke();
                    isRotating = true;
                })
                .OnComplete(() =>
                {
                    indicatorController_value.ResetIndicator();

            if (sliceItems_value[sliceToStopOn].itemType != ItemType.GrenadeItems)
                    {
                        string itemDescription = "Up to x" + sliceItems_value[sliceToStopOn].itemQuantity_value + " " + sliceItems_value[sliceToStopOn].baseItemName_value;
                        UIManager.Instance.ShowGainedItemText(itemDescription);

                        StartCoroutine(InventorySystem.Instance.AddItem(sliceItems_value[sliceToStopOn]));
                    }

            //If the slice is a grenade, show the grenade panel
            if (sliceItems_value[sliceToStopOn].itemType == ItemType.GrenadeItems)
                    {
                        UIManager.Instance.ShowBombPanel();
                    }

                    isRotating = false;
                });
        }
    }
}