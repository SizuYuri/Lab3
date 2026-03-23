using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Gun : MonoBehaviour
{
    [Header("Weapon Info")]
    [SerializeField] private string weaponName = "Pistol";

    [Header("Shooting")]
    [SerializeField] private float damage = 20f;
    [SerializeField] private float fireRate = 0.3f;
    [SerializeField] private float range = 100f;
    [SerializeField] private float spread = 0.01f;
    [SerializeField] private LayerMask hitMask = ~0;

    [Header("Ammo")]
    [SerializeField] private int magazineSize = 12;
    [SerializeField] private int reserveAmmo = 48;
    [SerializeField] private float reloadTime = 1.5f;

    [Header("Recoil")]
    [SerializeField] private Transform weaponVisual;
    [SerializeField] private float recoilBackDistance = 0.03f;
    [SerializeField] private float recoilReturnSpeed = 10f;

    [Header("Effects")]
    [SerializeField] private ParticleSystem muzzleFlash;
    [SerializeField] private AudioSource shootAudio;
    [SerializeField] private AudioSource reloadAudio;
    [SerializeField] private LineRenderer bulletTracer;
    [SerializeField] private Transform shootPoint;
    [SerializeField] private float tracerDuration = 0.03f;

    [Header("UI")]
    [SerializeField] private Text ammoText;

    private Camera playerCamera;
    private int currentAmmo;
    private float nextFireTime;
    private bool isReloading;

    private Vector3 initialLocalPos;
    private Vector3 targetLocalPos;

    public bool IsReloading => isReloading;
    public int CurrentAmmo => currentAmmo;
    public int ReserveAmmo => reserveAmmo;
    public string WeaponName => weaponName;

    private void Start()
    {
        playerCamera = Camera.main;
        currentAmmo = magazineSize;

        if (weaponVisual == null)
            weaponVisual = transform;

        initialLocalPos = weaponVisual.localPosition;
        targetLocalPos = initialLocalPos;

        if (bulletTracer != null)
            bulletTracer.enabled = false;

        UpdateAmmoUI();
    }

    private void OnEnable()
    {
        isReloading = false;
        UpdateAmmoUI();
    }

    private void Update()
    {
        HandleRecoilVisual();

        if (!gameObject.activeInHierarchy || isReloading)
            return;

        if (Input.GetKeyDown(KeyCode.R) && currentAmmo < magazineSize && reserveAmmo > 0)
        {
            StartCoroutine(Reload());
            return;
        }

        if (Input.GetButton("Fire1") && Time.time >= nextFireTime)
        {
            TryShoot();
        }
    }

    private void TryShoot()
    {
        if (playerCamera == null) return;

        if (currentAmmo <= 0)
        {
            if (reserveAmmo > 0 && !isReloading)
                StartCoroutine(Reload());

            return;
        }

        nextFireTime = Time.time + fireRate;
        currentAmmo--;
        UpdateAmmoUI();

        if (muzzleFlash != null)
            muzzleFlash.Play();

        if (shootAudio != null)
            shootAudio.Play();

        ApplyRecoil();

        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        Vector3 direction = ray.direction;
        direction += playerCamera.transform.right * Random.Range(-spread, spread);
        direction += playerCamera.transform.up * Random.Range(-spread, spread);
        direction.Normalize();

        Vector3 hitPoint = ray.origin + direction * range;

        if (Physics.Raycast(ray.origin, direction, out RaycastHit hit, range, hitMask))
        {
            hitPoint = hit.point;

            if (hit.collider.TryGetComponent(out EnemyTarget enemy))
            {
                enemy.TakeDamage(damage);
            }
        }

        if (bulletTracer != null && shootPoint != null)
        {
            StartCoroutine(ShowTracer(shootPoint.position, hitPoint));
        }
    }

    private IEnumerator ShowTracer(Vector3 startPoint, Vector3 endPoint)
    {
        bulletTracer.enabled = true;
        bulletTracer.SetPosition(0, startPoint);
        bulletTracer.SetPosition(1, endPoint);

        yield return new WaitForSeconds(tracerDuration);

        bulletTracer.enabled = false;
    }

    private IEnumerator Reload()
    {
        isReloading = true;

        if (reloadAudio != null)
            reloadAudio.Play();

        yield return new WaitForSeconds(reloadTime);

        int needed = magazineSize - currentAmmo;
        int ammoToLoad = Mathf.Min(needed, reserveAmmo);

        currentAmmo += ammoToLoad;
        reserveAmmo -= ammoToLoad;

        isReloading = false;
        UpdateAmmoUI();
    }

    private void ApplyRecoil()
    {
        targetLocalPos = initialLocalPos - new Vector3(0f, 0f, recoilBackDistance);
    }

    private void HandleRecoilVisual()
    {
        if (weaponVisual == null) return;

        weaponVisual.localPosition = Vector3.Lerp(
            weaponVisual.localPosition,
            targetLocalPos,
            recoilReturnSpeed * Time.deltaTime
        );

        targetLocalPos = Vector3.Lerp(
            targetLocalPos,
            initialLocalPos,
            recoilReturnSpeed * Time.deltaTime
        );
    }

    public void AddAmmo(int amount)
    {
        reserveAmmo += amount;
        UpdateAmmoUI();
    }

    public void ForceUpdateUI()
    {
        UpdateAmmoUI();
    }

    private void UpdateAmmoUI()
    {
        if (ammoText != null)
            ammoText.text = currentAmmo + " / " + reserveAmmo;
    }
}