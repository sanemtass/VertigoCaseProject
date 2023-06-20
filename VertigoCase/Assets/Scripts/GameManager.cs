using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WheelGame;

public class GameManager : MonoBehaviour
{
    public WheelController wheelController;
    public WheelSO silverWheel;
    public WheelSO bronzeWheel;
    private int currentZone;

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
        if (currentZone % 5 == 0)
        {
            // Current zone is a multiple of 5, show silver wheel
            wheelController.ChangeWheel(silverWheel);
        }
        else
        {
            // Current zone is not a multiple of 5, show bronze wheel
            wheelController.ChangeWheel(bronzeWheel);
        }
    }
}

