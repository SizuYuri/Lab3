using UnityEngine;

public class DamageZone : MonoBehaviour
{
    [SerializeField] private int damagePerSecond = 20;

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (other.TryGetComponent<PlayerHealth>(out PlayerHealth playerHealth))
        {
            playerHealth.TakeDamage(Mathf.CeilToInt(damagePerSecond * Time.deltaTime));
        }
    }
}