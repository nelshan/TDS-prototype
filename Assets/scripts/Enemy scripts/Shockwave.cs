using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shockwave : MonoBehaviour
{
    [SerializeField] private int damage;

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<player_controller>().TakeDam(damage);
        }
    }

    // Method called by the Animation Event
    private void DestroyShockwave()
    {
        // Destroy the Shockwave game object
        Destroy(gameObject);
    }
}
