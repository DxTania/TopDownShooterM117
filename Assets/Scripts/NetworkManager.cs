using UnityEngine;
using System.Collections;

public class NetworkManager : MonoBehaviour {

	private HostData[] hostList;
	private const string typeName = "UCLA CSM117";
	private const string gameName = "TopDown";
	private bool gameStarted = false;
	public GameObject playerPrefab;
	public GameObject scorePrefab;

	void Start () {
		DontDestroyOnLoad (this);
		Application.LoadLevel ("menu");
		BtConnector.moduleName ("HC-06");
		//MasterServer.ipAddress = "149.142.250.42";
		/*MasterServer.ipAddress = "169.232.80.18";
		MasterServer.port = 23466;
		Network.natFacilitatorIP = "169.232.80.18";
		Network.natFacilitatorPort = 50005;*/
	}

	void Update () {
		GameObject[] players = GameObject.FindGameObjectsWithTag ("Player");
		if (gameStarted && Network.isServer && players.Length == 0) {
			Network.Destroy(GameObject.FindGameObjectWithTag("ScoreObject"));
			GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
			for (var i = 0; i < enemies.Length; i++) {
				Network.Destroy(enemies[i]);
			}
			Network.Disconnect();
			MasterServer.UnregisterHost();
			gameStarted = false;
		}
	}

	void OnPlayerDisconnected(NetworkPlayer player) {
		Debug.Log("Clean up after player " + player);
		Network.RemoveRPCs(player);
		Network.DestroyPlayerObjects(player);
	}

	void OnDisconnectedFromServer(NetworkDisconnection info) {
		Application.LoadLevel ("menu");
	}

	private void StartServer () {
		Network.InitializeServer(4, 25005, !Network.HavePublicAddress());
		MasterServer.RegisterHost(typeName, gameName);
	}

	private void JoinServer (HostData hostData) {
		Network.Connect(hostData);
	}

	private void RefreshHostList () {
		MasterServer.RequestHostList(typeName);
	}
	
	void OnMasterServerEvent (MasterServerEvent msEvent) {
		if (msEvent == MasterServerEvent.HostListReceived) {
			hostList = MasterServer.PollHostList ();
		}
	}

	void OnConnectedToServer () {
		Debug.Log("Server Joined");
		Application.LoadLevel ("game");
	}

	void OnLevelWasLoaded (int l) {
		if (l == 2) {
			SpawnPlayer ();
			gameStarted = true;
		}

		if (Network.isServer) {
			Network.Instantiate (scorePrefab, new Vector3(0, -160, 0), Quaternion.identity, 0);
		}
	}

	void OnServerInitialized () {
		Debug.Log("Server Initializied!");
		Application.LoadLevel ("game");
	}

	private void SpawnPlayer() {
		Network.Instantiate(playerPrefab, new Vector3(200f, 200f, 0f), Quaternion.identity, 0);
	}

	void OnGUI () {
		if (!Network.isClient && !Network.isServer) {
			if (GUI.Button (new Rect (100, 100, 250, 100), "Start Server")) {
				StartServer ();
			}
			if (GUI.Button (new Rect (100, 250, 250, 100), "Refresh Hosts")) {
				RefreshHostList ();
			}
			if (hostList != null) {
				for (int i = 0; i < hostList.Length; i++) {
					if (GUI.Button (new Rect (400, 100 + (110 * i), 300, 100), hostList [i].gameName))
						JoinServer (hostList [i]);
				}
			}
			if(GUI.Button(new Rect(400, 250, 250, 100), "Connect")) 
			{
				if (!BtConnector.isBluetoothEnabled ()){
					BtConnector.askEnableBluetooth();
				} else BtConnector.connect();
				BtConnector.stopListen();
			}

		}
	}
}
