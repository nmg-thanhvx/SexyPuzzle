using UnityEngine;
using System.Collections;

public class Stage : MonoBehaviour {

  
    private int currentValue;
    public int picNum;
	// Use this for initialization
	void Start () {
	    currentValue = PlayerPrefs.GetInt(Game.currentStage);
    }
	
	// Update is called once per frame
	void Update () {
	
	}
    void OnClick()
    {
        CsManager.picNum = picNum;
        GameGui.instance.PushPanel("GamePanel");
    }
}
