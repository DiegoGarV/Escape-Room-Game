using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class EndGameTrigger : MonoBehaviour
{
    [SerializeField] private GameObject winCanvas;
    [SerializeField] private PlayerInput playerInput;

    private bool _done;

    void Start()
    {
        if (winCanvas != null) winCanvas.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (_done) return;
        if (!other.CompareTag("Player")) return;

        _done = true;
        StartCoroutine(WinSequence());
    }

    private IEnumerator WinSequence()
    {
        // 1) Mostrar UI antes de pausar
        if (winCanvas != null)
        {
            Debug.Log("AAAAAAAAAAAAAAAAA");
            winCanvas.SetActive(true);

            // Si hay CanvasGroup, asegúrate de verlo
            var cg = winCanvas.GetComponentInChildren<CanvasGroup>(true);
            if (cg != null) cg.alpha = 1f;
        }

        // 2) Soltar cursor ya
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // 3) Esperar 1 frame (para que la UI se dibuje)
        yield return null;

        // 4) Bloquear input del jugador
        if (playerInput != null) playerInput.enabled = false;

        // 5) Ahora sí, pausar
        Time.timeScale = 0f;
    }
}
