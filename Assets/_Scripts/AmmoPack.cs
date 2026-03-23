using UnityEngine;

public class AmmoPack : MonoBehaviour
{
    [SerializeField] private int ammoAmount = 20;
    [SerializeField] private AudioClip pickupSound;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        WeaponSwitcher switcher = other.GetComponentInChildren<WeaponSwitcher>();
        Gun activeGun = null;

        if (switcher != null)
            activeGun = switcher.GetCurrentGun();

        if (activeGun == null)
            activeGun = other.GetComponentInChildren<Gun>(true);

        if (activeGun != null)
        {
            activeGun.AddAmmo(ammoAmount);

            if (pickupSound != null)
                AudioSource.PlayClipAtPoint(pickupSound, transform.position);

            Destroy(gameObject);
        }
    }
}