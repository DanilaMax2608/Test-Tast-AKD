using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float sensitivity = 2;
    [SerializeField] private float smoothing = 1.5f;

    [SerializeField] private float swayAmount = 0.02f;
    [SerializeField] private float swaySpeed = 1f;

    [SerializeField] private float headBobAngle = 2f;
    [SerializeField] private float headBobSmoothing = 0.1f;

    [SerializeField] private float walkSwayAngleZ = 3f;
    [SerializeField] private float walkSwayAngleX = 1f;
    [SerializeField] private float walkSwayAngleY = 1f;
    [SerializeField] private float walkSwaySpeed = 8f;
    [SerializeField] private float walkShiftAmount = 0.02f;

    private Vector2 velocity;
    private Vector2 frameVelocity;
    private Vector3 originalPosition;
    private float currentHeadBobAngle = 0f;
    private float walkSwayTimer = 0f;

    private Transform target;
    private bool isDialogueActive = false;

    private Vector3 targetPosition;
    private Vector3 velocityPosition;
    private float smoothTime = 0.3f;

    void Reset()
    {
        player = GetComponentInParent<PlayerMovement>().transform;
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        originalPosition = transform.localPosition;

        transform.localRotation = Quaternion.identity;
        player.localRotation = Quaternion.identity;

        transform.localEulerAngles = Vector3.zero;
        player.localEulerAngles = Vector3.zero;
    }

    [System.Obsolete]
    void Update()
    {
        if (isDialogueActive && target != null)
        {
            Vector3 direction = target.position - transform.position;
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            if (Quaternion.Angle(transform.rotation, targetRotation) > 0.1f)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 2f);
            }
            return;
        }


        Vector2 mouseDelta = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
        Vector2 rawFrameVelocity = Vector2.Scale(mouseDelta, Vector2.one * sensitivity);
        frameVelocity = Vector2.Lerp(frameVelocity, rawFrameVelocity, 1 / smoothing);
        velocity += frameVelocity;
        velocity.y = Mathf.Clamp(velocity.y, -90, 90);

        transform.localRotation = Quaternion.AngleAxis(-velocity.y, Vector3.right);
        player.localRotation = Quaternion.AngleAxis(velocity.x, Vector3.up);

        float targetHeadBobAngle = Mathf.Clamp(-mouseDelta.x * headBobAngle, -headBobAngle, headBobAngle);
        currentHeadBobAngle = Mathf.Lerp(currentHeadBobAngle, targetHeadBobAngle, headBobSmoothing);
        transform.localRotation *= Quaternion.Euler(0, 0, currentHeadBobAngle);

        if (player.GetComponent<Rigidbody>().velocity.magnitude > 0.1f)
        {
            walkSwayTimer += Time.deltaTime * walkSwaySpeed;

            float swayAngleX = Mathf.Sin(walkSwayTimer) * walkSwayAngleX;
            float swayAngleY = Mathf.Cos(walkSwayTimer) * walkSwayAngleY;
            float swayAngleZ = Mathf.Sin(walkSwayTimer) * walkSwayAngleZ;
            float shiftY = Mathf.Cos(walkSwayTimer * 2) * walkShiftAmount;

            transform.localPosition = originalPosition + new Vector3(0, shiftY, 0);
            transform.localRotation *= Quaternion.Euler(swayAngleX, swayAngleY, swayAngleZ);
        }
        else
        {
            float swayX = Mathf.Sin(Time.time * swaySpeed) * swayAmount;
            float swayY = Mathf.Sin(Time.time * swaySpeed * 0.5f) * swayAmount;

            transform.localPosition = originalPosition + new Vector3(swayX, swayY, 0);
        }
    }

    public void StartDialogue(Transform target)
    {
        this.target = target;
        isDialogueActive = true;
        targetPosition = target.position;
    }

    public void EndDialogue()
    {
        this.target = null;
        isDialogueActive = false;
    }
}
