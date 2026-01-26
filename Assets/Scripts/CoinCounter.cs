using UnityEngine;

public class CoinCounter : MonoBehaviour
{
    public static CoinCounter Instance { get; private set; }

    public int Coins { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void AddCoin(int amount = 1)
    {
        Coins += amount;
    }
}