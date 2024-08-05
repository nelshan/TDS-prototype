using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 15f;
    [SerializeField] private GameObject projectile;
    [SerializeField] private Transform projectileSpawnPos;
    [SerializeField] private float startTimeBetweenShots = 0.5f;

    private Animator weaponAnimator;
    private float lastShotTime;
    private Camera mainCamera;

    void Awake()
    {
        weaponAnimator = GetComponentInChildren<Animator>();
        mainCamera = Camera.main;
        lastShotTime = -startTimeBetweenShots; // So the player can shoot immediately at start
    }

    void Update()
    {
        if (!PushMenu.GameIsPused)
        {
            HandleAiming();
            HandleShooting();
        }
    }

    private void HandleAiming()
    {
        Vector2 direction = mainCamera.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    private void HandleShooting()
    {
        if (Input.GetMouseButton(0) && Time.time - lastShotTime >= startTimeBetweenShots)
        {
            Shoot();
            lastShotTime = Time.time;
        }
    }

    private void Shoot()
    {
        weaponAnimator.SetTrigger("shoot");
        Instantiate(projectile, projectileSpawnPos.position, transform.rotation);
    }
}
