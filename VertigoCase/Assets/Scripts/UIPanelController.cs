using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UIPanelController : MonoBehaviour
{
    public static UIPanelController Instance { get; private set; }

    public RectTransform panelTransform; // Sola kayacak olan panel
    public float moveAmount = 200f; // Panelin kayacağı miktar
    public float slideDuration = 1f; // Kayma süresi

    public static event Action OnPanelSlided;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("More than one instance of UIPanelController!");
        }
        Instance = this;
    }

    public void SlidePanel()
    {
        // Kayma işlemini başlat
        panelTransform.DOMoveX(panelTransform.position.x - moveAmount, slideDuration)
            .SetEase(Ease.InOutQuad)
            .OnComplete(() =>
            {
                // Panel kaydırıldığında olayı tetikle
                OnPanelSlided?.Invoke();
            });
    }

}
