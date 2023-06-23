using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;

public class UIPanelController : MonoBehaviour
{
    public static UIPanelController Instance { get; private set; }

    public RectTransform panelTransform;
    public List<TextMeshProUGUI> zoneTexts_value = new List<TextMeshProUGUI>();
    public float moveAmount = 32.5f;
    public float slideDuration = 1f;

    public static event Action OnPanelSlided;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("More than one instance of UIPanelController!");
        }
        Instance = this;
    }

    void Start()
    {
        InitializeZones();
    }

    public void InitializeZones()
    {
        zoneTexts_value.Clear();

        for (int i = 1; i <= 60; i++)
        {
            GameObject newZone = new GameObject("Zone " + i);
            newZone.transform.SetParent(panelTransform);

            TextMeshProUGUI zoneText = newZone.AddComponent<TextMeshProUGUI>();

            // Set size
            RectTransform rectTransform = zoneText.GetComponent<RectTransform>();
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 35f);
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 45f);

            zoneText.text = i.ToString();
            zoneText.fontSize = 17;
            zoneText.alignment = TextAlignmentOptions.Center;
            rectTransform.localScale = Vector3.one;

            if (i % 5 == 0)
            {
                if (i == 30 || i == 60)
                {
                    zoneText.color = Color.blue;
                }
                else
                {
                    zoneText.color = Color.green;
                }
            }
            else
            {
                zoneText.color = Color.white;
            }

            zoneTexts_value.Add(zoneText);
        }
    }

    public void SlidePanel()
    {
        foreach (var textMeshPro in zoneTexts_value)
        {
            textMeshPro.rectTransform.DOMoveX(textMeshPro.rectTransform.position.x - moveAmount, slideDuration)
                .SetEase(Ease.InOutQuad);
        }

        TextMeshProUGUI firstZone = zoneTexts_value[0];
        zoneTexts_value.RemoveAt(0);
        zoneTexts_value.Add(firstZone);

        OnPanelSlided?.Invoke();
    }

}
