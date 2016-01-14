using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class CsAbout : MonoBehaviour {

	public GUISkin skin;
	
	float speed = 0.7f;
	
	//-----------------------------
	// Update
	//-----------------------------
	void Update () {
		float amtToMove = speed * Time.deltaTime;
		transform.Translate(Vector3.back * amtToMove);
		
		Vector3 pos = transform.position;
		if (transform.position.z > 4.8f) {
			pos.z = -10;
			transform.position = pos;
		}	
			
	}
	
	//-----------------------------
	// OnGUI()
	//-----------------------------
	void OnGUI() {
		GUI.skin = skin;
		
	
	
		var w = Screen.width / 2;
		var h = Screen.height / 2;
		
		// Go back
		if (GUI.Button(new Rect(w - 45, h + 120, 90, 35), "Go back")) {
            SceneManager.LoadScene("GameTitle");
        }	
	}
}
