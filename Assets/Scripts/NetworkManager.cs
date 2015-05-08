using UnityEngine;
using System.Collections;

public class NetworkManager : MonoBehaviour {

	private HostData[] hostList;
	private const string typeName = "TopDownShooterM117";
	private const string gameName = "RoomName";
	public GameObject playerPrefab;

	void Start () {
		DontDestroyOnLoad (this);
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
		SpawnPlayer ();
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
			if (GUI.Button(new Rect(100, 250, 250, 100), "Refresh Hosts")) {
				RefreshHostList();
			}
			if (hostList != null) {
				for (int i = 0; i < hostList.Length; i++) {
					if (GUI.Button(new Rect(400, 100 + (110 * i), 300, 100), hostList[i].gameName))
						JoinServer(hostList[i]);
				}
			}
		}
	}
}
