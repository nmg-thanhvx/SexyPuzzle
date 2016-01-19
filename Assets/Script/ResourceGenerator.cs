using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public class ResourceGenerator : MonoBehaviour {

	static Dictionary<string,ResourceGenerator> _dict = new Dictionary<string, ResourceGenerator>();
	public static ResourceGenerator Get(string key) {
		return _dict[key];
	}

	[SerializeField] int _value = 10;
	[SerializeField] int maxValue = 10;
	
	[SerializeField] float generationTime = 60;
	
	[SerializeField] bool save = true;
	[SerializeField] string key = "";
	
	float time = 0;
	
	public int Value {
		get {
			return _value;			
		}
		set {
			Debug.LogError("Resource Value Should be modified using member functions, not Value property");
			this._value = Mathf.Clamp(value,0,this.maxValue);
		}
	}
	
	public int MaxValue {
		get {
			return maxValue;
		}
	}
	
	public float Percent {
		get {
			return (float)_value / (float)maxValue;
		}
	}
	
	public float TimeTillNext {
		get {
			return (generationTime - time);
		}
	}
	
	public delegate void ResourceGeneratorDelegate(ResourceGenerator generator);
	
	public event ResourceGeneratorDelegate OnGenerated;
	public event ResourceGeneratorDelegate OnConsumed;
	
	public UnityEvent OnValueChanged;
	
	void Awake()
	{
		_dict[this.key] = this;
	}
	
	// Use this for initialization
	void Start () 
	{		
		_value = maxValue;
		
		if ( save ) {
			Load();
		}
		
	}
	
	void OnDestroy()
	{
		
		if ( save ) {
			Save();
		}
		
		if ( _dict.ContainsKey(key) && _dict[key] == this ) {
			_dict.Remove(key);
		}
		
	}
	
	void OnApplicationQuit()
	{
		if ( save )
		{
			Save();
		}
	}
	
	void OnApplicationPause(bool paused)
	{
		if ( save ) {
			if ( paused )
			{
				Save();
			}
			else
			{
				Load();
			}
		}
	}
	
	void OnApplicationFocus(bool focused)
	{
		if ( !Application.runInBackground && save )
		{
			if ( focused )
			{
				Load();
			}
			else
			{
				Save();
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
	
		if ( _value < maxValue ) {
			
			time += Time.deltaTime;
			
			if ( time >= generationTime ) {
				
				time = 0;
				_value += 1;
				
				OnValueChanged.Invoke();
				
				if ( OnGenerated != null ) {
					OnGenerated(this);
				}
				
			}
			
		}
	
	}
	
	void Save()
	{
		
		var dateString = System.DateTime.Now.ToString();
		
		PlayerPrefs.SetString("ResourceGenerator_"+key+"_date",dateString);
		PlayerPrefs.SetInt("ResourceGenerator_"+key+"_value",_value);
		PlayerPrefs.SetFloat("ResourceGenerator_"+key+"_time",time);
			
	}
	
	void Load()
	{
		
		if ( PlayerPrefs.HasKey("ResourceGenerator_"+key+"_date") ) {
		
			var dateString = PlayerPrefs.GetString("ResourceGenerator_"+key+"_date");
			var date = System.DateTime.Parse(dateString);
			var now = System.DateTime.Now;
			
			var totalSeconds = (float)((now - date).TotalSeconds);
			
			this._value = PlayerPrefs.GetInt("ResourceGenerator_"+key+"_value",0);
			this.time = PlayerPrefs.GetFloat("ResourceGenerator_"+key+"_time",0);
			
			this.time += totalSeconds;
			
			int points = Mathf.FloorToInt(this.time / generationTime);
			
			this.time -= (points * generationTime);
			
			this._value += points;
			
			if ( this._value >= maxValue ) {
				this._value = maxValue;
				this.time = 0;
			}
			
		}
		
	}
	
	[ContextMenu("ConsumeAll")]
	void ConsumeAll()
	{
		
		this._value = 0;
		
		OnValueChanged.Invoke();
		
		if ( OnConsumed != null ) {
			OnConsumed(this);
		}
		
	}
	
	public bool Consume(int amount)
	{
		if ( amount >= this._value ) {
			
			this._value -= amount;
			
			OnValueChanged.Invoke();
			
			if ( OnConsumed != null ) {
				OnConsumed(this);
			}
			
			return true;
			
		} else {
			
			return false;
		
		}
		
	}
	
}
