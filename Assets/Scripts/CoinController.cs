using UnityEngine;

public class CoinPickup : MonoBehaviour
{
    [Header("Animation")]
    [SerializeField] private float rotateSpeed = 120f;
    [SerializeField] private float floatAmplitude = 0.15f;
    [SerializeField] private float floatSpeed = 2f;
    [SerializeField] private float phaseOffset = 0f;

    [Header("Pickup")]
    [SerializeField] private string playerTag = "Player";

    private Vector3 _startPos;

    void Awake()
    {
        _startPos = transform.position;
        if (Mathf.Approximately(phaseOffset, 0f))
            phaseOffset = Random.Range(0f, 10f);
    }

    void Update()
    {
        transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime, Space.World);

        float y = Mathf.Sin((Time.time + phaseOffset) * floatSpeed) * floatAmplitude;
        transform.position = _startPos + new Vector3(0f, y, 0f);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayFirePickupSfx(transform.position);
        if (ProgressManager.Instance != null)
            ProgressManager.Instance.AddCoin(1);

        Destroy(gameObject);
    }
}
