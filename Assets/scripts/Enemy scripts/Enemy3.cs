using UnityEngine;
using System.Collections;

public class Enemy3 : MonoBehaviour
{
    [SerializeField] private float speed;
    private Transform target;

    [SerializeField] private int damage;
    [SerializeField] private int health;
    [SerializeField] private float separationRadius = 0.5f; // Radius within which enemies will try to separate
    [SerializeField] private float separationForce = 0.5f; // Force with which enemies will push each other away

    private Animator animator;

    [SerializeField] private float attackRange = 5f; // Range within which the enemy starts the laser attack
    [SerializeField] private float attackCooldown = 3f; // Time between attacks
    private bool isAttacking;
    [SerializeField] private Transform laserSpawnPosition; // Variable for laser attack spawn position
    [SerializeField] private GameObject laserAtkPrefab; // Prefab for the laser attack anchor point
    [SerializeField] private int minLaserDamage = 10; // Minimum laser damage
    [SerializeField] private int maxLaserDamage = 20; // Maximum laser damage
    [SerializeField] private GameObject laserIndicatedWarning; // Warning sprite GameObject
    [SerializeField] private float laserWarningTime = 2f; // Duration for laser warning

    [SerializeField] private GameObject deathBooldEffect; // Death effect with particle effect and sound

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

        float distanceToPlayer = Vector2.Distance(transform.position, target.position);
        if (distanceToPlayer <= attackRange && !isAttacking)
        {
            StartCoroutine(StartLaserAttack());
        }
        else if (distanceToPlayer > attackRange && !isAttacking)
        {
            MoveTowardsPlayer();
        }
    }

    private void MoveTowardsPlayer()
    {
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
    }

    private IEnumerator StartLaserAttack()
    {
        isAttacking = true;

        // Position and rotate the warning sprite based on the player's position
        Vector3 direction = (target.position - laserSpawnPosition.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        laserIndicatedWarning.transform.position = laserSpawnPosition.position;
        laserIndicatedWarning.transform.rotation = Quaternion.Euler(0, 0, angle);
        laserIndicatedWarning.SetActive(true);

        // Play the laser warning sound
        AudioManager.Instance.PlaySFX("Enemy3 laserIndicatedWarning sfx");

        SpriteRenderer warningSpriteRenderer = laserIndicatedWarning.GetComponentInChildren<SpriteRenderer>();
        StartCoroutine(FadeWarningSprite(warningSpriteRenderer));

        yield return new WaitForSeconds(laserWarningTime); // Wait for the warning time before attacking

        // Deactivate the warning sprite
        laserIndicatedWarning.SetActive(false);

        // Start the laser attack
        int laserDamage = Random.Range(minLaserDamage, maxLaserDamage);
        GameObject laserAnchorInstance = Instantiate(laserAtkPrefab, laserSpawnPosition.position, Quaternion.Euler(0, 0, angle));
        laserAnchorInstance.GetComponent<LaserAttack>().Initialize(laserDamage, target);

        // Play the laser attack sound
        AudioManager.Instance.PlaySFX("Enemy3 laserAtk sfx");
        
        CinemachineShake.Instance.ShakeCamera(5f, 0.1f); // Shake the VirtualCamera with intensity and shakeTime

        yield return new WaitForSeconds(attackCooldown); // Cooldown before next attack
        isAttacking = false;
    }

    private IEnumerator FadeWarningSprite(SpriteRenderer spriteRenderer)
    {
        float halfTime = laserWarningTime / 2f;
        float fastFadeTime = halfTime / 4f;
        float elapsedTime = 0f;
        Color color = spriteRenderer.color;

        // Slow fade in and out
        while (elapsedTime < halfTime)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.PingPong(elapsedTime, halfTime) / halfTime;
            spriteRenderer.color = color;
            yield return null;
        }

        // Fast fade in and out
        elapsedTime = 0f;
        while (elapsedTime < fastFadeTime * 2)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.PingPong(elapsedTime, fastFadeTime) / fastFadeTime;
            spriteRenderer.color = color;
            yield return null;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Apply damage to the player
            other.GetComponent<player_controller>().TakeDam(damage);
            
            CinemachineShake.Instance.ShakeCamera(5f, 0.1f); // Shake the VirtualCamera with intensity and shakeTime

            // Play the collision sound
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

        // Instantiate death effect
        Instantiate(deathBooldEffect, transform.position, Quaternion.identity);

        itemDropManager.TryDropItem(transform.position);
 
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
