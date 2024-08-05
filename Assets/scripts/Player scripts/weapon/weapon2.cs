using UnityEngine;

public class Weapon2 : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 15f;
    [SerializeField] private GameObject projectile;
    [SerializeField] private Transform projectileSpawnPos;
    [SerializeField] private float startTimeBetweenShots = 0.5f;

    private Animator weaponAnimator;
    private float timeBetweenShots;
    private Camera mainCamera;

    void Awake()
    {
        weaponAnimator = GetComponentInChildren<Animator>();
        mainCamera = Camera.main;
        timeBetweenShots = startTimeBetweenShots;
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
        if (Input.GetMouseButton(0))
        {
            if (timeBetweenShots <= 0)
            {
                Shoot();
                timeBetweenShots = startTimeBetweenShots;
            }
            else
            {
                timeBetweenShots -= Time.deltaTime;
            }
        }
        else
        {
            timeBetweenShots = 0;
        }
    }
    private void Shoot()
    {
        weaponAnimator.SetTrigger("shoot");
        Instantiate(projectile, projectileSpawnPos.position, transform.rotation);
    }
}
