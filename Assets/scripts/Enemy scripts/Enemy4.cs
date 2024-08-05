using UnityEngine;
using TMPro;
using System.Collections;

public class Enemy4 : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 2f; // Movement speed of Enemy4

    [SerializeField] private float projectileSpeed = 5f; // Speed of the projectiles shot by Enemy4
    private Transform target;

    [SerializeField] private int damage;
    [SerializeField] private int health;
    [SerializeField] private float separationRadius = 0.5f; // Radius within which enemies will try to separate
    [SerializeField] private float separationForce = 0.5f; // Force with which enemies will push each other away

    [SerializeField] private GameObject deathBooldEffect; // Death effect with animation and sound

    [SerializeField] private float attackRange = 5f; // Range within which enemy attacks
    [SerializeField] private float fireRate = 1f; // Time between attacks
    private float nextFireTime = 0f;
    [SerializeField] private GameObject projectilePrefab; // The projectile to be shot
    [SerializeField] private Transform projectileSpawnPosition; // The position from which projectiles are spawned

    [SerializeField] private int projectileCount = 8; // Number of projectiles to shoot in a circle

    private Animator animator;
    private ItemDropManager itemDropManager; // Reference to the ItemDropManager

    private void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        animator = GetComponentInChildren<Animator>();
        itemDropManager = GetComponent<ItemDropManager>(); // Initialize the item drop manager
    }

    private void Update()
    {
        if (health <= 0)
        {
            HandleDeath(); // Handle the death and effects
            return; // Exit the Update method
        }

        MoveTowardsPlayer();

        // Check if player is within attack range
        if (Vector2.Distance(transform.position, target.position) <= attackRange && Time.time >= nextFireTime)
        {
            ShootProjectilesInCircle();
            nextFireTime = Time.time + fireRate; // Set the next fire time
        }
    }

    private void MoveTowardsPlayer()
    {
        // Move towards the player
        Vector2 newPosition = Vector2.MoveTowards(transform.position, target.position, movementSpeed * Time.deltaTime);

        // Apply separation force to avoid stacking
        Collider2D[] nearbyEnemies = Physics2D.OverlapCircleAll(transform.position, separationRadius, LayerMask.GetMask("Enemy"));
        foreach (var enemy in nearbyEnemies)
        {
            if (enemy != null && enemy.gameObject != this.gameObject)
            {
                Vector2 separationDirection = (transform.position - enemy.transform.position).normalized;
                newPosition += separationDirection * separationForce * Time.deltaTime;
            }
        }

        // Apply the final new position
        transform.position = newPosition;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Apply damage to the player
            player_controller player = other.GetComponent<player_controller>();
            if (player != null)
            {
                player.TakeDam(damage);
            }

            CinemachineShake.Instance.ShakeCamera(5f, 0.1f); // Shake the VirtualCamera intensity and shakeTime
            
            // Play damage sound
            AudioManager.Instance.PlaySFX("player damage sfx");
        }
        else if (other.CompareTag("PlayerProjectile")) // Check for collision with projectile
        {
            Projectile projectile = other.GetComponent<Projectile>();
            if (projectile != null)
            {
                TakeDamage(projectile.damage);
            }
        }
        else if (other.CompareTag("PlayerexplosionProjectile")) // Check for collision with explosion projectile
        {
            ExplosionBullet2 explosionBullet = other.GetComponent<ExplosionBullet2>();
            if (explosionBullet != null)
            {
                TakeDamage(explosionBullet.damage);
            }
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        animator.SetTrigger("Damage"); // Play enemy damage animation
        if (health <= 0)
        {
            HandleDeath(); // Handle the death and effects
        }
    }

    private void HandleDeath()
    {
        // Award score and increment enemies destroyed
        int randScoreBonus = Random.Range(100, 200);
        target.GetComponent<player_controller>().score += randScoreBonus;
        target.GetComponent<player_controller>().IncrementEnemiesDestroyed();

        // Instantiate death effect
        if (deathBooldEffect != null)
        {
            Instantiate(deathBooldEffect, transform.position, Quaternion.identity);
        }

        itemDropManager.TryDropItem(transform.position);

        // Destroy the enemy game object
        Destroy(gameObject);
    }

    private void ShootProjectilesInCircle()
    {
        // Shoot projectiles in a circular pattern
        float angleStep = 360f / projectileCount;
        float angle = 0f;

        for (int i = 0; i < projectileCount; i++)
        {
            // Calculate the projectile's direction
            float projectileDirX = projectileSpawnPosition.position.x + Mathf.Sin((angle * Mathf.PI) / 180);
            float projectileDirY = projectileSpawnPosition.position.y + Mathf.Cos((angle * Mathf.PI) / 180);

            Vector2 projectileVector = new Vector2(projectileDirX, projectileDirY);
            Vector2 projectileMoveDirection = (projectileVector - (Vector2)projectileSpawnPosition.position).normalized;

            // Instantiate the projectile
            GameObject projectile = Instantiate(projectilePrefab, projectileSpawnPosition.position, Quaternion.identity);
            Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = projectileMoveDirection * projectileSpeed; // Set the projectile speed
            }

            // Initialize the projectile
            Enemy4Projectile enemy4Projectile = projectile.GetComponent<Enemy4Projectile>();
            if (enemy4Projectile != null)
            {
                enemy4Projectile.Initialize();
            }

            angle += angleStep;
        }
    }

    // Draw Gizmos to visualize the attack range
    private void OnDrawGizmosSelected()
    {
        // Draw a red wireframe sphere for the attack range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
