using UnityEngine;

public class PeekCamera : MonoBehaviour
{
    Quaternion startRotation = Quaternion.identity;
    Vector3 startPosition = new(0, 0, -8);

    const float startHorizontalRotation = -90;
    const float startVerticalRotation = 90;

    float currentHorizontalRotation;
    float currentVerticalRotation;

    float horizontalSpeed = 0;
    float verticalSpeed = 0;

    float endHorizontalRotation;
    float endVerticalRotation;

    float maxVerticalPeek = 170;
    float minVerticalPeek = 10;

    float radius = 8f;
    float angularVelocity = 2f;
    float smoothTime = 0.15f;

    private void Awake()
    {
        gameObject.transform.position = startPosition;
        gameObject.transform.rotation = startRotation;

        endHorizontalRotation = currentHorizontalRotation = startHorizontalRotation;
        endVerticalRotation = currentVerticalRotation = startVerticalRotation;

        transform.LookAt(Vector3.zero);
    }

    private void Update()
    {
        int horizontalInputAxis = 0;
        int verticalInputAxis = 0;
        if (Input.GetKey(KeyCode.LeftArrow)) {
            horizontalInputAxis--;
        }
        if (Input.GetKey(KeyCode.RightArrow)) {
            horizontalInputAxis++;
        }

        if (Input.GetKey(KeyCode.UpArrow)) {
            verticalInputAxis--;
        }
        if (Input.GetKey(KeyCode.DownArrow)) {
            verticalInputAxis++;
        }

        endHorizontalRotation += horizontalInputAxis * angularVelocity;
        endVerticalRotation += verticalInputAxis * angularVelocity;
        endVerticalRotation = Mathf.Clamp(endVerticalRotation, minVerticalPeek, maxVerticalPeek);
    }

    private void FixedUpdate()
    {
        currentHorizontalRotation = Mathf.SmoothDamp(currentHorizontalRotation, endHorizontalRotation, ref horizontalSpeed, smoothTime);
        currentVerticalRotation = Mathf.SmoothDamp(currentVerticalRotation, endVerticalRotation, ref verticalSpeed, smoothTime);
        gameObject.transform.position = radius * GetPosition();
        gameObject.transform.LookAt(Vector3.zero);
        gameObject.transform.rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, 0);
    }

    Vector3 GetPosition()
    {
        float v = currentVerticalRotation * Mathf.Deg2Rad;
        float h = currentHorizontalRotation * Mathf.Deg2Rad;
        float x = Mathf.Sin(v) * Mathf.Cos(h);
        float y = Mathf.Cos(v);
        float z = Mathf.Sin(v) * Mathf.Sin(h);

        return new(x, y, z);
    }
}
