using UnityEngine;
using StarterAssets;

public class FootstepController : MonoBehaviour
{
    [SerializeField] private FirstPersonController fp;
    [SerializeField] private CharacterController cc;

    [Header("Step Timing")]
    [SerializeField] private float walkStepInterval = 0.5f;
    [SerializeField] private float runStepInterval = 0.35f;

    [Header("Thresholds")]
    [SerializeField] private float minMoveSpeed = 0.1f;

    private float _timer;

    void Reset()
    {
        fp = GetComponent<FirstPersonController>();
        cc = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (Time.timeScale == 0f) return;
        if (AudioManager.Instance == null) return;
        if (fp == null || cc == null) return;

        bool grounded = cc.isGrounded;

        Vector3 v = cc.velocity;
        v.y = 0f;
        float speed = v.magnitude;

        if (!grounded || speed < minMoveSpeed)
        {
            _timer = 0f;
            return;
        }

        float interval = speed > 3.5f ? runStepInterval : walkStepInterval;

        _timer += Time.deltaTime;
        if (_timer >= interval)
        {
            _timer = 0f;
            AudioManager.Instance.PlayFootstep();
        }
    }
}
