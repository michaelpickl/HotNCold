using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PuzzlePiece : MonoBehaviour 
{
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;
    private Vector3 lastValidPosition;
    private Quaternion lastValidRotation;
    

    private Collider tableCollider;

    private Rigidbody rb;
    private bool isGrabbed = false;
    public bool isAnimated = false;

    private void Awake() {
        grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        grabInteractable.selectEntered.AddListener(OnGrab);
        grabInteractable.selectExited.AddListener(OnRelease);

        rb = GetComponent<Rigidbody>();

        GameObject table = GameObject.Find("Table");
        if (table != null) {
            tableCollider = table.GetComponent<Collider>();
        }

        SavePosition();
    }

    private void FixedUpdate() {
        if (!isGrabbed && !isAnimated) {
            if (!IsWithinTableBounds()) {
                ResetPosition();
            }
        }
    }

    public void SavePosition()
    {
        lastValidPosition = transform.position;
        lastValidRotation = transform.rotation;
    }

    private void OnGrab(SelectEnterEventArgs args) {
        GameManager.Instance.SetDraggingPiece(transform);
        isGrabbed = true;
    }

    private void OnRelease(SelectExitEventArgs args) {
        isGrabbed = false;
        if (IsWithinTableBounds()) {
            lastValidPosition = transform.position;
            lastValidRotation = transform.rotation;
            if (tableCollider != null)
            {
                float pieceHeight = GetPieceThickness();
                if (transform.position.y > tableCollider.bounds.max.y + (pieceHeight / 2))
                {
                    lastValidPosition.y = tableCollider.bounds.max.y + (pieceHeight / 2);
                }
            }
        } else {
            if(!isAnimated) {
                ResetPosition();
            }
        }

        GameManager.Instance.CheckPlacement(transform);
    }

    public float GetPieceThickness(){
        return GetComponent<MeshRenderer>().bounds.size.y;
    }

    private bool IsWithinTableBounds() {
        if (tableCollider == null) return true;

        Bounds bounds = tableCollider.bounds;
        Vector3 piecePosition = transform.position;

        bool insideXZ = bounds.min.x <= piecePosition.x && piecePosition.x <= bounds.max.x &&
                        bounds.min.z <= piecePosition.z && piecePosition.z <= bounds.max.z;

        bool aboveTable = piecePosition.y >= bounds.min.y;

        return insideXZ && aboveTable;
    }

    private void ResetPosition() {
        if (rb != null) {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }
        transform.position = lastValidPosition;
        transform.rotation = lastValidRotation;
        Debug.Log("PuzzlePiece zurückgesetzt!");

        Invoke(nameof(EnablePhysics), 0.1f);
    }

    private void EnablePhysics() {
        if (rb != null) {
            rb.isKinematic = false;
        }
    }
}
