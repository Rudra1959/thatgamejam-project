using UnityEngine;

public class SideScrollerCamera : MonoBehaviour
{
    [Header("Target")]
    public Transform player; // Drag your Player object here

    [Header("Positioning")]
    public Vector3 offset = new Vector3(0, 3, -15); // Adjust Z for distance, Y for height
    public float smoothTime = 0.2f;

    [Header("Rotation")]
    public Vector3 cameraRotation = new Vector3(10, 0, 0); // Slight downward tilt

    private Vector3 currentVelocity = Vector3.zero;

    void Start()
    {
        // Set the initial rotation once so it stays as a side-scroller
        transform.eulerAngles = cameraRotation;
    }

    void LateUpdate()
    {
        if (player == null) return;

        // 1. Calculate the target position (Player position + fixed offset)
        Vector3 targetPosition = player.position + offset;

        // 2. Smoothly move the camera to that position
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref currentVelocity, smoothTime);
    }
}