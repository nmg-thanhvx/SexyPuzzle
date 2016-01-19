using UnityEngine;
using System.Collections.Generic;
using Prime31;



namespace Prime31
{
	public class FacebookEventListener : MonoBehaviour
	{
#if UNITY_IOS || UNITY_ANDROID
		// Listens to all the events.  All event listeners MUST be removed before this object is disposed!
		void OnEnable()
		{
			FacebookManager.sessionOpenedEvent += sessionOpenedEvent;
			FacebookManager.loginFailedEvent += loginFailedEvent;

			FacebookManager.graphRequestCompletedEvent += graphRequestCompletedEvent;
			FacebookManager.graphRequestFailedEvent += facebookCustomRequestFailed;
			FacebookManager.facebookComposerCompletedEvent += facebookComposerCompletedEvent;

			FacebookManager.shareDialogFailedEvent += shareDialogFailedEvent;
			FacebookManager.shareDialogSucceededEvent += shareDialogSucceededEvent;

			FacebookManager.gameDialogFailedEvent += gameDialogFailedEvent;
			FacebookManager.gameDialogSucceededEvent += gameDialogSucceededEvent;
		}


		void OnDisable()
		{
			// Remove all the event handlers when disabled
			FacebookManager.sessionOpenedEvent -= sessionOpenedEvent;
			FacebookManager.loginFailedEvent -= loginFailedEvent;

			FacebookManager.graphRequestCompletedEvent -= graphRequestCompletedEvent;
			FacebookManager.graphRequestFailedEvent -= facebookCustomRequestFailed;
			FacebookManager.facebookComposerCompletedEvent -= facebookComposerCompletedEvent;

			FacebookManager.shareDialogFailedEvent -= shareDialogFailedEvent;
			FacebookManager.shareDialogSucceededEvent -= shareDialogSucceededEvent;

			FacebookManager.gameDialogFailedEvent -= gameDialogFailedEvent;
			FacebookManager.gameDialogSucceededEvent -= gameDialogSucceededEvent;
		}



		void sessionOpenedEvent()
		{
			Debug.Log( "Successfully logged in to Facebook" );
		}


		void loginFailedEvent( P31Error error )
		{
			Debug.Log( "Facebook login failed: " + error );
		}


		void facebokDialogCompleted()
		{
			Debug.Log( "facebokDialogCompleted" );
		}


		void graphRequestCompletedEvent( object obj )
		{
			Debug.Log( "graphRequestCompletedEvent" );
			Prime31.Utils.logObject( obj );
		}


		void facebookCustomRequestFailed( P31Error error )
		{
			Debug.Log( "facebookCustomRequestFailed failed: " + error );
		}


		void facebookComposerCompletedEvent( bool didSucceed )
		{
			Debug.Log( "facebookComposerCompletedEvent did succeed: " + didSucceed );
		}


		void shareDialogFailedEvent( P31Error error )
		{
			Debug.Log( "shareDialogFailedEvent: " + error );
		}


		void shareDialogSucceededEvent( string postId )
		{
			Debug.Log( "shareDialogSucceededEvent: " + postId );
		}


		void gameDialogFailedEvent( P31Error error )
		{
			Debug.Log( "gameDialogFailedEvent: " + error );
		}


		void gameDialogSucceededEvent( Dictionary<string,object> dict )
		{
			Debug.Log( "gameDialogSucceededEvent" );
			Utils.logObject( dict );
		}
#endif
	}

}
