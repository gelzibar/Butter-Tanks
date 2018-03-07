using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class MainMenu : MonoBehaviour {

	public Button serverButton, clientButton, dedicatedButton;
	public GameObject connectionStatus;

	public InputField ipAddress;


	void Start() {
		serverButton = GameObject.Find ("Server").GetComponent<Button> ();
		dedicatedButton = GameObject.Find ("Dedicated Server").GetComponent<Button> ();
		clientButton = GameObject.Find ("Client").GetComponent<Button> ();
		connectionStatus = GameObject.Find ("Connection Status");
		connectionStatus.SetActive(false);

		clientButton.onClick.AddListener (() => {GameObject.Find("Network Manager").GetComponent<HillNetworkManagerHUD>().SetupLocalClient();});
		serverButton.onClick.AddListener (() => {GameObject.Find("Network Manager").GetComponent<HillNetworkManagerHUD>().StartServer();});
		dedicatedButton.onClick.AddListener (() => {GameObject.Find("Network Manager").GetComponent<HillNetworkManagerHUD>().StartDedicated();});

		ipAddress = GameObject.Find ("Address Field").GetComponent<InputField> ();

		Cursor.visible = true;
		Cursor.lockState = CursorLockMode.None;
	}
	public void QuitGame() {
		Application.Quit ();
	}
}
