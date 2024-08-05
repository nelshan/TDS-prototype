using UnityEngine;
using System.Collections;

public class Enemy2Projectile : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float lifespan = 5f; // Time in seconds before the projectile is destroyed

    private Transform player;
    private Vector2 direction;
    private bool passedPlayer; // Flag to indicate if the projectile has passed the player
    private float creationTime;

    //public GameObject effect;

    public void Initialize(Transform playerTransform)
    {
        player = playerTransform;
        direction = (player.position - transform.position).normalized;
        passedPlayer = false;
        creationTime = Time.time; // Record the creation time
    }

    private void Update()
    {
        // Update the direction towards the player's current position
        direction = (player.position - transform.position).normalized;

        // Move towards the player until the projectile passes the player
        if (!passedPlayer)
        {
            transform.position = (Vector2)transform.position + direction * speed * Time.deltaTime;

            // Check if the projectile has passed the player
            if (Vector2.Dot((Vector2)(transform.position - player.position), direction) > 0)
            {
                passedPlayer = true;
            }
        }
        else
        {
            // If passed the player, continue moving in the same direction until lifespan expires
            transform.position = (Vector2)transform.position + direction * speed * Time.deltaTime;
        }

        // Destroy the projectile after the lifespan expires
        if (Time.time >= creationTime + lifespan)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<player_controller>().TakeDam(1);

            CinemachineShake.Instance.ShakeCamera(5f, 0.1f); // Shake the VirtualCamera with intensity and shakeTime

            AudioManager.Instance.PlaySFX("player damage sfx");
            
            Destroy(gameObject);
        }
    }
}
