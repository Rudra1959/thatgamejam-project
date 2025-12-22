using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    public Transform player;
    public bool useIsometricView = true;

    // Isometric Settings
    [Header("Isometric Settings")]
    public Vector3 isoOffset = new Vector3(-10, 10, -10);
    public Vector3 isoRotation = new Vector3(30, 45, 0);

    // Side-Scroll Settings
    [Header("Side-Scroll Settings")]
    public Vector3 sideOffset = new Vector3(0, 2, -10);
    public Vector3 sideRotation = new Vector3(0, 0, 0);

    void FixedUpdate()
    {
        Vector3 targetPos;
        Quaternion targetRot;

        if (useIsometricView) {
            targetPos = player.position + isoOffset;
            targetRot = Quaternion.Euler(isoRotation);
        } else {
            targetPos = player.position + sideOffset;
            targetRot = Quaternion.Euler(sideRotation);
        }

        // Smooth transition
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * 3f);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, Time.deltaTime * 3f);
    }
}