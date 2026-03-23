using UnityEngine;

public class EnemyTarget : MonoBehaviour
{
    [SerializeField] private float health = 50f;
    [SerializeField] private GameObject deathEffect;

    public void TakeDamage(float amount)
    {
        health -= amount;

        if (health <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}