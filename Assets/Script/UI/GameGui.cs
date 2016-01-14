using UnityEngine;
using UnityEngineInternal;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GameGui : PanelStack {

	static GameGui _instance;
	public static GameGui instance {
		get {
			return _instance;
		}
	}

	public List<PanelController> panels;
	
	public PanelController defaultPanel;
	
#region Monobehavior

	void Awake()
	{		
		_instance = this;
		DontDestroyOnLoad(gameObject);	
	}
	IEnumerator Start()
	{
	
		yield return new WaitForSeconds(0.5f);
		
		if ( defaultPanel != null ) {
			PushPanel(defaultPanel);
		}
		
	}
	
	public virtual void PushPanel(string panelName)
	{
		PanelController panel = GetPanel (panelName);
		PushPanel(panel);
	}
	
	public PanelController GetPanel(string panelName) 
	{
		return panels.Where(s => s).FirstOrDefault(s => s.name == panelName);
	}	

	public virtual void PopPanel(string panelName){
		PanelController panel = GetPanel (panelName);
		PopPanel (panel);
	}



#endregion
		
}
