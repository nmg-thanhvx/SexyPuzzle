using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using Prime31;



namespace Prime31
{
	public class FacebookGUIManager : MonoBehaviourGUI
	{
		public GameObject cube;

#if UNITY_IOS
		private string _userId;
		private bool _canUserUseFacebookComposer = false;
		private bool _hasPublishActions = false;

		public static string screenshotFilename = "someScreenshot.png";



		// common event handler used for all graph requests that logs the data to the console
		void completionHandler( string error, object result )
		{
			if( error != null )
				Debug.LogError( error );
			else
				Prime31.Utils.logObject( result );
		}


		void Start()
		{
			// dump custom data to log after a request completes
			FacebookManager.graphRequestCompletedEvent += result =>
			{
				Prime31.Utils.logObject( result );
			};

			// when the session opens or a reauth occurs we check the permissions to see if we can publish
			FacebookManager.sessionOpenedEvent += () =>
			{
				_hasPublishActions = FacebookBinding.getSessionPermissions().Contains( "publish_actions" );
			};

			// grab a screenshot for later use
			Application.CaptureScreenshot( screenshotFilename );

			// this is iOS 6 only!
			_canUserUseFacebookComposer = FacebookBinding.canUserUseFacebookComposer();

			// optionally enable logging of all requests that go through the Facebook class
			//Facebook.instance.debugRequests = true;
		}


		void OnGUI()
		{
			// center labels
			GUI.skin.label.alignment = TextAnchor.MiddleCenter;

			beginColumn();

			if( GUILayout.Button( "Initialize Facebook" ) )
			{
				FacebookBinding.init();
			}


			if( GUILayout.Button( "Login/Reauth with Read Perms" ) )
			{
				// Note: requesting publish permissions here will result in a crash. Only read permissions are permitted.
				var permissions = new string[] { "email", "user_friends" };
				FacebookBinding.loginWithReadPermissions( permissions );
			}


			if( GUILayout.Button( "Login/Reauth with Publish Perms" ) )
			{
				var permissions = new string[] { "publish_actions" };
				FacebookBinding.loginWithPublishPermissions( permissions );
			}


			if( GUILayout.Button( "Logout" ) )
			{
				FacebookBinding.logout();
			}


			if( GUILayout.Button( "Is Session Valid?" ) )
			{
				bool isLoggedIn = FacebookBinding.isSessionValid();
				Debug.Log( "Facebook is session valid: " + isLoggedIn );

				Facebook.instance.checkSessionValidityOnServer( isValid =>
				{
					Debug.Log( "checked session validity on server: " + isValid );
				});
			}


			if( GUILayout.Button( "Get Access Token" ) )
			{
				var token = FacebookBinding.getAccessToken();
				Debug.Log( "access token: " + token );
			}


			if( GUILayout.Button( "Get Granted Permissions" ) )
			{
				// This way of getting permissions uses the Facebook SDK. It is not always accurate since it does not hit the FB servers
				var permissions = FacebookBinding.getSessionPermissions();
				foreach( var perm in permissions )
					Debug.Log( perm );

				_hasPublishActions = permissions.Contains( "publish_actions" );

				// This way of getting the permissions hits Facebook's servers so it is certain to be valid.
				Facebook.instance.getSessionPermissionsOnServer( ( error, grantedPermissions ) =>
				{
					// check for a successful call then register if the publish_actions permissions is present
					if( grantedPermissions != null )
						_hasPublishActions = grantedPermissions.Contains( "publish_actions" );
				} );
			}


			if( GUILayout.Button( "Log App Event" ) )
			{
				var parameters = new Dictionary<string,object>
				{
					{ "someKey", 55 },
					{ "anotherKey", "string value" }
				};
				FacebookBinding.logEvent( "fb_mobile_add_to_cart", parameters );
			}


			endColumn( true );

			// toggle to show two different sets of buttons
			if( toggleButtonState( "Show More Actions" ) )
				secondColumnButtonsGUI();
			else
				secondColumnAdditionalButtonsGUI();
			toggleButton( "Show More Actions", "Toggle Buttons" );

			endColumn( false );


			if( bottomRightButton( "Twitter..." ) )
			{
				Application.LoadLevel( "TwitterTestScene" );
			}
		}


		private void secondColumnButtonsGUI()
		{
			if( _canUserUseFacebookComposer )
			{
				if( GUILayout.Button( "Show Facebook Composer" ) )
				{
					// ensure the image exists before attempting to add it!
					var pathToImage = Application.persistentDataPath + "/" + FacebookGUIManager.screenshotFilename;
					if( !System.IO.File.Exists( pathToImage ) )
						pathToImage = null;

					FacebookBinding.showFacebookComposer( "I'm posting this from Unity with a fancy image: " + Time.deltaTime, pathToImage, "http://prime31.com" );
				}
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
				FacebookBinding.showFacebookShareDialog( parameters );
			}


			if( GUILayout.Button( "Show App Invite Dialog" ) )
			{
				FacebookBinding.showAppInviteDialog( "fb208082439215773://prime31/test" );
			}


			if( GUILayout.Button( "Show Game Request Dialog" ) )
			{
				// prepare our content
				var content = new FacebookGameRequestContent();
				content.title = "The Dialog Title";
				content.message = "Play my neat game";
				// optional recipients. Make sure they are valid Facebook users unlike Don Johnson!
				//content.recipients.Add( "Don Johnson" );

				FacebookBinding.showGameRequestDialog( content );
			}


			if( GUILayout.Button( "Get App Friends" ) )
			{
				Facebook.instance.getFriends( completionHandler );
			}


			if( GUILayout.Button( "Graph Request (me)" ) )
			{
				Facebook.instance.getMe( ( error, result ) =>
				{
					// if we have an error we dont proceed any further
					if( error != null )
						return;

					if( result == null )
						return;

					// grab the userId and persist it for later use
					_userId = result.id;

					Debug.Log( "me Graph Request finished: " );
					Debug.Log( result );
				});
			}


			if( _userId != null )
			{
				if( GUILayout.Button( "Show Profile Image" ) )
				{
					Facebook.instance.fetchProfileImageForUserId( _userId, ( tex ) =>
					{
						cube.GetComponent<MeshRenderer>().material.mainTexture = tex;
					});
				}
			}
			else
			{
				GUILayout.Label( "Call the me Graph request to show user specific buttons" );
			}
		}


		private void secondColumnAdditionalButtonsGUI()
		{
			if( _hasPublishActions )
			{
				if( GUILayout.Button( "Post Image" ) )
				{
					var pathToImage = Application.persistentDataPath + "/" + FacebookGUIManager.screenshotFilename;
					#if UNITY_EDITOR
					pathToImage = Application.dataPath.Replace( "Assets", screenshotFilename );
					#endif
					if( !System.IO.File.Exists( pathToImage ) )
					{
						Debug.LogError( "there is no screenshot available at path: " + pathToImage );
						return;
					}

					var bytes = System.IO.File.ReadAllBytes( pathToImage );
					Facebook.instance.postImage( bytes, "im an image posted from iOS", completionHandler );
				}


				if( GUILayout.Button( "Post Message" ) )
				{
					Facebook.instance.postMessage( "im posting this from Unity: " + Time.deltaTime, completionHandler );
				}


				if( GUILayout.Button( "Post Message & Extras" ) )
				{
					Facebook.instance.postMessageWithLinkAndLinkToImage( "link post from Unity: " + Time.deltaTime, "http://prime31.com", "prime[31]", "http://prime31.com/assets/images/prime31logo.png", "Prime31 Logo", completionHandler );
				}


				if( GUILayout.Button( "Post Score" ) )
				{
					// note that posting a score requires publish_actions permissions!
					var parameters = new Dictionary<string,object>()
					{ { "score", "5677" } };
					Facebook.instance.graphRequest( "me/scores", HTTPVerb.POST, parameters, completionHandler );
				}
			}
			else
			{
				GUILayout.Label( "Login with publish_actions permissions to show posting buttons" );
			}


			if( GUILayout.Button( "Custom Graph Request: platform/posts" ) )
			{
				Facebook.instance.graphRequest( "platform/posts", HTTPVerb.GET, completionHandler );
			}


			if( GUILayout.Button( "Get Scores for me" ) )
			{
				Facebook.instance.getScores( "me", completionHandler );
			}
		}

#endif
	}

}
