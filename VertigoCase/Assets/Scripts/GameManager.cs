using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WheelGame;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public WheelController wheelController;
    public IndicatorController indicatorController;
    public WheelSO silverWheel;
    public WheelSO bronzeWheel;
    public WheelSO goldWheel;
    public bool isSafeZone = false;

    private int currentZone = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        UIPanelController.OnPanelSlided += ZoneUp;
    }

    private void OnDestroy()
    {
        UIPanelController.OnPanelSlided -= ZoneUp;
    }

    private void ZoneUp()
    {
        currentZone++;

        if ((currentZone + 1) % 30 == 0) //Check if it's super zone
        {
            wheelController.ChangeWheel(goldWheel);
            indicatorController.ChangeIndicator(WheelType.GoldWheel);
        }
        else if ((currentZone + 1 == 1) || ((currentZone + 1) % 5 == 0))
        {
            wheelController.ChangeWheel(silverWheel);
            indicatorController.ChangeIndicator(WheelType.SilverWheel);
        }
        else
        {
            wheelController.ChangeWheel(bronzeWheel);
            indicatorController.ChangeIndicator(WheelType.BronzeWheel);
        }
    }

}

