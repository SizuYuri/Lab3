using UnityEngine;

public class HealthPack : MonoBehaviour
{
    [SerializeField] private int healAmount = 25;
    [SerializeField] private AudioClip pickupSound;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (other.TryGetComponent<PlayerHealth>(out PlayerHealth health))
        {
            health.Heal(healAmount);

            if (pickupSound != null)
                AudioSource.PlayClipAtPoint(pickupSound, transform.position);

            Destroy(gameObject);
        }
    }
}