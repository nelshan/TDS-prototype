using UnityEngine;
using TMPro;
using System.Collections;

public class Enemy2 : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float stoppingDistance; // Distance at which the enemy stops moving towards the player
    private Transform target;

    [SerializeField] private int damage;
    [SerializeField] private int health;
    [SerializeField] private float separationRadius = 0.5f; // Radius within which enemies will try to separate
    [SerializeField] private float separationForce = 0.5f; // Force with which enemies will push each other away
    
    [SerializeField] private GameObject projectilePrefab; // Reference to the projectile prefab
    [SerializeField] private Transform projectileSpawnPos; // Position from where the projectile will spawn
    [SerializeField] private float projectileCooldown = 2f; // Cooldown time between projectiles
    private float nextProjectileTime; // Time when the enemy can shoot the next projectile

    [SerializeField] private GameObject deathBooldEffect; // Death effect with partical effect and sound

    private Animator animator;

    private ItemDropManager itemDropManager; // Reference to the ItemDropManager

    private void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        animator = GetComponentInChildren<Animator>();
        nextProjectileTime = Time.time; // Initialize the next projectile time
        itemDropManager = GetComponent<ItemDropManager>(); // Initialize the item drop manager
    }

    private void Update()
    {
        if (health <= 0)
        {
            HandleDeath(); // Handle the death and effects
            return; // Exit the Update method
        }

        float distanceToPlayer = Vector2.Distance(transform.position, target.position);
        if (distanceToPlayer > stoppingDistance)
        {
            // Move towards the player
            Vector2 newPosition = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

            // Apply separation force to avoid stacking
            Collider2D[] nearbyEnemies = Physics2D.OverlapCircleAll(transform.position, separationRadius, LayerMask.GetMask("Enemy"));
            foreach (var enemy in nearbyEnemies)
            {
                if (enemy != null && enemy.gameObject != this.gameObject)
                {
                    Vector2 separationDirection = (transform.position - enemy.transform.position).normalized;
                    newPosition += (Vector2)separationDirection * separationForce * Time.deltaTime;
                }
            }

            // Apply the final new position
            transform.position = newPosition;
        }

        if (Time.time >= nextProjectileTime)
        {
            ShootProjectile();
            nextProjectileTime = Time.time + projectileCooldown; // Set the next projectile time
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Apply damage to the player
            other.GetComponent<player_controller>().TakeDam(damage);
            
            AudioManager.Instance.PlaySFX("player damage sfx");
        }
        else if (other.CompareTag("PlayerProjectile")) // Check for collision with projectile
        {
            TakeDamage(other.GetComponent<Projectile>().damage);
        }
        else if (other.CompareTag("PlayerexplosionProjectile")) // Check for collision with explosionprojectile
        {
            TakeDamage(other.GetComponent<ExplosionBullet2>().damage);
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

        //Instantiate death effect
        Instantiate(deathBooldEffect, transform.position, Quaternion.identity);

        itemDropManager.TryDropItem(transform.position);
        
        // Destroy the enemy game object
        Destroy(gameObject);
    }

    private void ShootProjectile()
    {
        if (projectilePrefab != null && projectileSpawnPos != null)
        {
            GameObject projectile = Instantiate(projectilePrefab, projectileSpawnPos.position, Quaternion.identity);
            Enemy2Projectile enemyProjectile = projectile.GetComponent<Enemy2Projectile>();
            enemyProjectile.Initialize(target); // Set the target for the projectile
            
            AudioManager.Instance.PlaySFX("Enemy2 Projectileshot atk sfx");
        }
    }

    private void OnDrawGizmosSelected() // Draw the stopping distance
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, stoppingDistance);
    }
}