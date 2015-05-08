using UnityEngine;
using System.Collections;

public class ExplosionScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		if (GetComponent<NetworkView>().isMine) {
			StartCoroutine (MyMethod ());
		}
	}
	
	// Update is called once per frame
	void Update () {
		transform.Rotate (0, 0, 1000 * Time.deltaTime);
	}

	IEnumerator MyMethod() {
		yield return new WaitForSeconds(0.1f);
		Network.Destroy (transform.gameObject);
	}
}
