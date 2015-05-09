using UnityEngine;
using System.Collections;
using System;

public class EnemyScript : MonoBehaviour {

	public float speed;
	public GameObject explosionPrefab;
	private EnemyManager enemyManager;
	private GameObject [] players;
	private Transform playerPosition;

	void Start()
	{
		DontDestroyOnLoad (this);

		if (GetComponent<NetworkView> ().isMine) {
			enemyManager = GameObject.Find ("EnemySpawnPointManager").GetComponent<EnemyManager> ();
		}
	}

	void FixedUpdate ()
	{
		players = GameObject.FindGameObjectsWithTag ("Player");

		// Follow closest player
		float closestDist = Single.PositiveInfinity;
		for (var i = 0; i < players.Length; i++) {
			var dist = (this.transform.position -
			            players [i].transform.transform.position).sqrMagnitude;
			if (dist < closestDist) {
				closestDist = dist;
				playerPosition = players[i].transform;
			}
		}

		if (playerPosition) {
			var z = Mathf.Atan2 (playerPosition.transform.position.y - transform.position.y,
			                     playerPosition.transform.position.x - transform.position.x)
				* Mathf.Rad2Deg - 90;
			
			transform.eulerAngles = new Vector3 (0, 0, z);
			GetComponent<Rigidbody2D> ().AddForce (gameObject.transform.up * speed);
		}
	}

	public void OnTriggerEnter2D(Collider2D collisionInfo)
	{
		if ((collisionInfo.gameObject.tag == "Player" ||
		     collisionInfo.gameObject.tag == "Bullet") &&
		    GetComponent<NetworkView> ().isMine) {
			// Explode enemy on collision
			Network.Instantiate (explosionPrefab, transform.position, transform.rotation, 0);
			Network.Destroy (transform.gameObject);

			// Update enemy count
			enemyManager.EnemyDestroyed ();
		}
	}
}
