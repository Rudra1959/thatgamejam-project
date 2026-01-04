using UnityEngine;
using UnityEngine.UI;

public class DynamicHealthBar : MonoBehaviour
{
    [Header("References")]
    public Image fillImage; 
    private PlayerStats player;

    [Header("Choose Your Colors")]
    public Color color100 = Color.green;   //
    public Color color80 = Color.yellow;    //
    public Color color50 = new Color(1f, 0.5f, 0f); // Orange
    public Color color25 = Color.red;      //

    void Start()
    {
        // Find the player automatically so you don't have to drag it in
        player = Object.FindFirstObjectByType<PlayerStats>();
        
        if (fillImage == null)
            fillImage = GetComponent<Image>();
    }

    void Update()
    {
        if (player == null || fillImage == null) return;

        // 1. Get health as a percentage (0.0 to 1.0)
        float healthPercent = player.currentHealth / player.maxHealth;

        // 2. Shrink the bar toward the left
        fillImage.fillAmount = healthPercent;

        // 3. Change color based on your chosen thresholds
        if (healthPercent > 0.80f)
            fillImage.color = color100;
        else if (healthPercent > 0.50f)
            fillImage.color = color80;
        else if (healthPercent > 0.25f)
            fillImage.color = color50;
        else
            fillImage.color = color25;
    }
}