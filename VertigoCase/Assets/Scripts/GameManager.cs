using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WheelGame;

public class GameManager : MonoBehaviour
{
    public WheelController wheelController;
    public IndicatorController indicatorController;
    public WheelSO silverWheel;
    public WheelSO bronzeWheel;
    private int currentZone = 0;

    private void Start()
    {
        // Event'e methodu ekliyoruz
        UIPanelController.OnPanelSlided += ZoneUp;
    }

    private void OnDestroy()
    {
        // Component yok edildiğinde eventten methodu çıkarıyoruz
        UIPanelController.OnPanelSlided -= ZoneUp;
    }

    private void ZoneUp()
    {
        currentZone++;

        // Çarkın görünümünü değiştiriyoruz
        if ((currentZone + 1 == 1) || ((currentZone + 1) % 5 == 0))
        {
            // Current zone is 1 or a multiple of 5, show silver wheel
            wheelController.ChangeWheel(silverWheel);
            indicatorController.ChangeIndicator(WheelType.SilverWheel);
        }
        else
        {
            // Current zone is not 1 and not a multiple of 5, show bronze wheel
            wheelController.ChangeWheel(bronzeWheel);
            indicatorController.ChangeIndicator(WheelType.BronzeWheel);
        }
    }
}

