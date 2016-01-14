using UnityEngine;
using System.Collections;

public class BusyPanel : AnimatedPanel {

	static BusyPanel _instance;
	
	public static void Block()
	{
		if ( _instance == null ) {
			return;
		}
		_instance._Block();
	}
	
	public static void Unblock()
	{
		if ( _instance == null ) {
			return;
		}
		_instance._Unblock();
	}
	
	int count = 0;
	
	void Awake() 
	{
		_instance = this;
	}
	
	void Start()
	{
		this.gameObject.SetActive(false);
	}
	
	void _Block()
	{
		count++;
		if ( count > 0 ) {
			Show();
		}
	}
	
	void _Unblock()
	{
		count--;
		if ( count <= 0 ) {
			Hide();
		}
	}
	
}
