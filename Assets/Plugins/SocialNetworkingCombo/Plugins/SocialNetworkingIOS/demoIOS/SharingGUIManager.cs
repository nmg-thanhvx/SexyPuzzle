using UnityEngine;
using System.Collections;
using Prime31;




namespace Prime31
{
	public class SharingGUIManager : MonoBehaviourGUI
	{
#if UNITY_IOS
		public static string screenshotFilename = "someScreenshot.png";
	
	
		void Start()
		{
			// listen to the events fired by the SharingManager for illustration purposes
			SharingManager.sharingFinishedWithActivityTypeEvent += ( activityType ) =>
			{
				Debug.Log( "sharingFinishedWithActivityTypeEvent: " + activityType );
			};
	
			SharingManager.sharingCancelledEvent += () =>
			{
				Debug.Log( "sharingCancelledEvent" );
			};
	
			// grab a screenshot for later use
			Application.CaptureScreenshot( screenshotFilename );
		}
	
	
		void OnGUI()
		{
			beginColumn();
	
			if( GUILayout.Button( "Share URL and Text" ) )
			{
				// Note that calling setPopoverPosition is only required on iOS 8 when on an iPad
				SharingBinding.setPopoverPosition( Input.touches[0].position.x / 2f, ( Screen.height - Input.touches[0].position.y ) / 2f );
				SharingBinding.shareItems( new string[] { "http://prime31.com", "Here is some text with the URL" } );
			}
	
	
			if( GUILayout.Button( "Share Screenshot" ) )
			{
				var pathToImage = System.IO.Path.Combine( Application.persistentDataPath, screenshotFilename );
				if( !System.IO.File.Exists( pathToImage ) )
				{
					Debug.LogError( "there is no screenshot avaialable at path: " + pathToImage );
					return;
				}
	
				// Note that calling setPopoverPosition is only required on iOS 8 when on an iPad
				SharingBinding.setPopoverPosition( Input.touches[0].position.x / 2f, ( Screen.height - Input.touches[0].position.y ) / 2f );
				SharingBinding.shareItems( new string[] { pathToImage } );
			}
	
	
			if( GUILayout.Button( "Share Screenshot and Text" ) )
			{
				var pathToImage = System.IO.Path.Combine( Application.persistentDataPath, screenshotFilename );
				if( !System.IO.File.Exists( pathToImage ) )
				{
					Debug.LogError( "there is no screenshot avaialable at path: " + pathToImage );
					return;
				}
	
				// Note that calling setPopoverPosition is only required on iOS 8 when on an iPad
				SharingBinding.setPopoverPosition( Input.touches[0].position.x / 2f, ( Screen.height - Input.touches[0].position.y ) / 2f );
				SharingBinding.shareItems( new string[] { pathToImage, "Here is some text with the image" } );
			}
	
			endColumn();
	
	
			if( bottomRightButton( "Facebook..." ) )
			{
				Application.LoadLevel( "FacebookTestScene" );
			}
		}
	
#endif
	}

}
