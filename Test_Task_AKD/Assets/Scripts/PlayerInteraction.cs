using UnityEngine;
using UnityEngine.UI;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    [SerializeField] public float interactionDistance = 3f;
    [SerializeField] public KeyCode interactionKey = KeyCode.E;
    [SerializeField] private Image interactionIndicator;
    [SerializeField] private Transform handPosition;
    [SerializeField] private float objectOffset = 0.5f;

    private GameObject heldObject;
    private Rigidbody heldObjectRigidbody;
    private bool isDialogueActive = false;

    void Update()
    {
        if (heldObject != null)
        {
            if (Input.GetKeyDown(interactionKey))
            {
                DropObject(); 
            }

            UpdateHeldObjectPosition();
            return;
        }

        RaycastHit hit;

        Vector3 rayOrigin = playerCamera.transform.position;
        Vector3 rayDirection = playerCamera.transform.forward;

        Debug.DrawRay(rayOrigin, rayDirection * interactionDistance, Color.red);

        if (Physics.Raycast(rayOrigin, rayDirection, out hit, interactionDistance))
        {
            IInteractable interactable = hit.transform.GetComponent<IInteractable>();

            if (interactable != null)
            {
                interactionIndicator.enabled = true;

                if (Input.GetKeyDown(interactionKey) && !isDialogueActive)
                {
                    PickUpObject(hit.transform.gameObject); 
                }
            }
            else
            {
                interactionIndicator.enabled = false;
            }
        }
        else
        {
            interactionIndicator.enabled = false;
        }
    }

    private void PickUpObject(GameObject obj)
    {
        if (heldObject == null)
        {
            heldObject = obj;
            heldObjectRigidbody = obj.GetComponent<Rigidbody>();
            if (heldObjectRigidbody == null)
            {
                heldObjectRigidbody = obj.AddComponent<Rigidbody>();
            }
            heldObjectRigidbody.isKinematic = true;
            heldObject.transform.parent = handPosition;
            heldObject.transform.position = handPosition.position;
            heldObject.transform.rotation = handPosition.rotation;
        }
    }

    private void DropObject()
    {
        if (heldObject != null)
        {
            heldObject.transform.parent = null;
            heldObjectRigidbody.isKinematic = false;
            heldObject = null;
            heldObjectRigidbody = null;
        }
    }

    private void UpdateHeldObjectPosition()
    {
        if (heldObject == null || heldObjectRigidbody == null) return;

        Vector3 targetPosition = handPosition.position;

        Ray ray = new Ray(targetPosition, Vector3.down);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, objectOffset))
        {
            targetPosition.y = hit.point.y + objectOffset;
        }

        heldObject.transform.position = targetPosition;
        heldObject.transform.rotation = handPosition.rotation;
    }

    public void SetDialogueActive(bool active)
    {
        isDialogueActive = active;
    }
}
