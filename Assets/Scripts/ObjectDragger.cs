using UnityEngine;

public class ObjectDragger : MonoBehaviour
{
    public KeyCode dragKey = KeyCode.E;
    public Vector3 holdOffset = new Vector3(0, 1.5f, 2f);
    public float pickupRadius = 0.5f;
    public LayerMask interactableLayer;

    private GameObject carriedObject;
    private bool isDragging;
    private PlayerMovement playerMovement;
    private Transform holdPoint;

    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();

        holdPoint = new GameObject("HoldPoint").transform;
        holdPoint.SetParent(transform);
        holdPoint.localPosition = holdOffset;
    }

    void Update()
    {
        if (Input.GetKeyDown(dragKey))
        {
            if (!isDragging)
                TryPickupObject();
            else
                DropObject();
        }
    }

    void LateUpdate()
    {
        if (isDragging && carriedObject != null)
        {
            carriedObject.transform.position = holdPoint.position;
            carriedObject.transform.rotation = holdPoint.rotation;
        }
    }

    void TryPickupObject()
    {
        Collider[] hitColliders = Physics.OverlapSphere(
            transform.position + transform.forward * holdOffset.z + Vector3.up * holdOffset.y,
            pickupRadius,
            interactableLayer
        );

        foreach (var collider in hitColliders)
        {
            if (collider.CompareTag("Bug"))
            {
                Rigidbody rb = collider.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    carriedObject = collider.gameObject;
                    rb.isKinematic = true;
                    isDragging = true;
                    playerMovement.SetDragging(true);
                    break;
                }
            }
        }
    }

    void DropObject()
    {
        if (carriedObject == null) return;

        Rigidbody rb = carriedObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.velocity = playerMovement.GetCurrentVelocity() * 0.3f;
        }

        carriedObject = null;
        isDragging = false;
        playerMovement.SetDragging(false);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        if (holdPoint != null)
            Gizmos.DrawWireSphere(holdPoint.position, pickupRadius);
    }
}