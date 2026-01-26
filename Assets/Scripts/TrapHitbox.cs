using UnityEngine;

public class TrapHitbox : MonoBehaviour
{
    [SerializeField] private string playerTag = "Player";

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;

        ActionManager.Instance.Respawn(other.gameObject);
    }
}