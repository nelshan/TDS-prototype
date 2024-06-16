using UnityEngine;
using System.Collections;

public class LaserAttack : MonoBehaviour
{
    private int damage;
    private Transform target;
    [SerializeField] private float waitDestroyTimeDuration = 0.5f; // Duration to wait to destroy gameObject after the laser attack animation

    public void Initialize(int damage, Transform target)
    {
        this.damage = damage;
        this.target = target;
        StartCoroutine(PerformLaserAttack());
    }

    private IEnumerator PerformLaserAttack()
    {
        // Wait for the attack duration
        yield return new WaitForSeconds(waitDestroyTimeDuration);

        // Check if the parent exists before destroying
        if (transform.parent != null)
        {
            Destroy(transform.parent.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<player_controller>().TakeDam(damage);
        }
    }
}
