using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class CsGameTitle : MonoBehaviour {

	public GUISkin skin;
	
	//-----------------------------
	// Start
	//-----------------------------
	void Start() {

	}
	
	//-----------------------------
	// OnGUI()
	//-----------------------------
	void OnGUI() {
		GUI.skin = skin;
	
		int w = Screen.width / 2;
		var h = Screen.height / 2;
		
		int menuNum = 0;
		
		if (GUI.Button(new Rect(w - 70, h - 90, 140, 35), "New Game")) menuNum = 1;
		if (GUI.Button(new Rect(w - 70, h - 40, 140, 35), "Load Game")) menuNum = 2;
		if (GUI.Button(new Rect(w - 70, h + 10, 140, 35), "Options")) menuNum = 3;
		if (GUI.Button(new Rect(w - 70, h + 60, 140, 35), "About")) menuNum = 4;
		if (GUI.Button(new Rect(w - 70, h + 110, 140, 35), "Quit")) menuNum = 5;
	
		switch (menuNum) {
			case 1 :	// New Game
				CsManager.state = CsManager.STATE.START;
                SceneManager.LoadScene("GameMain");
                break;
			case 2 :	// Load Game
                SceneManager.LoadScene("GameMain");
                CsManager.state = CsManager.STATE.LOAD;
				break;
			case 3 :	// Options
                SceneManager.LoadScene("GameOptions");
                break;
			case 4 :	// About
                SceneManager.LoadScene("GameAbout");
                break;
			case 5 :	// Quit
				Application.Quit();
				break;
		}
	}
}
