using UnityEngine;

public class GarageDoor : MonoBehaviour
{
    [SerializeField] private float openAngle = 90f;
    [SerializeField] private float openTime = 10f;

    private Quaternion startRotation;
    private Quaternion targetRotation;
    private float elapsedTime = 0f;
    private bool isOpening = false;

    void Start()
    {
        startRotation = transform.rotation;
        targetRotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0, openAngle, 0));
        isOpening = true;
    }

    void Update()
    {
        if (isOpening)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / openTime);
            transform.rotation = Quaternion.Lerp(startRotation, targetRotation, t);

            if (t >= 1f)
            {
                isOpening = false;
            }
        }
    }
}
