using UnityEngine;

public class SwingTrap : MonoBehaviour
{
    public float amplitudeDegrees = 45f;
    public float speed = 1.2f;
    public float phaseOffset = 0f;
    public Vector3 localAxis = Vector3.forward;

    Quaternion _startRot;

    void Awake()
    {
        _startRot = transform.localRotation;
    }

    void Update()
    {
        float angle = Mathf.Sin((Time.time + phaseOffset) * speed) * amplitudeDegrees;
        transform.localRotation = _startRot * Quaternion.AngleAxis(angle, localAxis.normalized);
    }
}
