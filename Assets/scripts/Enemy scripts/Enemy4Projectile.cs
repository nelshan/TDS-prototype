using UnityEngine;
using System.Collections;

public class Enemy4Projectile : MonoBehaviour
{
    [SerializeField] private int damage = 10; // Default damage dealt by the projectile
    [SerializeField] private float lifespan = 5f; // Time in seconds before the projectile is destroyed
    private float creationTime;
    [SerializeField] private GameObject ParticleSystemPrefab; // Prefab for the fireball explosion

    public void Initialize()
    {
        creationTime = Time.time; // Record the creation time
    }

    private void Update()
    {
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
            player_controller player = other.GetComponent<player_controller>();
            if (player != null)
            {
                player.TakeDam(damage);
            }

            // Instantiate the explosion at the projectile's position
            if (ParticleSystemPrefab != null)
            {
                Instantiate(ParticleSystemPrefab, transform.position, Quaternion.identity);
            }

            CinemachineShake.Instance.ShakeCamera(5f, 0.1f); // Shake the VirtualCamera intensity and shakeTime

            // Play the hit sound
            AudioManager.Instance.PlaySFX("player damage sfx");

            Destroy(gameObject);
        }
        else if (other.CompareTag("Obstacle"))
        {
            // Instantiate the explosion at the projectile's position
            if (ParticleSystemPrefab != null)
            {
                Instantiate(ParticleSystemPrefab, transform.position, Quaternion.identity);
            }

            // Destroy the projectile upon hitting the obstacle
            Destroy(gameObject);
        }
    }
}
