using UnityEngine;

public class SideScrollCamera : MonoBehaviour
{
    public Transform player;       
    public float distance = 10.0f; 
    public float height = 3.0f;   
    public float smoothSpeed = 0.125f; 

    void LateUpdate()
    {
        if (player == null) return;

        // Calculate a target position that maintains a fixed side view
        // The camera's depth (Z) is offset from the player's Z by 'distance'
        Vector3 targetPosition = new Vector3(player.position.x, player.position.y + height, player.position.z - distance);

        // Smoothly follow
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed);

        // Lock rotation to look at player
        transform.LookAt(player.position + Vector3.up * (height * 0.5f));
    }
}