using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.EventSystems;

public class PlayerScript : MonoBehaviour {

	private GameObject joystick;
	private Vector3 startPos;
	private GameObject jumpButton;
	private GameObject health;
	public float speed;
	public GameObject bulletPrefab;

	void Start ()
	{
		DontDestroyOnLoad (this);
		joystick = GameObject.Find ("MobileJoystick");
		jumpButton = GameObject.Find ("JumpButton");
		health = GameObject.FindWithTag ("Health");

		if (joystick) {
			startPos = joystick.transform.position;
		}

		if (jumpButton) {
			EventTrigger eventTrigger = jumpButton.GetComponent<EventTrigger>();
			EventTrigger.Entry entry = new EventTrigger.Entry();
			entry.eventID = EventTriggerType.PointerDown;
			entry.callback = new EventTrigger.TriggerEvent();
			UnityEngine.Events.UnityAction<BaseEventData> callback =
				new UnityEngine.Events.UnityAction<BaseEventData>(FireBullet);
			entry.callback.AddListener(callback);
			eventTrigger.delegates.Add(entry);
		}
	}

	// Rotate player in sync with joystick and move in that direction
	void FixedUpdate ()
	{
		if (joystick) {
			Vector3 diff = joystick.transform.position - startPos;
			diff.Normalize();
			float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
			var pos = this.transform.position;
			
			if((Mathf.Abs (diff.x) > 0.1 || Mathf.Abs (diff.y) > 0.1) && GetComponent<NetworkView>().isMine) {
				transform.rotation = Quaternion.Euler(0f, 0f, rot_z - 90);
				GetComponent<Rigidbody2D>().AddForce(gameObject.transform.up * speed);
			}
			// Prevent the player from going off screen
			pos.x = Mathf.Clamp(this.transform.position.x, 25, Screen.width - 25);
			pos.y = Mathf.Clamp(this.transform.position.y, 25, Screen.height - 25);
			this.transform.position = pos;
		}
	}

	// Fire bullet in direction of player
	public void FireBullet(UnityEngine.EventSystems.BaseEventData baseEvent)
	{
		if (GetComponent<NetworkView> ().isMine) {
			Network.Instantiate (bulletPrefab, transform.position, transform.rotation, 0);
		}
	}

	public void OnTriggerEnter2D(Collider2D collisionInfo)
	{
		if (collisionInfo.gameObject.tag == "Enemy" && GetComponent<NetworkView> ().isMine) {
			// Subtract health from player
			float newHealth = health.GetComponent<HealthBar>().SubtractHealth (0.25f);
			if (newHealth <= 0) {
				EventTrigger eventTrigger = jumpButton.GetComponent<EventTrigger>();
				eventTrigger.enabled = false;
				Network.Destroy (transform.gameObject);
			}
		}
	}
}
