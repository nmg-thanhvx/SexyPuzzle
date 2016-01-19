using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Prime31;


#if UNITY_IOS || UNITY_ANDROID

#if UNITY_IOS
using FB = Prime31.FacebookBinding;
#else
using FB = Prime31.FacebookAndroid;
#endif



namespace Prime31
{
	public static class FacebookCombo
	{
		// Prepares the Facebook plugin for use
		public static void init()
		{
			FB.init();
		}


		// Gets the url used to launch the application. If no url was used returns string.Empty
		public static string getAppLaunchUrl()
		{
			return FB.getAppLaunchUrl();
		}


		// Authenticates the user with no additional permissions
		public static void login()
		{
			loginWithReadPermissions( new string[] {} );
		}


		// Authenticates the user for the provided read permissions
		public static void loginWithReadPermissions( string[] permissions )
		{
			FB.loginWithReadPermissions( permissions );
		}


		// Authenticates the user for the provided public permissions
		public static void loginWithPublishPermissions( string[] permissions )
		{
			FB.loginWithPublishPermissions( permissions );
		}


		// Checks to see if the current session is valid
		public static bool isSessionValid()
		{
			return FB.isSessionValid();
		}


		// Gets the current access token
		public static string getAccessToken()
		{
			return FB.getAccessToken();
		}


		// Gets the permissions granted to the current access token
		public static List<object> getSessionPermissions()
		{
			return FB.getSessionPermissions();
		}


		// Logs the user out and invalidates the token
		public static void logout()
		{
			FB.logout();
		}


		// Shows the native Facebook Share Dialog. Valid dictionary keys (from FBShareDialogParams) are: description, title, contentURL, imageURL
		public static void showFacebookShareDialog( Dictionary<string,object> parameters )
		{
			FB.showFacebookShareDialog( parameters );
		}


		// Shows the Facebook game request dialog. Note that Facebook has lots of rules about the combination of data sent so be sure to
		// read up on it: https://developers.facebook.com/docs/reference/ios/current/class/FBSDKGameRequestDialog/
		// Results in the gameDialogSucceeded/FailedEvent firing.
		public static void showGameRequestDialog( FacebookGameRequestContent content )
		{
			FB.showGameRequestDialog( content );
		}


		// Shows the Facebook app invite dialog
		public static void showAppInviteDialog( string appLinkUrl, string previewImageUrl = null )
		{
			FB.showAppInviteDialog( appLinkUrl, previewImageUrl );
		}


		#region App Events

		// Logs an event with optional parameters
		public static void logEvent( string eventName, Dictionary<string,object> parameters = null )
		{
			FB.logEvent( eventName, parameters );
		}


		// Logs an event, valueToSum and optional parameters
		public static void logEvent( string eventName, double valueToSum, Dictionary<string,object> parameters = null )
		{
			FB.logEvent( eventName, valueToSum, parameters );
		}

		#endregion

	}

}
#endif
