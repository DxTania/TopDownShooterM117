using UnityEngine;
using System.Collections;

public class EnemyManager : MonoBehaviour {

	public GameObject enemyPrefab;
	public float spawnTime;
	public int maxEnemyCount;
	private int enemyCount;

	void Start()
	{
		// Consistently call the Spawn function after spawnTime delay
		if (Network.isServer) {
			InvokeRepeating ("Spawn", spawnTime, spawnTime);
		}
	}

	void Spawn ()		
	{
		// Limit the number of enemies that can spawn
		if (enemyCount < maxEnemyCount) {
			// Find a random index between zero and one less than the number of spawn points. 
			//int spawnPointIndex = Random.Range (0, spawnPoints.Length);
			var pos = transform.position;
			var rot = transform.rotation;

			pos.x = Random.Range (85, Screen.width);
			pos.y = Screen.height - 50;

			// Create an instance of the enemy prefab at the random position and rotation
			Network.Instantiate (enemyPrefab, pos, rot, 0);
			enemyCount++; 
		}
	}

	// Called by enemies when they collide with player
	public void EnemyDestroyed ()
	{
		enemyCount--;
	}
}
