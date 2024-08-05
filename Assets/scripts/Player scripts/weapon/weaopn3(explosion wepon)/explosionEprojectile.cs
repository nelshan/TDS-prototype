using UnityEngine;

public class explosionEprojectile : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private GameObject explosionEffect; // Explosion effect with particle effect and sound

    private void Start()
    {
        AudioManager.Instance.PlaySFX("weapon3 shot sound");
    }

    private void Update()
    {
        transform.Translate(Vector2.right * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy") || other.CompareTag("Obstacle"))
        { 
            // Instantiate explosion effect at the projectile's position
            Instantiate(explosionEffect, transform.position, Quaternion.identity);

            // Destroy the projectile game object
            Destroy(gameObject);
        }
    }
}
