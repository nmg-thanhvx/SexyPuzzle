using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Prime31;


#if UNITY_IOS
namespace Prime31
{
	public enum FacebookSessionDefaultAudience
	{
		None = 0,
		OnlyMe = 10,
		Friends = 20,
		Everyone = 30
	}

	public enum FacebookSessionLoginBehavior
	{
		Native,
		Browser,
		SystemAccount,
		Web
	}


	public class FacebookBinding
	{
		static FacebookBinding()
		{
			// on login, set the access token
			FacebookManager.preLoginSucceededEvent += () =>
			{
				Facebook.instance.accessToken = getAccessToken();
			};
		}


		// internal doesn't really do anything here due to this being in the firstpass DLL but it does keep our test harness from touching it
		// you should never need to call this method. The plugin will call it for you when necessary
		internal static void babysitRequest( bool requiresPublishPermissions, Action afterAuthAction )
		{
			new FacebookAuthHelper( requiresPublishPermissions, afterAuthAction ).start();
		}


	    [DllImport("__Internal")]
	    private static extern void _facebookInit();

		// Initializes the Facebook plugin for your application
	    public static void init()
	    {
	        if( Application.platform == RuntimePlatform.IPhonePlayer )
				_facebookInit();

			// grab the access token in case it is saved
			Facebook.instance.accessToken = getAccessToken();
	    }


		[DllImport("__Internal")]
		private static extern string _facebookGetAppLaunchUrl();

		// Gets the url used to launch the application. If no url was used returns string.Empty
		public static string getAppLaunchUrl()
		{
			if( Application.platform == RuntimePlatform.IPhonePlayer )
				return _facebookGetAppLaunchUrl();

			return string.Empty;
		}


		[DllImport("__Internal")]
		private static extern void _facebookSetSessionLoginBehavior( int behavior );

		// Sets the login behavior. Must be called before any login calls! Understand what the login behaviors are and how they work before using this!
	    public static void setSessionLoginBehavior( FacebookSessionLoginBehavior loginBehavior )
	    {
	        if( Application.platform == RuntimePlatform.IPhonePlayer )
				_facebookSetSessionLoginBehavior( (int)loginBehavior );
	    }


	    [DllImport("__Internal")]
	    private static extern void _facebookRenewCredentialsForAllFacebookAccounts();

		// iOS 6+ only. Renews the credentials that iOS stores for any native Facebook accounts. You can safely call this at app launch or when logging a user out.
	    public static void renewCredentialsForAllFacebookAccounts()
	    {
	        if( Application.platform == RuntimePlatform.IPhonePlayer )
				_facebookRenewCredentialsForAllFacebookAccounts();
	    }


	    [DllImport("__Internal")]
	    private static extern bool _facebookIsLoggedIn();

		// Checks to see if the current session is valid
	    public static bool isSessionValid()
	    {
	        if( Application.platform == RuntimePlatform.IPhonePlayer )
				return _facebookIsLoggedIn();
			return false;
	    }


		[DllImport("__Internal")]
		private static extern string _facebookGetFacebookAccessToken();

		// Gets the current access token
		public static string getAccessToken()
		{
			if( Application.platform == RuntimePlatform.IPhonePlayer )
				return _facebookGetFacebookAccessToken();

			return string.Empty;
		}


		[DllImport("__Internal")]
		private static extern string _facebookGetSessionPermissions();

		// Gets the permissions granted to the current access token
		public static List<object> getSessionPermissions()
		{
			if( Application.platform == RuntimePlatform.IPhonePlayer )
				return Json.decode<List<object>>( _facebookGetSessionPermissions() );

			return new List<object>();
		}


		// Opens the Facebook single sign on login in Safari, the official Facebook app or uses iOS 6 Accounts if available
	    public static void login()
	    {
	        loginWithReadPermissions( new string[] { "email" } );
	    }


	    [DllImport("__Internal")]
		private static extern void _facebookLoginWithReadPermissions( string perms );

		// Shows the native authorization dialog, opens the Facebook single sign on login in Safari or the official Facebook app with the requested read (not publish!) permissions.
		// Results in the sessionOpenedEvent or loginFailedEvent firing.
	    public static void loginWithReadPermissions( string[] permissions )
	    {
	        if( Application.platform == RuntimePlatform.IPhonePlayer )
			{
				string permissionsString = null;
				if( permissions == null || permissions.Length == 0 )
					permissionsString = string.Empty;
				else
					permissionsString = string.Join( ",", permissions );

				_facebookLoginWithReadPermissions( permissionsString );
			}
	    }


	    [DllImport("__Internal")]
		private static extern void _facebookLoginWithPublishPermissions( string perms );

		// Authenticates/reauthorizes with the requested publish permissions.
		// Results in the sessionOpenedEvent or loginFailedEvent firing.
		public static void loginWithPublishPermissions( string[] permissions )
	    {
	        if( Application.platform == RuntimePlatform.IPhonePlayer )
			{
				var permissionsString = string.Join( ",", permissions );
				_facebookLoginWithPublishPermissions( permissionsString );
			}
	    }


	    [DllImport("__Internal")]
	    private static extern void _facebookLogout();

		// Logs the user out and invalidates the token
	    public static void logout()
	    {
	        if( Application.platform == RuntimePlatform.IPhonePlayer )
				_facebookLogout();

			Facebook.instance.accessToken = string.Empty;
	    }


	    [DllImport("__Internal")]
	    private static extern void _facebookGraphRequest( string graphPath, string httpMethod, string jsonDict );

		// Allows you to use any available Facebook Graph API method
	    public static void graphRequest( string graphPath, string httpMethod, Dictionary<string,object> keyValueHash )
	    {
	        if( Application.platform == RuntimePlatform.IPhonePlayer )
			{
				// convert the Dictionary to JSON
				string jsonDict = keyValueHash.toJson();
				if( jsonDict != null )
				{
					// if we arent logged in start up the babysitter
					if( !isSessionValid() )
					{
						babysitRequest( true, () => { _facebookGraphRequest( graphPath, httpMethod, jsonDict ); } );
					}
					else
					{
						_facebookGraphRequest( graphPath, httpMethod, jsonDict );
					}
				}
	 		}
		}


		#region iOS6 Facebook composer and Share Dialog

		[DllImport("__Internal")]
		private static extern bool _facebookCanUserUseFacebookComposer();

		// Checks to see if the user is using a version of iOS that supports the Facebook composer and if they have an account setup
		public static bool canUserUseFacebookComposer()
		{
			if( Application.platform == RuntimePlatform.IPhonePlayer )
				return _facebookCanUserUseFacebookComposer();

			return false;
		}


		[DllImport("__Internal")]
		private static extern void _facebookShowFacebookComposer( string message, string imagePath, string link );

		public static void showFacebookComposer( string message )
		{
			showFacebookComposer( message, null, null );
		}


		// Shows the Facebook composer with optional image path and link
		public static void showFacebookComposer( string message, string imagePath, string link )
		{
			if( Application.platform == RuntimePlatform.IPhonePlayer )
				_facebookShowFacebookComposer( message, imagePath, link );
		}


		[DllImport("__Internal")]
		private static extern void _facebookShowFacebookShareDialog( string json );

		// Shows the Facebook share dialog. Valid dictionary keys (from FBSDKShareLinkContent) are: description, title, contentURL, imageURL.
		// Results in the shareDialogSucceeded/FailedEvent firing.
		public static void showFacebookShareDialog( Dictionary<string,object> parameters )
		{
			if( Application.platform == RuntimePlatform.IPhonePlayer )
				_facebookShowFacebookShareDialog( parameters.toJson() );
		}


		[DllImport("__Internal")]
		private static extern void _facebookShowGameRequestDialog( string json );

		// Shows the Facebook game request dialog. Note that Facebook has lots of rules about the combination of data sent so be sure to
		// read up on it: https://developers.facebook.com/docs/reference/ios/current/class/FBSDKGameRequestDialog/
		// Results in the gameDialogSucceeded/FailedEvent firing.
		public static void showGameRequestDialog( FacebookGameRequestContent content )
		{
			if( Application.platform == RuntimePlatform.IPhonePlayer )
				_facebookShowGameRequestDialog( Json.encode( content ) );
		}


		[DllImport("__Internal")]
		private static extern void _facebookShowAppInviteDialog( string appLinkUrl, string previewImageUrl );

		// Shows the Facebook app invite dialog. Results in the shareDialogSucceeded/FailedEvent firing.
		public static void showAppInviteDialog( string appLinkUrl, string previewImageUrl = null )
		{
			if( Application.platform == RuntimePlatform.IPhonePlayer )
				_facebookShowAppInviteDialog( appLinkUrl, previewImageUrl );
		}

		#endregion


		#region Facebook App Events

		[DllImport("__Internal")]
		private static extern void _facebookLogEvent( string eventName );

		[DllImport("__Internal")]
		private static extern void _facebookLogEventWithParameters( string eventName, string json );

		// Logs an event with optional parameters
		public static void logEvent( string eventName, Dictionary<string,object> parameters = null )
		{
			if( Application.platform != RuntimePlatform.IPhonePlayer )
				return;

			if( parameters != null )
				_facebookLogEventWithParameters( eventName, Json.encode( parameters ) );
			else
				_facebookLogEvent( eventName );
		}


		[DllImport("__Internal")]
		private static extern void _facebookLogPurchaseEvent( double amount, string currency );

		// logs a purchase event
		public static void logPurchaseEvent( double amount, string currency )
		{
			if( Application.platform != RuntimePlatform.IPhonePlayer )
				return;

			_facebookLogPurchaseEvent( amount, currency );
		}


		[DllImport("__Internal")]
		private static extern void _facebookLogEventAndValueToSum( string eventName, double valueToSum );

		[DllImport("__Internal")]
		private static extern void _facebookLogEventAndValueToSumWithParameters( string eventName, double valueToSum, string json );

		// Logs an event, valueToSum and optional parameters
		public static void logEvent( string eventName, double valueToSum, Dictionary<string,object> parameters = null )
		{
			if( Application.platform != RuntimePlatform.IPhonePlayer )
				return;

			if( parameters != null )
				_facebookLogEventAndValueToSumWithParameters( eventName, valueToSum, Json.encode( parameters ) );
			else
				_facebookLogEventAndValueToSum( eventName, valueToSum );
		}

		#endregion

	}



	#region AuthHelper babysitter

	public class FacebookAuthHelper
	{
		public Action afterAuthAction;
		public bool requiresPublishPermissions;

		#pragma warning disable
		private static FacebookAuthHelper _instance;
		#pragma warning restore


		public FacebookAuthHelper( bool requiresPublishPermissions, Action afterAuthAction )
		{
			_instance = this;
			this.requiresPublishPermissions = requiresPublishPermissions;
			this.afterAuthAction = afterAuthAction;

			// login
			FacebookManager.sessionOpenedEvent += sessionOpenedEvent;
			FacebookManager.loginFailedEvent += loginFailedEvent;
		}


		~FacebookAuthHelper()
		{
			cleanup();
		}


		public void cleanup()
		{
			// if the afterAuthAction is not null we have not yet cleaned up
			if( afterAuthAction != null )
			{
				FacebookManager.sessionOpenedEvent -= sessionOpenedEvent;
				FacebookManager.loginFailedEvent -= loginFailedEvent;
			}

			_instance = null;
		}


		public void start()
		{
			FacebookBinding.login();
		}


		#region Event handlers

		void sessionOpenedEvent()
		{
			// do we need publish permissions?
			if( requiresPublishPermissions && !FacebookBinding.getSessionPermissions().Contains( "publish_actions" ) )
			{
				FacebookBinding.loginWithPublishPermissions( new string[] { "publish_actions" } );
			}
			else
			{
				afterAuthAction();
				cleanup();
			}
		}


		void loginFailedEvent( P31Error error )
		{
			cleanup();
		}

		#endregion

	}

	#endregion
}
#endif