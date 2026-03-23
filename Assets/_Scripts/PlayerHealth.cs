using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private Text healthText;

    [Header("Death")]
    [SerializeField] private GameObject deathScreen;
    [SerializeField] private FPSController fpsController;

    private int currentHealth;
    private bool isDead = false;

    private void Start()
    {
        currentHealth = maxHealth;
        UpdateUI();

        if (deathScreen != null)
            deathScreen.SetActive(false);
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);

        UpdateUI();

        if (currentHealth <= 0)
            Die();
    }

    public void Heal(int amount)
    {
        if (isDead) return;

        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);

        UpdateUI();
    }

    private void Die()
    {
        isDead = true;

        if (deathScreen != null)
            deathScreen.SetActive(true);

        if (fpsController != null)
        {
            fpsController.CanLook = false;
            fpsController.CanMove = false;
            fpsController.enabled = false;
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Debug.Log("Player died");
    }

    private void UpdateUI()
    {
        if (healthText != null)
            healthText.text = "HP: " + currentHealth;
    }
}