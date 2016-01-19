using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
using Prime31;



namespace Prime31
{
	public class FacebookManager : AbstractManager
	{
#if UNITY_ANDROID || UNITY_IOS
		// Fired after a successful login attempt was made
		public static event Action sessionOpenedEvent;

		// Fired just before the login succeeded event. For interal use only.
		public static event Action preLoginSucceededEvent;

		// Fired when an error occurs while logging in
		public static event Action<P31Error> loginFailedEvent;

		// Fired when a graph request finishes
		public static event Action<object> graphRequestCompletedEvent;

		// Fired when a graph request fails
		public static event Action<P31Error> graphRequestFailedEvent;

		// iOS only. Fired when the Facebook composer completes. True indicates success and false cancel/failure.
		public static event Action<bool> facebookComposerCompletedEvent;

		// Fired when the share dialog succeeds. Includes the postId of the new post.
		public static event Action<string> shareDialogSucceededEvent;

		// Fired when the share dialog fails
		public static event Action<P31Error> shareDialogFailedEvent;

		// Fired when the game dialog succeeds. Includes the requestId (request field) and recipients (to field)
		public static event Action<Dictionary<string,object>> gameDialogSucceededEvent;

		// Fired when the game dialog fails
		public static event Action<P31Error> gameDialogFailedEvent;



		static FacebookManager()
		{
			AbstractManager.initialize( typeof( FacebookManager ) );
		}



		public void sessionOpened( string accessToken )
		{
			preLoginSucceededEvent.fire();
			Facebook.instance.accessToken = accessToken;

			sessionOpenedEvent.fire();
		}


		public void loginFailed( string json )
		{
			loginFailedEvent.fire( P31Error.errorFromJson( json ) );
		}


		public void graphRequestCompleted( string json )
		{
			if( graphRequestCompletedEvent != null )
			{
				object obj = Prime31.Json.decode( json );
				graphRequestCompletedEvent.fire( obj );
			}
		}


		public void graphRequestFailed( string json )
		{
			graphRequestFailedEvent.fire( P31Error.errorFromJson( json ) );
		}


		// iOS only
		public void facebookComposerCompleted( string result )
		{
			facebookComposerCompletedEvent.fire( result == "1" );
		}


		public void shareDialogFailed( string json )
		{
			shareDialogFailedEvent.fire( P31Error.errorFromJson( json ) );
		}


		public void shareDialogSucceeded( string postId )
		{
			shareDialogSucceededEvent.fire( postId );
		}


		public void gameDialogFailed( string json )
		{
			gameDialogFailedEvent.fire( P31Error.errorFromJson( json ) );
		}


		public void gameDialogSucceeded( string json )
		{
			gameDialogSucceededEvent.fire( json.dictionaryFromJson() );
		}

#endif
	}

}
