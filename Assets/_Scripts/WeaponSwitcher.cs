using UnityEngine;

public class WeaponSwitcher : MonoBehaviour
{
    [SerializeField] private GameObject[] weapons;
    [SerializeField] private KeyCode[] weaponKeys = { KeyCode.Alpha1, KeyCode.Alpha2 };

    private int currentWeaponIndex = 0;

    private void Start()
    {
        ActivateWeapon(currentWeaponIndex, true);
    }

    private void Update()
    {
        if (weapons == null || weapons.Length == 0) return;

        for (int i = 0; i < weaponKeys.Length && i < weapons.Length; i++)
        {
            if (Input.GetKeyDown(weaponKeys[i]))
            {
                ActivateWeapon(i);
            }
        }

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll > 0f)
        {
            int nextIndex = (currentWeaponIndex + 1) % weapons.Length;
            ActivateWeapon(nextIndex);
        }
        else if (scroll < 0f)
        {
            int prevIndex = (currentWeaponIndex - 1 + weapons.Length) % weapons.Length;
            ActivateWeapon(prevIndex);
        }
    }

    private void ActivateWeapon(int newIndex, bool force = false)
    {
        if (weapons == null || weapons.Length == 0) return;
        if (newIndex < 0 || newIndex >= weapons.Length) return;
        if (!force && newIndex == currentWeaponIndex) return;

        for (int i = 0; i < weapons.Length; i++)
        {
            bool active = i == newIndex;
            weapons[i].SetActive(active);

            if (active && weapons[i].TryGetComponent<Gun>(out Gun gun))
            {
                gun.ForceUpdateUI();
            }
        }

        currentWeaponIndex = newIndex;
    }

    public Gun GetCurrentGun()
    {
        if (weapons == null || weapons.Length == 0) return null;

        if (weapons[currentWeaponIndex].TryGetComponent<Gun>(out Gun gun))
            return gun;

        return null;
    }
}