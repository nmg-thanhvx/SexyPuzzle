using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class CsOptions : MonoBehaviour {

	public GUISkin skin;
	
	bool isPicture = false;
	bool soundOn;
	int gridNum = 0;
	string[] imgSize = {"3x3", "3x4", "4x4", "4x5", "5x5", "5x6", "6x6"};
	
	Vector2 scrollPosition = Vector2.zero;
	
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
		soundOn = CsManager.isSound;
		
		if (isPicture) {
			SelectPicture();
			return;
		}
		
		var w = Screen.width / 2;
		var h = Screen.height / 2;
	
		var msg = "Sound On";
		if (!soundOn) msg = "Sound Off";
		
		// Sound OnOff
	
		soundOn = GUI.Toggle(new Rect(w - 80, h - 90, 160, 35), soundOn, " " + msg);

		CsManager.isSound = soundOn;
		
		// Image Size
		GUI.Label(new Rect(w - 80, h - 60, 160, 35), "Image Size :");
		gridNum = GUI.SelectionGrid(new Rect(w - 60, h - 30, 170, 90), gridNum, imgSize, 3);
		CsManager.stageNum = gridNum + 1;
		
	
		isPicture = GUI.Button(new Rect(w - 70, h + 75, 140, 35), "Select Picture"); 
		
		// Go back
		if (GUI.Button(new Rect(w - 45, h + 120, 90, 35), "Go back")) {
            SceneManager.LoadScene("GameTitle");
        }	
	}
	

	void SelectPicture() {
		
		int mx = 90;		
		int my = 30;		
		
		Rect winRect = new Rect(mx, my, Screen.width - mx * 2, Screen.height - my * 1.5f);
	
		Rect scrollRect = new Rect(mx + 10, my + 25, Screen.width - mx * 2 - 20, Screen.height - my * 1.5f - 30);
		Rect scrollArea = new Rect(0 ,0, Screen.width * 0.6f, Screen.height * 1.2f);
		
		GUI.Window(1, winRect, WindowFunc, "Select Picture");
		
		scrollPosition = GUI.BeginScrollView(scrollRect, scrollPosition, scrollArea, false, false);
		float pw = Screen.width / 4.5f;	
		float ph = pw * 7 / 5;			
	/*
		for (int i = 0; i < CsManager.picCount; i++) {
			Texture2D img = Resources.Load("picture" + (i + 1)) as Texture2D;	
			int x = i % 3;
			int y = i / 3;
			
			if (GUI.Button(new Rect(pw * x, ph * y, pw - 20, ph - 15), img)) {
				CsManager.picNum = i + 1;
				isPicture = false;
			}	
		}		
		*/
		GUI.EndScrollView();	
	}
	
	//-----------------------------
	// GUI Window 
	//-----------------------------
	void WindowFunc(int winID) {
	}	
}
