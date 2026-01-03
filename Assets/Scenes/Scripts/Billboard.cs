using UnityEngine;

public class Billboard : MonoBehaviour
{
    [SerializeField] private bool lockYAxis = true;

    private Transform camTransform;

    void Start()
    {
        // Cache the main camera transform for better performance
        if (Camera.main != null)
        {
            camTransform = Camera.main.transform;
        }
    }

    // LateUpdate ensures the camera has finished moving before we rotate the sprite
    void LateUpdate()
    {
        if (camTransform == null) return;

        // Calculate the direction the sprite should face
        Vector3 targetPosition = transform.position + camTransform.rotation * Vector3.forward;
        Vector3 targetOrientation = camTransform.rotation * Vector3.up;

        // Apply rotation
        transform.LookAt(targetPosition, targetOrientation);

        // If lockYAxis is true, the tree won't "lean" back when the camera is high up
        if (lockYAxis)
        {
            transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);
        }
    }
}