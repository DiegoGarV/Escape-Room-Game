using UnityEngine;
using System;

public class ProgressManager : MonoBehaviour
{
    public static ProgressManager Instance { get; private set; }

    public int Coins { get; private set; }

    public event Action<int> OnCoinsChanged;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void AddCoin(int amount = 1)
    {
        Coins += amount;
        OnCoinsChanged?.Invoke(Coins);
    }
}