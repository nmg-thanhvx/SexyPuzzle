using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Prime31;


#if UNITY_ANDROID
namespace Prime31
{
	public enum FacebookSessionDefaultAudience
	{
		None,
		OnlyMe,
		Friends,
		Everyone
	}


	public enum FacebookSessionLoginBehavior
	{
		SSO_ONLY,
		SSO_WITH_FALLBACK,
		SUPPRESS_SSO
	}

	public class FacebookAndroid
	{
		private static AndroidJavaObject _facebookPlugin;


		static FacebookAndroid()
		{
			if( Application.platform != RuntimePlatform.Android )
				return;

			// find the plugin instance
			using( var pluginClass = new AndroidJavaClass( "com.prime31.FacebookPlugin" ) )
				_facebookPlugin = pluginClass.CallStatic<AndroidJavaObject>( "instance" );

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


		// Initializes the Facebook plugin for your application and optionally prints the key hash for this specific
		// apk. The key hash is useful to ensure that your application is properly setup on the Facebook web site
		public static void init( bool printKeyHash = true )
		{
			if( Application.platform != RuntimePlatform.Android )
				return;

			_facebookPlugin.Call( "init", printKeyHash );
			Facebook.instance.accessToken = getAccessToken();
		}


		// Gets the url used to launch the application. If no url was used returns string.Empty. This is equivelant to the Intent.getData() method on the native side.
		public static string getAppLaunchUrl()
		{
			if( Application.platform != RuntimePlatform.Android )
				return string.Empty;

			return _facebookPlugin.Call<string>( "getAppLaunchUrl" );
		}


		// Sets the login behavior for the session. Call this before logging a user in.
		public static void setSessionLoginBehavior( FacebookSessionLoginBehavior loginBehavior )
		{
			if( Application.platform != RuntimePlatform.Android )
				return;

			_facebookPlugin.Call( "setSessionLoginBehavior", loginBehavior.ToString() );
		}


		// Sets the default audience for the authentication system. Call this before logging a user in.
		public static void setDefaultAudience( FacebookSessionDefaultAudience defaultAudience )
		{
			if( Application.platform != RuntimePlatform.Android )
				return;

			_facebookPlugin.Call( "setDefaultAudience", defaultAudience.ToString() );
		}

		// Checks to see if the current session is valid
		public static bool isSessionValid()
		{
			if( Application.platform != RuntimePlatform.Android )
				return false;

			return _facebookPlugin.Call<bool>( "isSessionValid" );
		}


		// Gets the current access token
		public static string getAccessToken()
		{
			if( Application.platform != RuntimePlatform.Android )
				return string.Empty;

			return _facebookPlugin.Call<string>( "getAccessToken" );
		}


		// Gets the permissions granted to the current access token
		public static List<object> getSessionPermissions()
		{
			if( Application.platform == RuntimePlatform.Android )
			{
				var permissions = _facebookPlugin.Call<string>( "getSessionPermissions" );
				return permissions.listFromJson();
			}

			return new List<object>();
		}


		public static void login()
		{
			loginWithReadPermissions( new string[] {} );
		}


		// Authenticates the user requesting the passed in read permissions.
		// Results in the sessionOpenedEvent or loginFailedEvent firing.
		public static void loginWithReadPermissions( string[] permissions )
		{
			if( Application.platform != RuntimePlatform.Android )
				return;

			_facebookPlugin.Call( "loginWithReadPermissions", new object[] { permissions } );
		}


		// Authenticates the user requesting the passed in publish permissions.
		// Results in the sessionOpenedEvent or loginFailedEvent firing.
		public static void loginWithPublishPermissions( string[] permissions )
		{
			if( Application.platform != RuntimePlatform.Android )
				return;

			_facebookPlugin.Call( "loginWithPublishPermissions", new object[] { permissions } );
		}


		// Logs the user out and invalidates the token
		public static void logout()
		{
			if( Application.platform != RuntimePlatform.Android )
				return;

			_facebookPlugin.Call( "logout" );
			Facebook.instance.accessToken = string.Empty;
		}


		// Shows the native Facebook Share Dialog. Valid dictionary keys are: description, title, contentURL, imageURL.
		// Results in the shareDialogSucceeded/FailedEvent firing.
		public static void showFacebookShareDialog( Dictionary<string,object> parameters )
		{
			if( Application.platform != RuntimePlatform.Android )
				return;

			_facebookPlugin.Call( "showFacebookShareDialog", parameters.toJson() );
		}


		// Shows the Facebook app invite dialog. Results in the shareDialogSucceeded/FailedEvent firing.
		public static void showAppInviteDialog( string appLinkUrl, string previewImageUrl = null )
		{
			if( Application.platform != RuntimePlatform.Android )
				return;

			_facebookPlugin.Call( "showAppInviteDialog", appLinkUrl, previewImageUrl );
		}


		// Shows the Facebook game request dialog. Note that Facebook has lots of rules about the combination of data sent so be sure to
		// read up on it: https://developers.facebook.com/docs/reference/ios/current/class/FBSDKGameRequestDialog/
		// Results in the gameDialogSucceeded/FailedEvent firing.
		public static void showGameRequestDialog( FacebookGameRequestContent content )
		{
			if( Application.platform != RuntimePlatform.Android )
				return;

			_facebookPlugin.Call( "showGameRequestDialog", Json.encode( content ) );
		}


		// Calls a custom Graph API method with the key/value pairs in the Dictionary.  Pass in a null dictionary if no parameters are needed.
		public static void graphRequest( string graphPath, string httpMethod, Dictionary<string,string> parameters )
		{
			if( Application.platform != RuntimePlatform.Android )
				return;

			parameters = parameters ?? new Dictionary<string,string>();

			// call off to java land
			if( !isSessionValid() )
				babysitRequest( true, () => { _facebookPlugin.Call( "graphRequest", graphPath, httpMethod, parameters.toJson() ); } );
			else
				_facebookPlugin.Call( "graphRequest", graphPath, httpMethod, parameters.toJson() );
		}


		#region Facebook App Events

		// Logs an event with optional parameters
		public static void logEvent( string eventName, Dictionary<string,object> parameters = null )
		{
			if( Application.platform != RuntimePlatform.Android )
				return;

			if( parameters != null )
				_facebookPlugin.Call( "logEventWithParameters", eventName, Json.encode( parameters ) );
			else
				_facebookPlugin.Call( "logEvent", eventName );
		}


		// logs a purchase event
		public static void logPurchaseEvent( double amount, string currency )
		{
			if( Application.platform != RuntimePlatform.Android )
				return;

			_facebookPlugin.Call( "logPurchaseEvent", amount, currency );
		}


		// Logs an event, valueToSum and optional parameters
		public static void logEvent( string eventName, double valueToSum, Dictionary<string,object> parameters = null )
		{
			if( Application.platform != RuntimePlatform.Android )
				return;

			if( parameters != null )
				_facebookPlugin.Call( "logEventAndValueToSumWithParameters", eventName, valueToSum, Json.encode( parameters ) );
			else
				_facebookPlugin.Call( "logEventAndValueToSum", eventName, valueToSum );
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
			FacebookAndroid.login();
		}


		#region Event handlers

		void sessionOpenedEvent()
		{
			// do we need publish permissions?
			if( requiresPublishPermissions && !FacebookAndroid.getSessionPermissions().Contains( "publish_actions" ) )
			{
				FacebookAndroid.loginWithPublishPermissions( new string[] { "publish_actions" } );
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
