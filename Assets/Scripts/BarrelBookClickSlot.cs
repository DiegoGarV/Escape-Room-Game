using UnityEngine;

public class BarrelBookClickSlot : MonoBehaviour
{
    [Header("Slot")]
    public BookColor requiredColor = BookColor.Red;
    public Transform bookPlace;                 // empty hijo "bookPlace"
    public PuzzleBooksManager puzzleManager;    // manager del puzzle

    [Header("Placement")]
    public Vector3 placedLocalEuler = Vector3.zero;
    public bool disableBookColliders = true;

    private bool _filled;

    /// <summary>
    /// Intenta colocar el libro que el jugador tiene en la mano.
    /// Devuelve true si lo colocó (para que el ActionManager NO lo suelte).
    /// </summary>
    public bool TryPlaceHeldBook(ActionManager actions)
    {
        if (_filled) return false;
        if (bookPlace == null) return false;
        if (actions == null) return false;

        Transform held = actions.GetHeldTransform();
        if (held == null) return false;

        // Debe ser un libro
        BookItem book = held.GetComponentInParent<BookItem>();
        if (book == null) return false;

        // Validación de color
        if (book.isPlaced) return false;
        if (book.color != requiredColor) return false;

        // Marcar y "sacar" de la mano sin dropear
        book.isPlaced = true;
        actions.ClearHeldWithoutDropping();

        // Apagar física
        var rb = held.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        if (disableBookColliders)
        {
            foreach (var c in held.GetComponentsInChildren<Collider>(true))
                c.enabled = false;
        }

        // Encajar
        held.SetParent(bookPlace, worldPositionStays: false);
        held.localPosition = Vector3.zero;
        held.localRotation = Quaternion.Euler(placedLocalEuler);

        _filled = true;

        if (puzzleManager != null)
            puzzleManager.OnBookPlaced();

        return true;
    }
}
