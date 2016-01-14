using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(UIPanel))]
public class PanelController : PanelStack
{
	
	protected UIPanel _panel;
	public UIPanel panel {
		get {
			if ( _panel == null ) {
				_panel = GetComponent<UIPanel>();
			}
			return _panel;
		}
	}
	
	protected PanelStack _parentStack;
	public PanelStack parentStack {
		get {
			return _parentStack;
		}
		set {
			_parentStack = value;
		}
	}
	
	public bool IsShowing {
		get {
			return (panel.alpha > 0 && gameObject.activeSelf);
		}
	}
	
	public float alpha {
		get {
			return panel.alpha;
		}
		set {
			panel.alpha = value;
		}
	}
	
	public virtual void Show(System.Action onFinish = null)
	{
		
		if ( this.panel.alpha == 1f ) {
			return;
		}
		
		this.gameObject.SetActive(true);
		
		PanelWillShow();
		this.panel.alpha = 1f;
		PanelDidShow();
		
		if ( onFinish != null )
		{
			onFinish();
		}
	
	}
	
	public virtual void Hide(System.Action onFinish = null)
	{
		
		if ( this.panel.alpha == 0f ) {
			return;
		}
		
		PanelWillHide();
		this.panel.alpha = 0f;
		PanelDidHide();
		
		this.gameObject.SetActive(false);
		
		if ( onFinish != null )
		{
			onFinish();
		}
		
	}

	public virtual void PanelWillPush()
	{
	}

	protected virtual void PanelWillShow()
	{
	}
	
	protected virtual void PanelDidShow()
	{
	}

	protected virtual void PanelWillHide()
	{
	}
	
	protected virtual void PanelDidHide()
	{
	}

#region Navigation Helpers

	public void PopSelf()
	{
		if ( this.parentStack != null ) {
			this.parentStack.PopPanel(this);
		}
	}
	
	public void PopSelfToTop()
	{
		this.parentStack.PopToTop();
	}
	
#endregion	

}
