using UnityEngine;
using System.Collections;

public class EnemyManager : MonoBehaviour {

	//public HealthBar playerHealth;	//TODO: For multiplayer, which player's health???  
	public GameObject enemy;
	public float spawnTime = 3f;
	//public Transform[] spawnPoints;
	public int enemyCount = 0;
	
	void Start()
	{
		// Call the Spawn function after a delay of the spawnTime and then continue to call after the same amount of time.
		InvokeRepeating ("Spawn", spawnTime, spawnTime);
	}
	
	void Spawn ()		
	{
		// If the player has no health left, exit the function;
		/*if (playerHealth.currentHealth <= 0f)  //TODO: Implement this when the player's health is known
			return;*/

		// Limit the number of enemies that can spawn
		if (enemyCount == 5)
			return;

		// Find a random index between zero and one less than the number of spawn points. 
		//int spawnPointIndex = Random.Range (0, spawnPoints.Length);
		var pos = transform.position;
		var rot = transform.rotation;

		pos.x = Random.Range (85, Screen.width);
		pos.y = Screen.height-50;


		//Create an instance of the enemy prefab at the randomly selected spawn point's position and rotation.
		Network.Instantiate (enemy, pos, rot, 0);
		enemyCount++; 
	}


}
