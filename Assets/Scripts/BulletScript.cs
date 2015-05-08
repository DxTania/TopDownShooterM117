using UnityEngine;
using System.Collections;

public class BulletScript : MonoBehaviour {
	public float bulletSpeed;

	void Start ()
	{
		GetComponent<Rigidbody2D>().GetComponent<Rigidbody2D> ()
			.AddForce (transform.up * bulletSpeed);
	}

	// Destroy bullet once it's off screen
	void OnBecameInvisible ()
	{
		if (GetComponent<NetworkView> ().isMine) {
			Network.Destroy (gameObject);
		}
	}
}
