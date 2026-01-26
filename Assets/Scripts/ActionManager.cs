using UnityEngine;
using UnityEngine.InputSystem;

public class ActionManager : MonoBehaviour
{
    public static ActionManager Instance { get; private set; }

    [Header("Respawn Settings")]
    [SerializeField] private Transform respawnPoint;

    [Header("Pick/Drop Settings")]
    [SerializeField] Transform ObjectAnchor;
    [SerializeField] private LayerMask collectableMask;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float collectDistance = 3f;
    [SerializeField] private float dropForwardDistance = 1.2f;
    [SerializeField] private float dropRayUp = 1.0f;

    [Header("Interact")]
    [SerializeField] private float interactDistance = 3f;
    [SerializeField] private LayerMask interactMask;


    [Header("Tags")]
    [SerializeField] private string collectableTag = "Collectable";
    [SerializeField] private string doorTag = "Door";
    
    [SerializeField] private string barrelTag = "Barrel";

    private Transform heldObject;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void Respawn(GameObject player)
    {
        var cc = player.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;

        player.transform.SetPositionAndRotation(respawnPoint.position, respawnPoint.rotation);

        if (cc != null) cc.enabled = true;
    }

    void Update()
    {
        if (Mouse.current == null) return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            var cam = Camera.main;
            if (cam == null) return;

            RaycastHit hit;
            
            if (heldObject != null)
            {
                // Cuando tengo un objeto en la mano, reviso qué estoy clickeando
                var hits = Physics.RaycastAll(
                    cam.transform.position,
                    cam.transform.forward,
                    interactDistance,
                    ~0, // todo
                    QueryTriggerInteraction.Ignore
                );

                // Ordena por distancia (lo más cercano primero)
                System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

                // 1) Prioridad: puerta (para no romper la mecánica de la llave)
                foreach (var h in hits)
                {
                    if (h.collider != null && h.collider.CompareTag(doorTag))
                    {
                        var door = h.collider.GetComponentInParent<DoorController>();
                        if (door != null) door.Open();

                        Destroy(heldObject.gameObject);
                        heldObject = null;
                        return;
                    }
                }

                // 2) Luego: barril (colocar libro si aplica)
                foreach (var h in hits)
                {
                    if (h.collider != null && h.collider.CompareTag(barrelTag))
                    {
                        var slot = h.collider.GetComponentInParent<BarrelBookClickSlot>();
                        if (slot != null)
                        {
                            // Si lo colocó, NO dropear
                            if (slot.TryPlaceHeldBook(this))
                                return;
                        }
                    }
                }

                // 3) Si no fue puerta ni se colocó en barril -> dropear
                DropHeldObject(cam);
                return;

            }

            if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, collectDistance, collectableMask, QueryTriggerInteraction.Ignore))
            {
                if (hit.collider != null && hit.collider.CompareTag(collectableTag))
                {
                    PickUpObject(hit.collider.transform);
                }
            }
            
        }
    }

    private void SetHeldCollidersEnabled(Transform t, bool enabled)
    {
        var cols = t.GetComponentsInChildren<Collider>(true);
        foreach (var c in cols) c.enabled = enabled;
    }

    private void PickUpObject(Transform obj)
    {
        heldObject = obj;
        
        var rb = heldObject.GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;

        SetHeldCollidersEnabled(heldObject, false);

        heldObject.SetParent(ObjectAnchor, worldPositionStays: false);
        heldObject.localPosition = Vector3.zero;
        heldObject.localRotation = Quaternion.Euler(0f, 90f, -90f);
    }

    private void DropHeldObject(Camera cam)
    {
        Vector3 forwardFlat = new Vector3(cam.transform.forward.x, 0f, cam.transform.forward.z).normalized;
        Vector3 dropStart = cam.transform.position + forwardFlat * dropForwardDistance + Vector3.up * dropRayUp;

        Vector3 dropPos = dropStart;
        if (Physics.Raycast(dropStart, Vector3.down, out RaycastHit groundHit, 5f, groundMask, QueryTriggerInteraction.Ignore))
        {
            dropPos = groundHit.point;
        }

        heldObject.SetParent(null, worldPositionStays: true);
        heldObject.position = dropPos;
        heldObject.rotation = Quaternion.identity;

        SetHeldCollidersEnabled(heldObject, true);

        var rb = heldObject.GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = false;

        heldObject = null;
    }

    public Transform GetHeldTransform()
    {
        return heldObject;
    }

    // Esto suelta el held del anchor SIN tirarlo al piso
    public void ClearHeldWithoutDropping()
    {
        heldObject = null;
    }

}
