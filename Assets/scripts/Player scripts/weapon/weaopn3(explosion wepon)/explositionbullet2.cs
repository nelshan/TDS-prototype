using System.Collections;
using UnityEngine;

public class ExplosionBullet2 : MonoBehaviour
{
    public int damage = 1;
    private Animator animator;

    private void Start()
    {
        AudioManager.Instance.PlaySFX("weapon3 explosion-1");
        AudioManager.Instance.PlaySFX("weapon3 explosion-2");
        animator = GetComponent<Animator>();
        StartCoroutine(DestroyAfterAnimation());
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }

            Enemy2 enemy2 = other.GetComponent<Enemy2>();
            if (enemy2 != null)
            {
                enemy2.TakeDamage(damage);
            }

            Enemy3 enemy3 = other.GetComponent<Enemy3>();
            if (enemy3 != null)
            {
                enemy3.TakeDamage(damage);
            }

            Enemy4 enemy4 = other.GetComponent<Enemy4>();
            if (enemy4 != null)
            {
                enemy4.TakeDamage(damage);
            }

            Enemy5 enemy5 = other.GetComponent<Enemy5>();
            if (enemy5 != null)
            {
                enemy5.TakeDamage(damage);
            }
        }
        else if (other.CompareTag("Player"))
        {
            player_controller player = other.GetComponent<player_controller>();
            if (player != null)
            {
                player.TakeDam(damage);
            }
        }
    }

    private IEnumerator DestroyAfterAnimation()
    {
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        Destroy(gameObject);
    }
}
