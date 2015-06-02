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

	private byte [] buffer = new byte[1024];
	private byte [] buffer5 = new byte[5];
	private bool header;
	private int counter;
	private bool buffer5HasRead;

	private string line;
	private sbyte posX;
	private sbyte posY;
	private sbyte button;
	private sbyte parity;
	private int calibrateControllerCounter;
	private int posXCali;
	private int posYCali;
	private sbyte newStartPosX;
	private sbyte newStartPosY;

	private Vector3 newPos;

	void Start ()
	{
		DontDestroyOnLoad (this);
		joystick = GameObject.Find ("MobileJoystick");
		jumpButton = GameObject.Find ("JumpButton");
		health = GameObject.FindWithTag ("Health");
		header = false;
		counter = 0;
		buffer5HasRead = false;
		calibrateControllerCounter = 0;
		posXCali = 0;
		posYCali = 0;
		newStartPosX = 0;
		newStartPosY = 0;



			//A bluetooth controller is CONNECTED, don't use the virtual joystick and button
			if (BtConnector.isConnected ()) {


				buffer = BtConnector.readBuffer ();
				counter = 0;
				buffer5HasRead = false;
				for (int i = 0; i < buffer.Length; i++) {
					if (buffer [i] == 10 && counter == 0) {
						header = true;
					} else if (header == true) {
						buffer5 [counter] = buffer [i];
						counter++;
						if (counter == 5) {
							header = false;
							counter = 0;
							buffer5HasRead = true;
							break;
						}
					}
				}

				if (buffer5HasRead == true) {
					posX = (sbyte)buffer5 [1];
					posY = (sbyte)buffer5 [2];
					button = (sbyte)buffer5 [3];
					parity = (sbyte)buffer5 [4];
				}
				//startPos = new Vector3(10,-4, 0);		//TODO: Change the default startPos for the controller
				startPos = new Vector3 (-posX, posY, 0);		//TODO: Change the default startPos for the controller

			}
		//If a bluetooth controller is NOT CONNECTED, use the virtual joystick and button
		else {
				if (joystick) {
					startPos = joystick.transform.position;
				}

				if (jumpButton) {
					EventTrigger eventTrigger = jumpButton.GetComponent<EventTrigger> ();
					EventTrigger.Entry entry = new EventTrigger.Entry ();
					entry.eventID = EventTriggerType.PointerDown;
					entry.callback = new EventTrigger.TriggerEvent ();
					UnityEngine.Events.UnityAction<BaseEventData> callback =
				new UnityEngine.Events.UnityAction<BaseEventData> (FireBullet);
					entry.callback.AddListener (callback);
					eventTrigger.delegates.Add (entry);
				//eventTrigger.enabled = false;
				}
			} 
	
	}

	// Rotate player in sync with joystick and move in that direction
	void FixedUpdate ()
	{




			if (BtConnector.isConnected ()) {
				buffer = BtConnector.readBuffer ();
				counter = 0;
				buffer5HasRead = false;
				for (int i = 0; i < buffer.Length; i++) {
					if (buffer [i] == 10 && counter == 0) {
						header = true;
					} else if (header == true) {
						buffer5 [counter] = buffer [i];
						counter++;
						if (counter == 5) {
							header = false;
							counter = 0;
							buffer5HasRead = true;
							break;
						}
					}
				}
				//If successfully read, buffer5 contains the 5 bytes (excluding the newline) sent by the controller
				//buffer5[0] = 4 
				//buffer5[1] = p->x
				//buffer5[2] = p->y
				//buffer5[3] = p->buttons
				//buffer5[4] = p->parity
				if (buffer5HasRead == true) {
					posX = (sbyte)buffer5 [1];
					posY = (sbyte)buffer5 [2];
					button = (sbyte)buffer5 [3];
					parity = (sbyte)buffer5 [4];
				}

				//Eliminate any bad initial conditions introduced during the menu scene
				if (Mathf.Abs (startPos.x) > 5 || Mathf.Abs (startPos.y) > 5) {
					startPos = new Vector3 (-posX, posY, 0);
				}

				//Fire bullet if A (has a value of 0b00010) button is pressed
				if (button == 2) {
					FireBullet2 ();
					//button = 0;
				}

				newPos = new Vector3 (-posX, posY, 0);
				Vector3 diff = newPos - startPos;
				diff.Normalize ();
				float rot_z = Mathf.Atan2 (diff.y, diff.x) * Mathf.Rad2Deg;
				var pos = this.transform.position;

				if ((Mathf.Abs (diff.x) > 0.1 || Mathf.Abs (diff.y) > 0.1) && GetComponent<NetworkView> ().isMine) {
					transform.rotation = Quaternion.Euler (0f, 0f, rot_z - 90);
					GetComponent<Rigidbody2D> ().AddForce (gameObject.transform.up * speed);
				}

				// Prevent the player from going off screen
				if (pos.x < 25)
					pos.x = 25;
				else if (pos.x > Screen.width - 25)
					pos.x = Screen.width - 25;
			
				if (pos.y < 25)
					pos.y = 25;
				else if (pos.y > Screen.height - 25)
					pos.y = Screen.height - 25;
			
				this.transform.position = pos;
			} else {
				if (joystick) {
					Vector3 diff = joystick.transform.position - startPos;
					diff.Normalize ();
					float rot_z = Mathf.Atan2 (diff.y, diff.x) * Mathf.Rad2Deg;
					var pos = this.transform.position;
			
					if ((Mathf.Abs (diff.x) > 0.1 || Mathf.Abs (diff.y) > 0.1) && GetComponent<NetworkView> ().isMine) {
						transform.rotation = Quaternion.Euler (0f, 0f, rot_z - 90);
						GetComponent<Rigidbody2D> ().AddForce (gameObject.transform.up * speed);
					}
					// Prevent the player from going off screen
					if (pos.x < 25)
						pos.x = 25;
					else if (pos.x > Screen.width - 25)
						pos.x = Screen.width - 25;

					if (pos.y < 25)
						pos.y = 25;
					else if (pos.y > Screen.height - 25)
						pos.y = Screen.height - 25;

					this.transform.position = pos;
				}
			}

	}

	// Fire bullet in direction of player
	public void FireBullet(UnityEngine.EventSystems.BaseEventData baseEvent)
	{
		if (GetComponent<NetworkView> ().isMine) {
			Network.Instantiate (bulletPrefab, transform.position, transform.rotation, 0);
		}
	}

	public void FireBullet2()
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
				//BtConnector.stopListen();
			}
		}
	}
}
