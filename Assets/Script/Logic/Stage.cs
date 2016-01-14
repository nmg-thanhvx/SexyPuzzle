using UnityEngine;
using System.Collections;

public class Stage : MonoBehaviour {

    public string stageName;
    private int currentValue;
	// Use this for initialization
	void Start () {
	    currentValue = PlayerPrefs.GetInt(stageName);
    }
	
	// Update is called once per frame
	void Update () {
	
	}
    void OnClick()
    {
        Game.currentStage = stageName;
        Game.currentStageValue = currentValue;
        GameGui.instance.PushPanel("StagePanel");
        Debug.LogError(stageName);
        Debug.LogError(currentValue.ToString());
    }
}
