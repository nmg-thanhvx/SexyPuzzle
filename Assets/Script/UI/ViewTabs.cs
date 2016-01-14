using UnityEngine;
using System.Collections;

public class ViewTabs : MonoBehaviour {
	[SerializeField] StateManager screens;
	[SerializeField] StateManager[] tabs;

	public void OnTabClicked(StateManager clickedTab) {

		foreach (var tab in tabs) {
			if (tab != clickedTab) {
				tab.ChangeState("Deselected");
			}
		}
		clickedTab.ChangeState("Selected");
		screens.ChangeState (clickedTab.name);
	}

}
