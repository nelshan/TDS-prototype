using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class player_controller : MonoBehaviour
{
    [SerializeField] private float MovementSpeed = 5f;
    [SerializeField] private float DashSpeed = 20f;
    [SerializeField] private float DashDuration = 0.2f;
    private Vector2 movement;
    private Rigidbody2D PlayerRB;
    private Animator animator;

    public int score;
    [SerializeField] private TextMeshProUGUI scoreDisplay;

    [SerializeField] private int health;
    [SerializeField] private int maxHealth;
    [SerializeField] private Image playerHealthBar;

    private float safeTime;
    [SerializeField] private float startSafeTime;
    [SerializeField] private GameObject losePanel;
    public bool isDead;
    public GameObject deathBooldEffect; // Death effect with particle effect and sound

    [SerializeField] private TextMeshProUGUI enemiesDestroyedDisplay;
    [SerializeField] private int enemiesDestroyed;

    private bool isDashing = false;
    private float dashTime;

    [SerializeField] private float healPercentage = 0.2f; // 20% for heal percentage

    void Awake()
    {
        PlayerRB = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        health = maxHealth; // Set initial health
        UpdateHealthBar(); // Initialize the health bar
        enemiesDestroyed = 0; // Initialize the enemiesDestroyed counter
        UpdateEnemiesDestroyedDisplay(); // Initialize the display
    }

    void Update()
    {
        scoreDisplay.text = score.ToString();

        if (isDashing)
        {
            if (Time.time >= dashTime)
            {
                isDashing = false;
                PlayerRB.velocity = Vector2.zero; // Stop the dash
            }
            return; // Skip the regular movement if dashing
        }

        movement.Set(PlayerInputManager.Movement.x, PlayerInputManager.Movement.y);
        PlayerRB.velocity = movement * MovementSpeed;
        bool isRunning = movement != Vector2.zero;
        animator.SetBool("isRunning", isRunning);

        if (safeTime > 0)
        {
            safeTime -= Time.deltaTime;
        }

        if (health <= 0)
        {
            losePanel.SetActive(true);
            AudioManager.Instance.PlaySFX("Game over sfx");
            isDead = true;
            HandleDeath();
        }

        // Check for dash input
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartDash();
        }
    }

    private void StartDash()
    {
        if (movement != Vector2.zero)
        {
            isDashing = true;
            dashTime = Time.time + DashDuration;

            PlayerRB.velocity = movement.normalized * DashSpeed; // Set dash velocity
            
            CinemachineShake.Instance.ShakeCamera(5f, 0.1f); //shake the VirtualCamera intensity and shakeTime

            AudioManager.Instance.PlaySFX("player dash sfx");
        }
    }

    public void TakeDam(int dam)
    {
        if (safeTime <= 0)
        {
            health -= dam;
            UpdateHealthBar();
            safeTime = startSafeTime;
        }
    }

    public void UpdateHealthBar()
    {
        playerHealthBar.fillAmount = (float)health / maxHealth; // Normalize health value between 0 and 1
    }

    public void IncrementEnemiesDestroyed()
    {
        enemiesDestroyed++;
        UpdateEnemiesDestroyedDisplay();
    }

    private void UpdateEnemiesDestroyedDisplay()
    {
        enemiesDestroyedDisplay.text = enemiesDestroyed.ToString();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Heal"))
        {
            HealPlayer();
            AudioManager.Instance.PlaySFX("player heal sfx");
            Destroy(collision.gameObject); // Destroy the heal game object
        }
    }

    // Method to heal the player by a percentage of maxHealth
    private void HealPlayer()
    {
        int healAmount = Mathf.RoundToInt(maxHealth * healPercentage); // Calculate the heal amount based on the heal percentage
        health = Mathf.Min(health + healAmount, maxHealth); // Increase the player's health but do not exceed maxHealth
        UpdateHealthBar(); // Update the health bar to reflect the new health value
    }

    private void HandleDeath()
    {
        // Instantiate death effect
        Instantiate(deathBooldEffect, transform.position, Quaternion.identity);
        
        // Deactivate the player game object
        gameObject.SetActive(false);

        Time.timeScale = 0f;
    }
}
