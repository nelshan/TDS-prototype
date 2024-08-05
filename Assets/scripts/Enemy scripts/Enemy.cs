using UnityEngine;
using TMPro;
using System.Collections;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float Movementspeed;
    private Transform target;

    [SerializeField] private int damage;
    [SerializeField] private int health;
    [SerializeField] private float separationRadius = 0.5f; // Radius within which enemies will try to separate
    [SerializeField] private float separationForce = 0.5f; // Force with which enemies will push each other away
    
    [SerializeField] private GameObject deathBooldEffect; // Death effect with particle effect and sound
    private Animator animator;

    [SerializeField] private float FallbackDistance = 1f; // Distance to knock back
    [SerializeField] private float FallbackDuration = 0.5f; // Duration of the knockback effect
    [SerializeField] private float attackCooldown = 1f; // Cooldown time between attacks
    private float nextAttackTime; // Time when the enemy can attack again

    private ItemDropManager itemDropManager; // Reference to the ItemDropManager
    
    
    private void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        animator = GetComponent<Animator>();
        nextAttackTime = Time.time; // Initialize the next attack time
        itemDropManager = GetComponent<ItemDropManager>(); // Initialize the item drop manager
    }

    private void Update()
    {
        if (health <= 0)
        {
            HandleDeath(); // Handle the death and effects
            return; // Exit the Update method
        }

        if (Time.time >= nextAttackTime)
        {
            // Move towards the player
            Vector2 newPosition = Vector2.MoveTowards(transform.position, target.position, Movementspeed * Time.deltaTime);

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
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && Time.time >= nextAttackTime)
        {
            // Apply damage to the player
            other.GetComponent<player_controller>().TakeDam(damage);
            nextAttackTime = Time.time + attackCooldown; // Set the next attack time

            // Play attack sound
            AudioManager.Instance.PlaySFX("Enemy1 atk sfx");

            CinemachineShake.Instance.ShakeCamera(5f, 0.1f); //shake the VirtualCamera intensity and shakeTime
            
            // Apply knockback effect
            Vector2 knockbackDirection = (transform.position - other.transform.position).normalized;
            StartCoroutine(ApplyKnockback(knockbackDirection));
        }
        else if (other.CompareTag("PlayerProjectile")) // Check for collision with projectile
        {
            TakeDamage(other.GetComponent<Projectile>().damage);
        }
        else if (other.CompareTag("PlayerexplosionProjectile")) // Check for collision with explosion projectile
        {
            TakeDamage(other.GetComponent<ExplosionBullet2>().damage);
        }
    }

    private IEnumerator ApplyKnockback(Vector2 direction)
    {
        float elapsedTime = 0f;
        Vector2 startPosition = transform.position;
        Vector2 targetPosition = startPosition + direction * FallbackDistance;

        while (elapsedTime < FallbackDuration)
        {
            transform.position = Vector2.Lerp(startPosition, targetPosition, elapsedTime / FallbackDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition; // Ensure the final position is set
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
        Instantiate(deathBooldEffect, transform.position, Quaternion.identity);

        // Try dropping an item
        if (itemDropManager != null)
        {
            itemDropManager.TryDropItem(transform.position);
        }

        // Destroy the enemy game object
        Destroy(gameObject);
    }
}