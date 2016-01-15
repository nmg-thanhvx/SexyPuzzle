using UnityEngine;
using System.Collections;

public class Stage : MonoBehaviour {

  
    private int currentValue;
    public int picNum;
	// Use this for initialization
	void Start () {
	    currentValue = PlayerPrefs.GetInt(CsManager.picture);
        if (picNum >= currentValue) {
            string message = CsManager.picture + picNum + "is Unlock";
            Debug.LogError(message);
        }
    }
	
	// Update is called once per frame
	void Update () {
	
	}
    void OnClick()
    {       
        CsManager.stageNum = picNum;
        GameGui.instance.PushPanel("GamePanel");
    }
}
