using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class player_controller : MonoBehaviour
{
    [SerializeField] public float MovementSpeed = 5f;
    private Vector2 movement;
    private Rigidbody2D PlayerRB;
    private Animator animator;

    public int score;
    public TextMeshProUGUI scoreDisplay;

    public int health;
    public int maxHealth;
    [SerializeField] private Image playerHealthBar;

    private float safeTime;
    public float startSafeTime;
    public GameObject losePanel;
    public bool isDead;

    public TextMeshProUGUI enemiesDestroyedDisplay;
    private int enemiesDestroyed;


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
            isDead = true;
            //HandleDeath();
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
        // Normalize health value between 0 and 1
        playerHealthBar.fillAmount = (float)health / maxHealth;
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
    /*private void HandleDeath()
    {
        losePanel.SetActive(true);
        isDead = true;
        // Optionally, stop all player movements and disable controls
        PlayerRB.velocity = Vector2.zero;
        PlayerInputManager.enabled = false;
        // Play death animation or sound if available
        // animator.SetTrigger("Death");
    }*/
}
