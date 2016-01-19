using UnityEngine;
using System.Collections;
using System.Collections.Generic;



namespace Prime31
{
	public class TwitterEventListener : MonoBehaviour
	{
#if UNITY_IOS || UNITY_ANDROID
		// Listens to all the events.  All event listeners MUST be removed before this object is disposed!
		void OnEnable()
		{
			#if UNITY_ANDROID
			TwitterManager.twitterInitializedEvent += twitterInitializedEvent;
			#endif
			TwitterManager.loginSucceededEvent += loginSucceeded;
			TwitterManager.loginFailedEvent += loginFailed;
			TwitterManager.requestDidFinishEvent += requestDidFinishEvent;
			TwitterManager.requestDidFailEvent += requestDidFailEvent;
			TwitterManager.tweetSheetCompletedEvent += tweetSheetCompletedEvent;
		}
	
	
		void OnDisable()
		{
			#if UNITY_ANDROID
			TwitterManager.twitterInitializedEvent -= twitterInitializedEvent;
			#endif
			TwitterManager.loginSucceededEvent -= loginSucceeded;
			TwitterManager.loginFailedEvent -= loginFailed;
			TwitterManager.requestDidFinishEvent -= requestDidFinishEvent;
			TwitterManager.requestDidFailEvent -= requestDidFailEvent;
			TwitterManager.tweetSheetCompletedEvent -= tweetSheetCompletedEvent;
		}
	
	
		void twitterInitializedEvent()
		{
			Debug.Log( "twitterInitializedEvent" );
		}
	
	
		void loginSucceeded( string username )
		{
			Debug.Log( "Successfully logged in to Twitter: " + username );
		}
	
	
		void loginFailed( string error )
		{
			Debug.Log( "Twitter login failed: " + error );
		}
	
	
		void requestDidFailEvent( string error )
		{
			Debug.Log( "requestDidFailEvent: " + error );
		}
	
	
		void requestDidFinishEvent( object result )
		{
			if( result != null )
			{
				Debug.Log( "requestDidFinishEvent" );
				Prime31.Utils.logObject( result );
			}
		}
	
	
		void tweetSheetCompletedEvent( bool didSucceed )
		{
			Debug.Log( "tweetSheetCompletedEvent didSucceed: " + didSucceed );
		}
	
#endif
	}

}
