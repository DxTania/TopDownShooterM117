using UnityEngine;
using System.Collections;
using System;

public class EnemyScript : MonoBehaviour {

	public float speed;
	public GameObject explosionPrefab;
	private GameObject [] players;
	private Transform playerPosition;

	void Start() {
		DontDestroyOnLoad (this);
	}

	void FixedUpdate () {
		// Follow closest player
		players = GameObject.FindGameObjectsWithTag ("Player");

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

	// Explode enemy on collision
	public void OnCollisionEnter2D(Collision2D collisionInfo) {
		Network.Instantiate (explosionPrefab, transform.position, transform.rotation, 0);
		Destroy (transform.gameObject);
		// TODO: subtract health from player collided with
	}
}
