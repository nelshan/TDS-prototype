using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class Enemy5 : MonoBehaviour
{
    [SerializeField] private float speed;
    private Vector2 targetLocation;

    public int damage;
    public int health;
    public float separationRadius = 0.5f; // Radius within which enemies will try to separate
    public float separationForce = 0.5f; // Force with which enemies will push each other away
    public GameObject deathBloodEffect; // Death effect with particle effect and sound
    public GameObject spawnObject; // Object to spawn after reaching the location
    private Animator animator;

    // Range settings for random position around player
    public float minDistance = 2f;
    public float maxDistance = 5f;
    private bool hasReachedTargetLocation = false; // Flag to check if the target location is reached
    private bool isSpawning = false; // Flag to ensure spawning coroutine runs only once

    public float fallbackDuration = 0.5f; // Duration of the knockback effect
    public float attackCooldown = 1f; // Cooldown time between attacks
    private float nextAttackTime; // Time when the enemy can attack again
    public float attackRange = 3f; // Range within which the enemy will charge towards the player

    public float chargeForce = 10f; // Force applied when charging towards the player
    public float fallbackForce = 5f; // Force applied when falling back

    private Rigidbody2D rb;
    private bool isCharging = false; // Flag to check if the enemy is charging
    private Vector2 initialPosition; // To store the initial position before charge

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>(); // Get the Rigidbody2D component
        SetRandomTargetLocation();
        nextAttackTime = Time.time; // Initialize the next attack time
    }

    private void Update()
    {
        if (health <= 0)
        {
            HandleDeath(); // Handle the death and effects
            return; // Exit the Update method
        }

        Transform playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (playerTransform == null)
            return;

        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer <= attackRange && Time.time >= nextAttackTime)
        {
            if (!isCharging)
            {
                StartCoroutine(ChargeAndAttack(playerTransform));
            }
        }
        else if (!hasReachedTargetLocation)
        {
            // Move towards the target location
            Vector2 newPosition = Vector2.MoveTowards(transform.position, targetLocation, speed * Time.deltaTime);

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

            // Check if enemy reached the target location
            if (Vector2.Distance(transform.position, targetLocation) < 0.1f)
            {
                hasReachedTargetLocation = true;
                StartCoroutine(SpawnObjectPeriodically()); // Start spawning objects after reaching the target location
            }
        }
    }

    private void SetRandomTargetLocation()
    {
        Transform playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        float randomDistance = Random.Range(minDistance, maxDistance);
        float randomAngle = Random.Range(0, 2 * Mathf.PI);
        Vector2 randomOffset = new Vector2(Mathf.Cos(randomAngle), Mathf.Sin(randomAngle)) * randomDistance;
        targetLocation = (Vector2)playerTransform.position + randomOffset;
    }

    private IEnumerator ChargeAndAttack(Transform playerTransform)
    {
        isCharging = true;

        // Store the initial position before charging
        initialPosition = rb.position;

        Vector2 playerPosition = playerTransform.position;

        // Calculate the direction to the player
        Vector2 chargeDirection = (playerPosition - rb.position).normalized;

        // Apply force to charge towards the player
        rb.AddForce(chargeDirection * chargeForce, ForceMode2D.Impulse);

        // Wait until the enemy reaches the player or some maximum time has passed
        float chargeTime = 1f; // Maximum time to charge
        float elapsedTime = 0f;

        while (elapsedTime < chargeTime && Vector2.Distance(rb.position, playerPosition) > 0.1f)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Stop the charge
        rb.velocity = Vector2.zero;

        // Apply damage to the player if close enough
        if (Vector2.Distance(transform.position, playerPosition) <= 0.5f) // Adjust the distance threshold as needed
        {
            playerTransform.GetComponent<player_controller>().TakeDam(damage);
        }

        // Fall back to the initial position
        Vector2 fallbackDirection = (initialPosition - rb.position).normalized;
        rb.AddForce(fallbackDirection * fallbackForce, ForceMode2D.Impulse);

        // Wait for the fallback duration
        yield return new WaitForSeconds(fallbackDuration);

        // Stop the fallback at the initial position
        rb.velocity = Vector2.zero;
        rb.position = initialPosition;

        nextAttackTime = Time.time + attackCooldown; // Set the next attack time
        isCharging = false;
    }

    private IEnumerator SpawnObjectPeriodically()
    {
        if (!isSpawning) // Ensure the coroutine runs only once
        {
            isSpawning = true;

            // Wait for 1 second before the first spawn
            yield return new WaitForSeconds(1f);

            while (true)
            {
                // Trigger the spawn animation
                animator.SetTrigger("SpawnObject");

                // Wait for the animation to finish
                yield return new WaitUntil(() => !animator.GetCurrentAnimatorStateInfo(0).IsName("enemy1spawnObject") && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);

                // Instantiate the spawn object
                Instantiate(spawnObject, transform.position, Quaternion.identity);

                // Wait for 5 seconds before the next spawn
                yield return new WaitForSeconds(5f);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerProjectile")) // Check for collision with projectile
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
        Transform playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        int randScoreBonus = Random.Range(100, 200);
        playerTransform.GetComponent<player_controller>().score += randScoreBonus;
        playerTransform.GetComponent<player_controller>().IncrementEnemiesDestroyed();

        // Instantiate death effect
        Instantiate(deathBloodEffect, transform.position, Quaternion.identity);

        // Destroy the enemy game object
        Destroy(gameObject);
    }

    // Draw Gizmos to visualize min and max distance and target location
    private void OnDrawGizmos()
    {
        Transform playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (playerTransform != null)
        {
            // Draw min and max distance circles
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(playerTransform.position, minDistance);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(playerTransform.position, maxDistance);

            // Draw attack range
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, attackRange);

            // Draw target location if it has been set
            if (targetLocation != Vector2.zero)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(targetLocation, 0.2f); // Draw a small sphere at the target location
                Gizmos.DrawLine(transform.position, targetLocation); // Draw a line from the enemy to the target location
            }
        }
    }
}
