using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    public static PlayerData Instance { get; private set; }

    public int Cash { get; private set; }
    public int Gold { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Set initial cash and gold amounts
            Cash = 200;
            Gold = 200;
        }
        else
        {
            if (Instance != this)
            {
                // If a different instance is already assigned, destroy this one
                Destroy(gameObject);
            }
        }
    }

    public void AddCash(int amount)
    {
        Cash += amount;
    }

    public void RemoveCash(int amount)
    {
        Cash = Mathf.Max(0, Cash - amount);
    }

    public void AddGold(int amount)
    {
        Gold += amount;
    }

    public void RemoveGold(int amount)
    {
        Gold = Mathf.Max(0, Gold - amount);
    }
}

