using UnityEngine;

public class Spawner : MonoBehaviour
{
    public float startTimeBtwSpawn;
    private float timeBtwSpawn;

    public GameObject[] enemies;
	public Transform spawnPos;

	private player_controller player;

	private void Start()
	{
		player = FindObjectOfType<player_controller>();
	}

	private void Update()
	{
		if(timeBtwSpawn <= 0){ 
			int randEnemy = Random.Range(0, enemies.Length);
			Instantiate(enemies[randEnemy], spawnPos.position, Quaternion.identity);
			timeBtwSpawn = startTimeBtwSpawn;
		} else{ 
			timeBtwSpawn -= Time.deltaTime;		
		}
	}
}