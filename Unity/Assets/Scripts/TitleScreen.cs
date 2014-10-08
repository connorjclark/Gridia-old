using UnityEngine;
using System.Collections;
using Gridia;

public class TitleScreen : MonoBehaviour {

	void Start () {
	
	}
	
	void Update () {
	    
	}

    void OnGUI() {
        int width = 150;
        int height = 30;
        int x = (Screen.width - width) / 2;

        if (GUI.Button(new Rect(x, 200, width, height), "Host server"))
        {
            StartGame(true);
        }
        if (GUI.Button(new Rect(x, 300, width, height), "Connect to server"))
        {
            StartGame(false);
        }
    }

    void StartGame(bool asServer) {
        GridiaConstants.IsServer = asServer;
        Application.LoadLevel("Main");
    }
}