using UnityEngine;

public class PerfectSideCamera : MonoBehaviour
{
    public Transform target; // Drag Player here
    public Vector3 offset = new Vector3(0, 4, -12); // Height and Distance
    public float smoothTime = 0.3f;
    
    private Vector3 velocity = Vector3.zero;

    void LateUpdate()
    {
        if (target == null) return;

        // Calculate where the camera should be
        Vector3 desiredPosition = target.position + offset;

        // Smoothly move to that position
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothTime);

        // Force the camera to always look at the player's chest
        transform.LookAt(target.position + Vector3.up * 1.5f);
    }
}