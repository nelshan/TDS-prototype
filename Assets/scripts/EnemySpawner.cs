/*:
Wave 1: Only element 0 is spawned.
Wave 2: element 0 and element 1 are spawned.
Wave 3: element 0, element 1, and element 2 are spawned.
Wave 4: element 0, element 1, element 2, and element 3 are spawned.
Wave 5: element 1, element 2, element 3, and element 4 are spawned.

(element 0(Enemy1) max number from 100 to 200 in all waves)
(element 1(Enemy2) max number from 10 to 15 in all waves)
(element 2(Enemy3) max number from 5 to 10 in all waves)
(element 3(Enemy4) max number from 7 to 20 in all waves)
(element 4(Enemy5) max number from 7 to 10 in all waves)

*/

using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] enemies; // Array to hold the different enemy prefabs
    [SerializeField] private float spawnInterval = 2f; // Time interval between spawns

    [SerializeField] private Vector2 spawnBoxSize = new Vector2(20f, 20f); // Size of the spawn box
    [SerializeField] private Vector2 spawnBoxPosition = new Vector2(0f, 0f); // Position of the spawn box
    [SerializeField] private Camera mainCamera; // Reference to the main camera
    [SerializeField] private Color gizmoColor = Color.red; // Color for the gizmo
    [SerializeField] private float waveDuration = 180f; // Duration of each wave in seconds (3 minutes)

    private int currentWave = 1; // Tracks the current wave
    private int totalWaves = 5; // Total number of waves

    // Max numbers of enemies for each element
    private int[] maxEnemiesPerElement = { 0, 0, 0, 0, 0 };

    private void Start()
    {
        StartCoroutine(SpawnWaves());
    }

    private IEnumerator SpawnWaves()
    {
        // Loop through each wave
        while (true) // Run indefinitely until player dies
        {
            // Log the start of the wave
            Debug.Log("Starting Wave " + currentWave);

            // Determine the max enemies for each element based on the wave
            SetMaxEnemiesForWave(currentWave);

            // Spawn enemies for the current wave
            StartCoroutine(SpawnEnemies(currentWave));

            // Wait for the wave duration
            yield return new WaitForSeconds(waveDuration);

            // Log the completion of the wave
            Debug.Log("Wave " + currentWave + " completed!");

            // Move to the next wave if below wave 5
            if (currentWave < totalWaves)
            {
                currentWave++;
            }
        }
    }

    private IEnumerator SpawnEnemies(int wave)
    {
        // Calculate end time of the current wave
        float waveEndTime = Time.time + waveDuration;

        // Array to track how many enemies have been spawned per element
        int[] enemiesSpawned = new int[enemies.Length];

        while (Time.time < waveEndTime)
        {
            Vector3 spawnPosition = GetValidSpawnPosition();

            if (spawnPosition != Vector3.zero)
            {
                // Spawn enemies based on the current wave number
                int enemyCount = Mathf.Min(wave, enemies.Length); // Ensure enemy index does not exceed array length
                for (int i = 0; i < enemyCount; i++)
                {
                    // Check if we have reached the maximum number of allowed enemies for this element
                    if (enemiesSpawned[i] < maxEnemiesPerElement[i])
                    {
                        Instantiate(enemies[i], spawnPosition, Quaternion.identity);
                        enemiesSpawned[i]++;
                    }
                }
            }

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SetMaxEnemiesForWave(int wave)
    {
        switch (wave)
        {
            case 1:
                maxEnemiesPerElement[0] = Random.Range(100, 201); // Element 0: 100-200
                break;
            case 2:
                maxEnemiesPerElement[0] = Random.Range(100, 201); // Element 0: 100-200
                maxEnemiesPerElement[1] = Random.Range(10, 16);   // Element 1: 10-15
                break;
            case 3:
                maxEnemiesPerElement[0] = Random.Range(100, 201); // Element 0: 100-200
                maxEnemiesPerElement[1] = Random.Range(10, 16);   // Element 1: 10-15
                maxEnemiesPerElement[2] = Random.Range(5, 11);    // Element 2: 5-10
                break;
            case 4:
                maxEnemiesPerElement[0] = Random.Range(100, 201); // Element 0: 100-200
                maxEnemiesPerElement[1] = Random.Range(10, 16);   // Element 1: 10-15
                maxEnemiesPerElement[2] = Random.Range(5, 11);    // Element 2: 5-10
                maxEnemiesPerElement[3] = Random.Range(7, 21);    // Element 3: 7-20
                break;
            case 5:
                maxEnemiesPerElement[0] = Random.Range(50, 101); // Element 0: 50-100
                maxEnemiesPerElement[1] = Random.Range(10, 16);   // Element 1: 10-15
                maxEnemiesPerElement[2] = Random.Range(5, 11);    // Element 2: 5-10
                maxEnemiesPerElement[3] = Random.Range(7, 21);    // Element 3: 7-20
                maxEnemiesPerElement[4] = Random.Range(7, 11);    // Element 4: 7-10
                break;
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
