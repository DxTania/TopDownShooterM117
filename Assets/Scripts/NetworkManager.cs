using UnityEngine;
using System.Collections;

public class NetworkManager : MonoBehaviour {

	private HostData[] hostList;
	private const string typeName = "UCLA CSM117";
	private const string gameName = "TopDown";
	private bool gameStarted = false;
	public GameObject playerPrefab;
	public GameObject scorePrefab;
	private GameObject score;

	void Start () {
		DontDestroyOnLoad (this);
		Application.LoadLevel ("menu");
	}

	void Update () {
		GameObject[] players = GameObject.FindGameObjectsWithTag ("Player");
		if (gameStarted && Network.isServer && players.Length == 0) {
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
			GameObject mobileControl = GameObject.Find ("MobileSingleStickControl");
			Network.Instantiate (scorePrefab, new Vector3(50, 199, 0), Quaternion.identity, 0);
			score = GameObject.FindGameObjectWithTag ("Score");
			score.transform.SetParent(mobileControl.transform);
			score.transform.localScale = new Vector3(1, 1, 1);
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
		}
	}
}
