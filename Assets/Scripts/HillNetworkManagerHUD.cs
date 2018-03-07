using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class HillNetworkManagerHUD : MonoBehaviour {

	private NetworkManager myNetworkManager;
	private NetworkClient myNetworkClient;
	public InputField ipAddress;

	public MainMenu myMenu;

	public Scene curScene;
	public string curSceneName;

	private float connectionTimer, connectionMaxTimer;
	private bool isConnecting;

	void Awake() {
		curScene = SceneManager.GetActiveScene();
		curSceneName = curScene.name;

		myNetworkManager = GameObject.Find("Network Manager").GetComponent<NetworkManager> ();
		myNetworkClient = new NetworkClient ();

		if(curSceneName == "Offline") {
			ipAddress = ipAddress.GetComponent<InputField> ();
			//ipAddress.text = "127.0.0.1";
		}

		isConnecting = false;
		connectionTimer = 0.0f;
		connectionMaxTimer = 3.0f;
	}

	void Update() {
		curScene = SceneManager.GetActiveScene();
		curSceneName = curScene.name;

		if (curSceneName == "Offline" && myMenu == null) {
			myMenu = GameObject.Find ("Main Menu Manager").GetComponent<MainMenu>();
			myMenu.ipAddress.text = myNetworkManager.networkAddress;
		}

		if (isConnecting == false) {
		
		} else if (isConnecting == true && connectionTimer < connectionMaxTimer) {
			connectionTimer += Time.deltaTime;
		} else if (isConnecting == true && connectionTimer > connectionMaxTimer) {
			// Connection Time Out
			isConnecting = false;
			connectionTimer = 0.0f;
			if (myMenu != null) {
				myMenu.connectionStatus.SetActive (false);
			}

			if (!myNetworkClient.isConnected) {
				NetworkClient.ShutdownAll();
			}

		}
	}
	public void StartServer() {
		if (!NetworkClient.active && !NetworkServer.active) {
			Debug.Log ("Starting Server");
//			Debug.Log ("Listening Port: " + myNetworkManager.networkPort);
			myNetworkManager.StartHost ();
		}
	}

	public void StartDedicated() {
		if (!NetworkClient.active && !NetworkServer.active) {
			Debug.Log ("Starting Dedicated Server");
			//			Debug.Log ("Listening Port: " + myNetworkManager.networkPort);
			myNetworkManager.StartServer ();
		}
	}

	public void SetupLocalClient()
	{
		if(!isConnecting)
		{
			myNetworkClient.RegisterHandler(MsgType.Connect, OnConnected);
			ipAddress = GameObject.Find ("Address Field").GetComponent<InputField>();
			myNetworkManager.networkAddress = ipAddress.text;
			myNetworkClient = myNetworkManager.StartClient ();
			myMenu.connectionStatus.SetActive (true);
			isConnecting = true;
			connectionTimer = 0.0f;
			Debug.Log("Setup Local Client");
		}
	}

	public void StopLocalClient()
	{
		if (NetworkServer.active && NetworkClient.active) {
			Debug.Log ("Server Host is disconnecting.");
			myNetworkManager = GameObject.Find ("NetworkManager").GetComponent<NetworkManager> ();

			myNetworkManager.StopHost ();

		} else if (NetworkClient.active) {
			myNetworkManager = GameObject.Find ("NetworkManager").GetComponent<NetworkManager> ();
			myNetworkManager.StopClient ();
		} else if (NetworkServer.active) {
			myNetworkManager = GameObject.Find ("NetworkManager").GetComponent<NetworkManager> ();
			myNetworkManager.StopServer ();
		}

		NetworkClient.ShutdownAll();

	}

	public virtual void OnClientConnect(NetworkConnection conn)
	{

	}

	// client function
	public void OnConnected(NetworkMessage netMsg)
	{
		Debug.Log("Connected to server");
	}

	public void OnError(NetworkMessage netMsg) {
		Debug.Log("Could not connect to server.");
	}

	public void OnFailedToConnect(NetworkConnectionError error) {
		Debug.Log("Could not connect to server: " + error);
	}

	public void OnConnectedToServer() {
		Debug.Log ("OnConnectedToServer");
	}



}
