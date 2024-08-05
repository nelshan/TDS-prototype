using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class miniboss2 : MonoBehaviour
{
    [SerializeField] private float speed;
    private Transform target;

    [SerializeField] private int health;
    [SerializeField] private int maxHealth;
    [SerializeField] private Slider minibossHealthBar;

    private Animator animator;

    [SerializeField] private float attackRange = 5f; // Range within which the enemy starts the laser attack
    [SerializeField] private float attackCooldown = 3f; // Time between attacks
    private bool isAttacking = false;

    [SerializeField] private Transform ShockwaveSpawnPosition; // Variable for laser attack spawn position
    [SerializeField] private GameObject ShockwaveAtkPrefab; // Prefab for the laser attack anchor point

    [SerializeField] private GameObject ShockwaveATKIndicatorCircle; // Warning sprite GameObject

    [SerializeField] private GameObject deathBooldEffect; // Death effect with particle effect and sound

    private void Start()
    {
        
        health = maxHealth; // Set initial health

        target = GameObject.FindGameObjectWithTag("Player").transform;
        animator = GetComponentInChildren<Animator>();

        // Ensure the attack indicator is initially inactive
        ShockwaveATKIndicatorCircle.SetActive(false);
    }

    private void Update()
    {
        if (health <= 0)
        {
            HandleDeath(); // Handle the death and effects
            return; // Exit the Update method
        }

        float distanceToPlayer = Vector2.Distance(transform.position, target.position);

        // If the player is within attack range and not currently attacking, initiate the attack
        if (distanceToPlayer <= attackRange && !isAttacking)
        {
            StartCoroutine(InitiateAttack());
        }
        else if (distanceToPlayer > attackRange && !isAttacking)
        {
            MoveTowardsPlayer();
        }
    }

    private void MoveTowardsPlayer()
    {
        // Move the miniboss towards the player's position
        Vector2 newPosition = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
        transform.position = newPosition; // Apply the final new position
    }

    private IEnumerator InitiateAttack()
    {
        isAttacking = true;

        // Activate the attack indicator
        ShockwaveATKIndicatorCircle.SetActive(true);

        // Wait for 0.5 second before initiating the attack
        yield return new WaitForSeconds(0.8f);

        // Deactivate the attack indicator
        ShockwaveATKIndicatorCircle.SetActive(false);

        // Spawn the shockwave attack prefab at the designated spawn position
        Instantiate(ShockwaveAtkPrefab, ShockwaveSpawnPosition.position, Quaternion.identity);

        // Wait for the attack cooldown duration before allowing another attack
        yield return new WaitForSeconds(attackCooldown);

        // Reset the attacking flag
        isAttacking = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerProjectile")) // Check for collision with projectile
        {
            TakeDamage(other.GetComponent<Projectile>().damage);
        }
        else if (other.CompareTag("PlayerexplosionProjectile")) // Check for collision with explosion projectile
        {
            TakeDamage(other.GetComponent<ExplosionBullet2>().damage);
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        UpdateHealthBar();

        // animator.SetTrigger("Damage"); // Play enemy damage animation
        if (health <= 0)
        {
            HandleDeath(); // Handle the death and effects
        }
    }

    public void UpdateHealthBar()
    {
        minibossHealthBar.value = (float)health / maxHealth; // Normalize health value between 0 and 1
    }

    private void HandleDeath()
    {
        // Award score and increment enemies destroyed
        int randScoreBonus = Random.Range(100, 200);
        target.GetComponent<player_controller>().score += randScoreBonus;
        target.GetComponent<player_controller>().IncrementEnemiesDestroyed();

        // Instantiate death effect
        Instantiate(deathBooldEffect, transform.position, Quaternion.identity);

        // Destroy the enemy game object
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        // Draw a red sphere at the transform's position to visualize the attack range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
