using UnityEngine;

public class CoinPickup : MonoBehaviour
{
    [Header("Animation")]
    [SerializeField] private float rotateSpeed = 120f; // grados/seg
    [SerializeField] private float floatAmplitude = 0.15f; // altura
    [SerializeField] private float floatSpeed = 2f; // frecuencia
    [SerializeField] private float phaseOffset = 0f;

    [Header("Pickup")]
    [SerializeField] private string playerTag = "Player";

    private Vector3 _startPos;

    void Awake()
    {
        _startPos = transform.position;
        // Para que no todas leviten igual si clonas
        if (Mathf.Approximately(phaseOffset, 0f))
            phaseOffset = Random.Range(0f, 10f);
    }

    void Update()
    {
        // Giro (puedes cambiar Vector3.up por Vector3.forward según tu mesh)
        transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime, Space.World);

        // Levitación
        float y = Mathf.Sin((Time.time + phaseOffset) * floatSpeed) * floatAmplitude;
        transform.position = _startPos + new Vector3(0f, y, 0f);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;

        if (CoinCounter.Instance != null)
            CoinCounter.Instance.AddCoin(1);

        Destroy(gameObject);
    }
}
