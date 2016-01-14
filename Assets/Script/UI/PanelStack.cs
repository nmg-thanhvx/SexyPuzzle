using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//Menu System
public class PanelStack : MonoBehaviour {
	
#region Menu Stack
	
	protected Stack<PanelController> panelStack = new Stack<PanelController>();
	
	public PanelController currentPanel {
		get {
			if ( panelStack.Count > 0 ) {
				return panelStack.Peek();
			}
			return null;
		}
	}
	
	public int Count {
		get {
			return panelStack.Count;
		}
	}
	
	//Push panel will hide the panel before it
	public virtual void PushPanel(PanelController panel)
	{
		
		if ( currentPanel != null ) {
			
			currentPanel.Hide(()=>{ 
				
				panelStack.Push(panel);
				panel.parentStack = this;
				panel.Show();
				
			});
			
		}
		else
		{
			
			panelStack.Push(panel);
			panel.parentStack = this;
			panel.Show();
			
		}
		
	}
		
	//Push panel without hiding panel below it
	public virtual void PushPanelOnTop(PanelController panel)
	{		
		panelStack.Push(panel);
		panel.parentStack = this;
		panel.Show();
	}
	
	//Pop the top panel off the stack and show the one beheath it
	public virtual void PopPanel()
	{
		
		var oldPanel = panelStack.Pop();
		
		oldPanel.Hide(()=>{
			
			var newPanel = this.currentPanel;
			if ( newPanel != null && !newPanel.IsShowing ) {
				newPanel.Show();
			}
			
		});
		
	}
	
	//Pop all panels till there is only one panel left in the stack
	public virtual void PopToTop()
	{
		//Pop Panels Till Top
		while ( panelStack.Count > 1 ) {
			currentPanel.Hide();
			panelStack.Pop();
		}
		
		if ( !currentPanel.IsShowing ) 
		{
			currentPanel.Show();	
		}

	}	
	
	//Pop till we remove specific panel
	public virtual void PopPanel(PanelController panel)
	{
		
		if ( !panelStack.Contains(panel) ) {
			return;
		}
		
		PanelController oldPanel = null;
		
		//Pop panels until we find the right one we're trying to pop
		do
		{
			oldPanel = panelStack.Pop();
			oldPanel.Hide();
		} while(oldPanel != panel && panelStack.Count > 0);
		
		var newPanel = this.currentPanel;
		if ( newPanel != null && !newPanel.IsShowing ) {
			
			newPanel.Show();
		}
		
	}
	
#endregion
	
}
