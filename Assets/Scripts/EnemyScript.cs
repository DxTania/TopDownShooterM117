using UnityEngine;
using System.Collections;

public class EnemyScript : MonoBehaviour {

	public float speed;
	private GameObject [] players;
	Transform playerPosition;

	void Start() {
		DontDestroyOnLoad (this);
	}

	void FixedUpdate () {
		// Follow closest player
		players = GameObject.FindGameObjectsWithTag ("Player");

		if (players.Length == 2) {
			var player1dist = (this.transform.position - players [0].transform.transform.position).sqrMagnitude;
			var player2dist = (this.transform.position - players [1].transform.transform.position).sqrMagnitude;
			
			playerPosition = player1dist > player2dist ? players [1].transform : players [0].transform;
			
			var z = Mathf.Atan2 (playerPosition.transform.position.y - transform.position.y,
			                     playerPosition.transform.position.x - transform.position.x)
				* Mathf.Rad2Deg - 90;
			
			transform.eulerAngles = new Vector3 (0, 0, z);
			GetComponent<Rigidbody2D> ().AddForce (gameObject.transform.up * speed);
		}
	}
}
