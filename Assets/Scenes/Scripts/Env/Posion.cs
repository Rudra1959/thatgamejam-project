using UnityEngine;

public class PoisonMushroom : MonoBehaviour
{
    public float damageAmount = 2f;
    public float poisonRadius = 2f;

    void Update() {
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null && Vector3.Distance(transform.position, player.transform.position) < poisonRadius) {
            player.GetComponent<PlayerStats>().TakeDamage(damageAmount * Time.deltaTime);
            // Visual feedback: You could add a green particle effect here
        }
    }
}