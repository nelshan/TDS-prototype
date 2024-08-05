using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed;
	public int damage = 1;

	private void Start()
	{
		AudioManager.Instance.PlaySFX("weapon shot sound sfx");
	}
	private void Update()
	{
		transform.Translate(Vector2.right * speed * Time.deltaTime);
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if(other.CompareTag("Enemy")){ 
			Enemy enemy = other.GetComponent<Enemy>();
			if (enemy != null)
            {
				enemy.TakeDamage(damage);
				Destroy(gameObject);
			}
		}
	}
}