using UnityEngine;
using System.Collections;

public class GamePanel : AnimatedPanel {


	static GamePanel _instance;
	public static GamePanel instance {
		get {
			return _instance;
		}
	}


	// Use this for initialization
	void Awake () {
		if (_instance != null) {
			Debug.LogError ("Multiple Game Instances Exist.");
		} else {
			_instance = this;
		}

	}

	
	#region Panel

	protected override void PanelWillShow ()
	{

	}
	protected override void PanelDidShow ()
	{
		
	}
	protected override void PanelWillHide ()
	{
		
	}
	protected override void PanelDidHide ()
	{
		
	}
	#endregion
}
