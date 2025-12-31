using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;
    public bool isFreezing = false;

    void Start() => currentHealth = maxHealth;

    void Update() {
        if (isFreezing) TakeDamage(1f * Time.deltaTime); 
    }

    public void TakeDamage(float amount) {
        currentHealth -= amount;
        if (currentHealth <= 0) Die();
    }

    public void Heal(float amount) {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
    }

    void Die() {
        // Reload scene on death
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
}