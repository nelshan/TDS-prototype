using UnityEngine;

public class destroyObject : MonoBehaviour
{
   [SerializeField] private float lifetime;

	private void Start()
	{
		Destroy(gameObject, lifetime);
	}
}
