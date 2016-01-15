using UnityEngine;
using System.Collections;

public class Stages : MonoBehaviour {

    public string stageName;
    // Use this for initialization
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
    void OnClick()
    {

        CsManager.picture = stageName;
        GameGui.instance.PushPanel("StagePanel");    
    }
}
