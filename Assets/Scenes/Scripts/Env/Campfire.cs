using UnityEngine;
using UnityEngine.InputSystem;

public class Campfire : MonoBehaviour
{
    public GameObject fireVisual;
    public float healRate = 10f;
    public int sticksRequired = 3;
    private bool isLit = false;

    // This name MUST match exactly what the PlayerController calls
    public void AttemptToLight() 
    {
        if (isLit) return;

        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            InventoryManager inv = player.GetComponent<InventoryManager>();
            if (inv != null && inv.ConsumeSticks(sticksRequired))
            {
                isLit = true;
                if (fireVisual != null) fireVisual.SetActive(true);
                Debug.Log("Fire started! Character is warming up.");
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (isLit && other.CompareTag("Player"))
        {
            other.GetComponent<PlayerStats>()?.Heal(healRate * Time.deltaTime);
        }
    }
}