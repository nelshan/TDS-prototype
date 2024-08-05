using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner2 : MonoBehaviour
{
    public GameObject[] enemies; // Array to hold the different enemy prefabs
    public float spawnInterval = 2f; // Time interval between spawns
    public Vector2 spawnBoxSize = new Vector2(20f, 20f); // Size of the spawn box
    public Vector2 spawnBoxPosition = new Vector2(0f, 0f); // Position of the spawn box
    public Camera mainCamera; // Reference to the main camera
    public Color gizmoColor = Color.red; // Color for the gizmo

    [SerializeField] private int totalEnemiesSpawned = 0; // Counter for the total number of enemies spawned

    private void Start()
    {
        StartCoroutine(SpawnEnemies());
    }

    private IEnumerator SpawnEnemies()
    {
        // Infinite loop to keep spawning enemies
        while (true)
        {
            Vector3 spawnPosition = GetValidSpawnPosition();

            if (spawnPosition != Vector3.zero)
            {
                // Select a random enemy from the array
                int enemyIndex = Random.Range(0, enemies.Length);
                Instantiate(enemies[enemyIndex], spawnPosition, Quaternion.identity);

                // Increment the total enemies spawned counter
                totalEnemiesSpawned++;

                // Log the total number of enemies spawned
                Debug.Log("Total Enemies Spawned: " + totalEnemiesSpawned);
            }

            // Wait for the next spawn interval
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private Vector3 GetValidSpawnPosition()
    {
        Vector3 spawnPosition = Vector3.zero;
        bool validPosition = false;

        while (!validPosition)
        {
            // Get random position within the box
            float randomX = Random.Range(spawnBoxPosition.x - spawnBoxSize.x / 2, spawnBoxPosition.x + spawnBoxSize.x / 2);
            float randomY = Random.Range(spawnBoxPosition.y - spawnBoxSize.y / 2, spawnBoxPosition.y + spawnBoxSize.y / 2);
            spawnPosition = new Vector3(randomX, randomY, 0f);

            // Check if the spawn position is outside the camera view
            Vector3 viewportPoint = mainCamera.WorldToViewportPoint(spawnPosition);

            if (viewportPoint.x < 0 || viewportPoint.x > 1 || viewportPoint.y < 0 || viewportPoint.y > 1)
            {
                validPosition = true;
            }
        }

        return spawnPosition;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;
        Gizmos.DrawWireCube(spawnBoxPosition, spawnBoxSize);
    }
}
