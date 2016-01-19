using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Prime31;




namespace Prime31
{
	public class FacebookUIManager : MonoBehaviourGUI
	{
#if UNITY_ANDROID
		public static string screenshotFilename = "someScreenshot.png";


		// common event handler used for all Facebook graph requests that logs the data to the console
		void completionHandler( string error, object result )
		{
			if( error != null )
				Debug.LogError( error );
			else
				Prime31.Utils.logObject( result );
		}


		void Start()
		{
			// grab a screenshot for later use
			Application.CaptureScreenshot( screenshotFilename );

			// optionally enable logging of all requests that go through the Facebook class
			//Facebook.instance.debugRequests = true;
		}


		void OnGUI()
		{
			beginColumn();


			if( GUILayout.Button( "Initialize Facebook" ) )
			{
				FacebookAndroid.init();
			}


			if( GUILayout.Button( "Set Login Behavior to SUPPRESS_SSO" ) )
			{
				FacebookAndroid.setSessionLoginBehavior( FacebookSessionLoginBehavior.SUPPRESS_SSO );
			}


			if( GUILayout.Button( "Login/Reauth with Read Perms" ) )
			{
				FacebookAndroid.loginWithReadPermissions( new string[] { "email", "user_birthday", "user_friends" } );
			}


			if( GUILayout.Button( "Login/Reauth with Publish Perms" ) )
			{
				FacebookAndroid.loginWithPublishPermissions( new string[] { "publish_actions" } );
			}


			if( GUILayout.Button( "Logout" ) )
			{
				FacebookAndroid.logout();
			}


			if( GUILayout.Button( "Is Session Valid?" ) )
			{
				var isSessionValid = FacebookAndroid.isSessionValid();
				Debug.Log( "Is session valid?: " + isSessionValid );
			}


			if( GUILayout.Button( "Get Session Token" ) )
			{
				var token = FacebookAndroid.getAccessToken();
				Debug.Log( "session token: " + token );
			}


			if( GUILayout.Button( "Get Granted Permissions" ) )
			{
				var permissions = FacebookAndroid.getSessionPermissions();
				Debug.Log( "granted permissions: " + permissions.Count );
				Prime31.Utils.logObject( permissions );
			}


			endColumn( true );


			if( GUILayout.Button( "Post Image" ) )
			{
				var pathToImage = Application.persistentDataPath + "/" + screenshotFilename;
				var bytes = System.IO.File.ReadAllBytes( pathToImage );

				Facebook.instance.postImage( bytes, "im an image posted from Android", completionHandler );
			}


			if( GUILayout.Button( "Graph Request (me)" ) )
			{
				Facebook.instance.graphRequest( "me", completionHandler );
			}


			if( GUILayout.Button( "Post Message" ) )
			{
				Facebook.instance.postMessage( "im posting this from Unity: " + Time.deltaTime, completionHandler );
			}


			if( GUILayout.Button( "Post Message & Extras" ) )
			{
				Facebook.instance.postMessageWithLinkAndLinkToImage( "link post from Unity: " + Time.deltaTime, "http://prime31.com", "prime[31]", "http://prime31.com/assets/images/prime31logo.png", "Prime31 Logo", completionHandler );
			}


			if( GUILayout.Button( "Show Share Dialog" ) )
			{
				var parameters = new Dictionary<string,object>
				{
					{ "contentURL", "http://prime31.com" },
					{ "title", "title goes here" },
					{ "imageURL", "http://prime31.com/assets/images/prime31logo.png" },
					{ "description", "description of what this share dialog is all about" }
				};
				FacebookAndroid.showFacebookShareDialog( parameters );
			}


			if( GUILayout.Button( "Show App Invite Dialog" ) )
			{
				FacebookAndroid.showAppInviteDialog( "//applinks?al_applink_data=%7B%22user_agent%22%3A%22Bolts%20iOS%201.0.0%22%2C%22target_url%22%3A%22http%3A%5C%2F%5C%2Fexample.com%5C%2Fapplinks%22%2C%22extras%22%3A%7B%22myapp_token%22%3A%22t0kEn%22%7D%7D" );
			}


			if( GUILayout.Button( "Show Game Request Dialog" ) )
			{
				// prepare our content
				var content = new FacebookGameRequestContent();
				content.title = "The Dialog Title";
				content.message = "Play my neat game";
				// optional recipients. Make sure they are valid Facebook users unlike Don Johnson!
				//content.recipients.Add( "Don Johnson" );

				FacebookAndroid.showGameRequestDialog( content );
			}


			if( GUILayout.Button( "Get Friends" ) )
			{
				Facebook.instance.getFriends( completionHandler );
			}


			if( GUILayout.Button( "Log App Event" ) )
			{
				var parameters = new Dictionary<string,object>
				{
					{ "someKey", 55 },
					{ "anotherKey", "string value" }
				};
				FacebookAndroid.logEvent( "fb_mobile_add_to_cart", parameters );
			}


			endColumn();


			if( bottomLeftButton( "Twitter Scene" ) )
			{
				Application.LoadLevel( "TwitterTestScene" );
			}
		}

#endif
	}

}
