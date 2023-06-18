using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WheelGame;

public class UIManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private ButtonHandler buttonHandler;
    [SerializeField] private WheelController wheelController;
    [SerializeField] private IndicatorController indicatorController; // indicatorController'ı ekleyin.

    public ButtonHandler ButtonHandler { get => buttonHandler; set => buttonHandler = value; }
    public WheelController WheelController { get => wheelController; set => wheelController = value; }
    public IndicatorController IndicatorController { get => indicatorController; set => indicatorController = value; } // Property'yi ekleyin.

    private void Start()
    {
        // Butonun tıklanma işlemine bir listener ekliyoruz
        ButtonHandler.OnButtonClicked += HandleButtonClick;
    }

    private void OnDestroy()
    {
        // Listener'ı kaldırıyoruz. Bu önemli çünkü listener'ı kaldırmazsak, bu nesne yok edildiğinde hala bir referans olacağı için bellek sızıntısına neden olabilir.
        ButtonHandler.OnButtonClicked -= HandleButtonClick;
    }

    private void HandleButtonClick()
    {
        // Butona tıklanıldığında çarkı döndür
        WheelController.RotateWheel();
    }
}
