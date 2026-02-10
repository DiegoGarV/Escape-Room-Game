using UnityEngine;

public class DrawerSlide : MonoBehaviour
{
    public enum SlideAxis { X, Z }

    [Header("Move Settings")]
    [SerializeField] private SlideAxis axis = SlideAxis.Z;
    [SerializeField] private float distance = 0.35f;   // cuánto se abre
    [SerializeField] private bool negativeDirection = false; // si debe ir hacia -X o -Z
    [SerializeField] private float speed = 6f;         // qué tan rápido se mueve

    private Vector3 _closedLocalPos;
    private Vector3 _openLocalPos;
    private bool _isOpen;

    void Awake()
    {
        _closedLocalPos = transform.localPosition;

        Vector3 dir = axis == SlideAxis.X ? Vector3.right : Vector3.forward;
        if (negativeDirection) dir = -dir;

        _openLocalPos = _closedLocalPos + dir * distance;
    }

    public void Toggle()
    {
        _isOpen = !_isOpen;
    }

    void Update()
    {
        Vector3 target = _isOpen ? _openLocalPos : _closedLocalPos;
        transform.localPosition = Vector3.Lerp(transform.localPosition, target, speed * Time.deltaTime);
    }
}
