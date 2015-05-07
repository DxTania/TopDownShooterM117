using UnityEngine;
using System.Collections;

public class EnemyScript : MonoBehaviour {

	public float speed;
	//public Transform player;
	GameObject [] player;
	Transform playerPosition;

	void Awake()
	{
		player = GameObject.FindGameObjectsWithTag ("Player");
		playerPosition = player[0].transform;  //TODO: CHANGE THIS. Only works for one player for now.
	}

	//Follow only one player so far. TODO: Implement so that plane will follow closest player
	void FixedUpdate () {
		var z = Mathf.Atan2 (playerPosition.transform.position.y - transform.position.y,
		                     playerPosition.transform.position.x - transform.position.x)
			* Mathf.Rad2Deg - 90;
		
		transform.eulerAngles = new Vector3 (0, 0, z);
		GetComponent<Rigidbody2D> ().AddForce (gameObject.transform.up * speed);
	}
	
	// Follow the player
	/*void FixedUpdate () {
		var z = Mathf.Atan2 (player.transform.position.y - transform.position.y,
		                     player.transform.position.x - transform.position.x)
					* Mathf.Rad2Deg - 90;

		transform.eulerAngles = new Vector3 (0, 0, z);
		GetComponent<Rigidbody2D> ().AddForce (gameObject.transform.up * speed);
	}*/
}
