using UnityEngine;

public class DoorController : MonoBehaviour
{
    [Header("Open Settings")]
    [SerializeField] private float openAngleY = -90f;
    [SerializeField] private float openSpeed = 180f;

    [Header("Coins")]
    [SerializeField] private int coinsRequired = 5;

    private Quaternion _closedRot;
    private Quaternion _openRot;
    private bool _isOpen;

    void Awake()
    {
        _closedRot = transform.localRotation;
        _openRot = _closedRot * Quaternion.Euler(0f, openAngleY, 0f);
    }

    void Update()
    {
        if (!_isOpen && CoinCounter.Instance != null && CoinCounter.Instance.Coins >= coinsRequired)
        {
            Open();
        }

        if (!_isOpen) return;

        transform.localRotation = Quaternion.RotateTowards(
            transform.localRotation,
            _openRot,
            openSpeed * Time.deltaTime
        );
    }

    public void Open()
    {
        _isOpen = true;
    }
}
