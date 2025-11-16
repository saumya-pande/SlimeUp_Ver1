using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private UiManager uiManager;
    [SerializeField] private int maxHealth = 3;
    private int currentHealth;
    private bool isDead = false;

    private void Start()
    {
        if (uiManager == null)
        {
            uiManager = FindFirstObjectByType<UiManager>();
            if (uiManager == null)
            {
                Debug.LogError("UiManager reference missing!");
                return;
            }
        }

        currentHealth = maxHealth;
        uiManager.InitializeHearts(maxHealth);
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;  // Prevent taking damage when already dead

        currentHealth = Mathf.Max(0, currentHealth - amount);
        uiManager.UpdateHealthDisplay(currentHealth);
        uiManager.TakeDamageDisplay();

        Debug.Log("Player took " + amount + " damage. Current health: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        uiManager.UpdateHealthDisplay(currentHealth);
    }

    private void Die()
    {
        isDead = true;
        Debug.Log("Player Died!");
        uiManager.ShowLoseMessage();
    }

    public void Respawn()
    {
        // Reset health to max
        currentHealth = maxHealth;
        isDead = false;
        
        // Update UI
        uiManager.UpdateHealthDisplay(currentHealth);
        uiManager.HideLoseMessage();  // Hide the lose message
        
        Debug.Log("Player respawned with full health");
    }

    public bool IsDead => isDead;
    public int CurrentHealth => currentHealth;
}
