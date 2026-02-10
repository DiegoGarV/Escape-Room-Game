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
        if (!_isOpen && ProgressManager.Instance != null && ProgressManager.Instance.Coins >= coinsRequired)
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
        if (_isOpen) return;
        _isOpen = true;

        var info = GetComponent<DoorAudioInfo>();
        var type = info != null ? info.type : DoorType.Wood;

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayDoorOpenSfx3D(type, transform.position);
    }
}
