using UnityEngine;
using System.Collections;

public class Stage : MonoBehaviour {

  
    private int currentValue;
    public int stageNum;
    // Use this for initialization

    void OnEnable() {
        string a = "Pictures/" + CsManager.picture + "/" + CsManager.picture + stageNum;
        this.gameObject.GetComponent<UITexture>().mainTexture = Resources.Load(a) as Texture;// "Tsunade" + picNum;
        currentValue = PlayerPrefs.GetInt(CsManager.picture);
        if (stageNum >= currentValue)
        {
            string message = CsManager.picture + stageNum + "is Unlock";
            Debug.LogError(message);
        }
    }
	
	// Update is called once per frame
	void Update () {
	
	}
    void OnClick()
    {       
        CsManager.stageNum = stageNum;
        GameGui.instance.PushPanel("GamePanel");
    }
}
