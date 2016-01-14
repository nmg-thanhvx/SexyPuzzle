using UnityEngine;
using System.Collections;
using AnimationOrTween;

[RequireComponent(typeof(Animation))]
public class AnimatedPanel : PanelController {

	public AnimationClip showAnimation;
	public Direction showPlayDirection = Direction.Forward;
	
	public AnimationClip hideAnimation;
	public Direction hidePlayDirection = Direction.Reverse;
	
	protected bool _isAnimating = false;
	public bool isAnimating {
		get {
			return _isAnimating;
		}
	}
	
	protected Animation _animation;
	public Animation panelAnimation {
		get {
			if ( _animation == null )
			{
				_animation = GetComponent<Animation>();
			}
			return _animation;
		}
	}
	
	public string sfxWillAppear = "";
	public string sfxDidAppear = "";
	
	public string sfxWillHide = "";
	public string sfxDidHide = "";
	
	public float defaultFadeTime = 0.1f;
	
	System.Action onShowFinishCallback = null;
	System.Action onHideFinishCallback = null;
	
	public override void Show(System.Action onFinish = null)
	{
		
		if ( this.panel.alpha == 1f && this.gameObject.activeSelf ) {
			return;
		}
		
		onShowFinishCallback = onFinish;
		
		this.gameObject.SetActive(true);
		
		//TODO: Block user input here	
		//TouchBlockerPanel.Block();
		
		if ( !string.IsNullOrEmpty(sfxWillAppear) ) {
			//MasterAudio.PlaySoundAndForget(sfxWillAppear);
		}
		
		try {
			PanelWillShow();
		} catch (System.Exception e) {
			Debug.LogError("PanelWillShow Failed " + e.Message + "\n " + e.StackTrace);
		}
		
		this.panel.alpha = 1f;
						
		_isAnimating = true;
		
		if ( showAnimation != null )
		{
			Debug.Log("Default: " + this.panelAnimation.clip.name);
			ActiveAnimation anim = ActiveAnimation.Play(this.panelAnimation, showAnimation.name,showPlayDirection,EnableCondition.EnableThenPlay,DisableCondition.DoNotDisable);
			
			if ( anim == null ) 
			{
				OnShowFinished();
			} 
			else 
			{
				EventDelegate.Add(anim.onFinished, OnShowFinished, true);                                     	
			}
		
		} 
		else
		{
			//Default to 1 second tween
			this.panel.alpha = 0;
			var tween = TweenAlpha.Begin(this.gameObject,defaultFadeTime,1f);
			tween.SetOnFinished(OnShowFinished);
			//OnShowFinished();
		}
		
	}
	
	public override void Hide(System.Action onFinish = null)
	{
		
		if ( this.alpha == 0 ) {
			return;
		}
		
		onHideFinishCallback = onFinish;
		
		if ( !string.IsNullOrEmpty(sfxWillHide) ) {
			//MasterAudio.PlaySoundAndForget(sfxWillHide);
		}
		
		try {
			PanelWillHide();
		} catch {
			Debug.LogError("PanelWillHide Failed " + this.name);
		}
		
		//TODO: Start blocking user input here
		//TouchBlockerPanel.Block();
		
		_isAnimating = true;
		if ( hideAnimation != null )
		{
			ActiveAnimation anim = ActiveAnimation.Play(this.GetComponent<Animation>(),hideAnimation.name,hidePlayDirection);
			
			if ( anim != null ) {
				EventDelegate.Add(anim.onFinished, OnHideFinished, true);
			} else {
				OnHideFinished();
			}
		}
		else
		{
			//Default Hide Fade
			this.panel.alpha = 1f;
			var tween = TweenAlpha.Begin(this.gameObject,defaultFadeTime,0f);
			tween.SetOnFinished(OnHideFinished);
			//OnHideFinished();
		}
		
	}
	
	protected virtual void OnShowFinished(System.Action onFinish = null) {
		
		if ( !string.IsNullOrEmpty(sfxDidAppear) ) {
			//MasterAudio.PlaySoundAndForget(sfxDidAppear);
		}
		
		//TODO: Stop blocking user input here
		//TouchBlockerPanel.Unblock();
		
		this.panel.SetDirty();
		_isAnimating = false;
		
		try {
			PanelDidShow();
		} catch {
			Debug.LogError("PanelDidShow Failed " + this.name);
		}
		
		if ( onShowFinishCallback != null )
		{
			onShowFinishCallback();
			onShowFinishCallback = null;
		}
		
	}
	
	protected virtual void OnHideFinished(System.Action onFinish = null) {
				
		if ( !string.IsNullOrEmpty(sfxDidHide) ) {
			//MasterAudio.PlaySoundAndForget(sfxDidHide);
		}
		
		//TODO: Stop blocking user input here
		//TouchBlockerPanel.Unblock();

		this.panel.alpha = 0f;
		this.gameObject.SetActive(false);
		_isAnimating = false;
		
		try {
			PanelDidHide();
		} catch {
			Debug.LogError("PanelDidHide Failed " + this.name);
		}
		
		if ( onHideFinishCallback != null )
		{
			onHideFinishCallback();
			onHideFinishCallback = null;
		}
		
	}
	
}
