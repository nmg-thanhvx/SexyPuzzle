using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Prime31;


#if UNITY_IOS

namespace Prime31
{
	public class SharingBinding
	{
	    [DllImport("__Internal")]
	    private static extern void _sharingShareItems( string items, string excludedActivityTypes );
	
		// Shows the share sheet with the given items. Items can be text, urls or full and proper paths to sharable files
	    public static void shareItems( string[] items )
	    {
	        if( Application.platform == RuntimePlatform.IPhonePlayer )
				_sharingShareItems( Json.encode( items ), null );
	    }
	
	
		// Shows the share sheet with the given items with a list of excludedActivityTypes. See Apple's docs for more information on excludedActivityTypes.
	    public static void shareItems( string[] items, string[] excludedActivityTypes )
	    {
	        if( Application.platform == RuntimePlatform.IPhonePlayer )
				_sharingShareItems( Json.encode( items ), Json.encode( excludedActivityTypes ) );
	    }
	
	
		[DllImport("__Internal")]
		private static extern void _sharingSetPopoverPosition( float x, float y );
	
		// iOS 8+ only and iPad only. Sets the popover arrow position for displaying the share sheet. Set this to match your share button location.
		public static void setPopoverPosition( float x, float y )
		{
			if( Application.platform == RuntimePlatform.IPhonePlayer )
				_sharingSetPopoverPosition( x, y );
		}
	
	}

}
#endif
