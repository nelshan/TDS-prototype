using UnityEngine;

public class Enemy4Projectile : MonoBehaviour
{
    public int damage; // Damage dealt by the projectile
    public GameObject fireballExplosionPrefab; // Prefab for the fireball explosion

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Apply damage to the player
            other.GetComponent<player_controller>().TakeDam(damage);
            // Create the explosion effect
            CreateExplosion();
            // Destroy the projectile upon hitting the player
            Destroy(gameObject);
        }
        else if (other.CompareTag("Obstacle")) // Optional: Destroy the projectile if it hits an obstacle
        {
            // Create the explosion effect
            CreateExplosion();
            // Destroy the projectile upon hitting the obstacle
            Destroy(gameObject);
        }
    }

    private void CreateExplosion()
    {
        // Instantiate the explosion at the projectile's position
        GameObject explosion = Instantiate(fireballExplosionPrefab, transform.position, Quaternion.identity);

        // Optional: Destroy the explosion after the animation completes
        Animator explosionAnimator = explosion.GetComponent<Animator>();
        float explosionDuration = explosionAnimator.GetCurrentAnimatorStateInfo(0).length;
        Destroy(explosion, explosionDuration);
    }
}
