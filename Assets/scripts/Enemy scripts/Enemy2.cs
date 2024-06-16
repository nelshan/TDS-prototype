using UnityEngine;
using TMPro;
using System.Collections;

public class Enemy2 : MonoBehaviour
{
    [SerializeField] private float speed;
    private Transform target;

    public int damage;
    public int health;
    public float separationRadius = 0.5f; // Radius within which enemies will try to separate
    public float separationForce = 0.5f; // Force with which enemies will push each other away
    
    // Projectile settings
    public GameObject projectilePrefab; // Reference to the projectile prefab
    public Transform projectileSpawnPos; // Position from where the projectile will spawn
    public float projectileCooldown = 2f; // Cooldown time between projectiles
    private float nextProjectileTime; // Time when the enemy can shoot the next projectile

    public GameObject deathBooldEffect; // Death effect with partical effect and sound
    
    private Animator animator;

    private void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        animator = GetComponentInChildren<Animator>();
        nextProjectileTime = Time.time; // Initialize the next projectile time
    }

    private void Update()
    {
        if (health <= 0)
        {
            HandleDeath(); // Handle the death and effects
            return; // Exit the Update method
        }
        // Move towards the player
        Vector2 newPosition = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

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
        }
        else if (other.CompareTag("PlayerProjectile")) // Check for collision with projectile
        {
            TakeDamage(other.GetComponent<Projectile>().damage);
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

        // Destroy the enemy game object
        Destroy(gameObject);
    }

    private void ShootProjectile()
    {
        if (projectilePrefab != null && projectileSpawnPos != null)
        {
            GameObject projectile = Instantiate(projectilePrefab, projectileSpawnPos.position, Quaternion.identity);
            EnemyProjectile enemyProjectile = projectile.GetComponent<EnemyProjectile>();
            enemyProjectile.Initialize(target); // Set the target for the projectile
        }
    }
}

