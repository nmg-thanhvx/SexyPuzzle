using UnityEngine;
using System.Collections;

public class AlertPanel : AnimatedPanel {

	static AlertPanel _instance;
	
	public static AlertPanel instance {
		get {
			return _instance;
		}
	}

	public static void Show(string text, System.Action<bool> onFinish, bool showCancelButton = true)
	{
		if ( _instance == null ) {
			return;
		}
		
		_instance._Show(text,onFinish,showCancelButton);
		
	}		

	[SerializeField] UIGrid buttonGrid;
	[SerializeField] UIButton okButton;
	[SerializeField] UIButton cancelButton;
	
	[SerializeField] UILabel textLabel;

	System.Action<bool> finishCallback = null;
	
#region Monobehavior

	void Awake()
	{
		_instance = this;
	}

	// Use this for initialization
	void Start() 
	{
		
	}

#endregion
			
	void _Show(string text, System.Action<bool> onFinish, bool showCancelButton = true)
	{
		
		if ( showCancelButton ) {
			cancelButton.gameObject.SetActive(true);
		} else {
			cancelButton.gameObject.SetActive(false);
		}
		buttonGrid.repositionNow = true;
		
		this.textLabel.text = text;
		finishCallback = onFinish;
		this.Show();
		
	}
	
	public void Ok()
	{
//		SoundsManager.instance.PlaySoundButton ();
//		Game.instance.BGM = true;
		if ( finishCallback != null ) {
			finishCallback(true);
		}
		this.Hide();
	}
	
	public void Cancel()
	{
//		Game.instance.BGM = false;
//		SoundsManager.instance.PlayCancelButton ();
		if ( finishCallback != null ) {
			finishCallback(false);
		}
		this.Hide();
	}
	
}
