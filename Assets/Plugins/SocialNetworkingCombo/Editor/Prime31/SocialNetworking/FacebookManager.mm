//
//  FacebookManager.m
//  Facebook
//
//  Created by Mike on 9/13/10.
//  Copyright 2010 Prime31 Studios. All rights reserved.
//

#import "FacebookManager.h"
#import <objc/runtime.h>
#import <Accounts/Accounts.h>
#import <Social/Social.h>
#if UNITY_VERSION >= 430
#include "AppDelegateListener.h"
#endif


NSString* const kFacebookUrlSchemeSuffixKey = @"kFacebookUrlSchemeKey";

void UnitySendMessage( const char * className, const char * methodName, const char * param );

UIViewController *UnityGetGLViewController();


@implementation FacebookManager

@synthesize urlSchemeSuffix, appLaunchUrl, hasFacebookId;

///////////////////////////////////////////////////////////////////////////////////////////////////
#pragma mark NSObject

+ (void)load
{
#if UNITY_VERSION >= 430
	UnityRegisterAppDelegateListener( (id)[self sharedManager] );
#endif
	[[NSNotificationCenter defaultCenter] addObserver:[self sharedManager]
											 selector:@selector(applicationDidFinishLaunching:)
												 name:UIApplicationDidFinishLaunchingNotification
											   object:nil];

	[[NSNotificationCenter defaultCenter] addObserver:[self sharedManager]
											 selector:@selector(applicationDidBecomeActive:)
												 name:UIApplicationDidBecomeActiveNotification
											   object:nil];
}


+ (FacebookManager*)sharedManager
{
	static dispatch_once_t pred;
	static FacebookManager *_sharedInstance = nil;

	dispatch_once( &pred, ^{ _sharedInstance = [[self alloc] init]; } );
	return _sharedInstance;
}


+ (BOOL)userCanUseFacebookComposer
{
	Class slComposer = NSClassFromString( @"SLComposeViewController" );
	if( slComposer && [SLComposeViewController isAvailableForServiceType:SLServiceTypeFacebook] )
		return YES;
	return NO;
}

- (id)init
{
	if( ( self = [super init] ) )
	{
		// if we have an appId or urlSchemeSuffix tucked away, set it now
		if( [[NSUserDefaults standardUserDefaults] objectForKey:kFacebookUrlSchemeSuffixKey] )
			self.urlSchemeSuffix = [[NSUserDefaults standardUserDefaults] objectForKey:kFacebookUrlSchemeSuffixKey];

		// check for FB App ID
		NSDictionary *dict = [[NSBundle mainBundle] infoDictionary];
		if( ![[dict allKeys] containsObject:@"FacebookAppID"] )
		{
			NSLog( @"You have not setup your Facebook app ID in the Info.plist file. Not having it in the Info.plist can cause your application to crash if you call any Facebook SDK method that requires it." );
			self.hasFacebookId = NO;
		}
		else
		{
			self.hasFacebookId = YES;
		}
		
		// check for a URL scheme
		if( ![[dict allKeys] containsObject:@"CFBundleURLTypes"] )
			NSLog( @"ERROR: You have not setup a URL scheme. Authentication via Safari or the Facebook.app will not work" );

		[self performSelector:@selector(publishPluginUsage) withObject:nil afterDelay:10];
		[self renewCredentialsForAllFacebookAccounts];
	}
	return self;
}


+ (FBSDKLoginManager*)sharedLoginManager
{
	static dispatch_once_t pred;
	static FBSDKLoginManager *_sharedInstance = nil;

	dispatch_once( &pred, ^{ _sharedInstance = [[FBSDKLoginManager alloc] init]; } );
	return _sharedInstance;
}


///////////////////////////////////////////////////////////////////////////////////////////////////
#pragma mark - NSNotifications

- (void)applicationDidFinishLaunching:(NSNotification*)note
{
	[[FBSDKApplicationDelegate sharedInstance] application:[UIApplication sharedApplication] didFinishLaunchingWithOptions:note.userInfo];
}


- (void)applicationDidBecomeActive:(NSNotification*)note
{
	[FBSDKAppEvents activateApp];
}


- (void)onOpenURL:(NSNotification*)notification
{
	NSURL* url = notification.userInfo[@"url"];

	if( url )
	{
		NSLog( @"url used to open app: %@", url );
		self.appLaunchUrl = url.absoluteString;
	}

	[[FBSDKApplicationDelegate sharedInstance] application:[UIApplication sharedApplication]
												   openURL:url
										 sourceApplication:notification.userInfo[@"sourceApplication"]
												annotation:notification.userInfo[@"annotation"]];
}


+ (BOOL)application:(UIApplication*)application openURL:(NSURL*)url sourceApplication:(NSString*)sourceApplication annotation:(id)annotation
{
	return [[FBSDKApplicationDelegate sharedInstance] application:application
												   openURL:url
										 sourceApplication:sourceApplication
												annotation:annotation];
}



///////////////////////////////////////////////////////////////////////////////////////////////////
#pragma mark Private/Internal

- (BOOL)handleOpenURL:(NSURL*)url sourceApplication:(NSString*)sourceApplication
{
	NSLog( @"url used to open app: %@", url );
	self.appLaunchUrl = url.absoluteString;

	return [[FBSDKApplicationDelegate sharedInstance] application:[UIApplication sharedApplication] didFinishLaunchingWithOptions:nil];
}


- (NSString*)getAppId
{
	return [[[NSBundle mainBundle] infoDictionary] objectForKey:@"FacebookAppID"];
}


- (NSString*)urlEncodeValue:(NSString*)str
{
	NSString *result = (NSString*)CFURLCreateStringByAddingPercentEscapes( kCFAllocatorDefault, (CFStringRef)str, NULL, CFSTR( "?=&+" ), kCFStringEncodingUTF8 );
	return [result autorelease];
}


- (void)publishPluginUsage
{
	dispatch_async( dispatch_get_global_queue( DISPATCH_QUEUE_PRIORITY_LOW, 0 ), ^
	{
		NSDictionary *dict = [NSDictionary dictionaryWithObjectsAndKeys:[self getAppId], @"appid",
							  @"prime31_socialnetworking", @"resource",
							  @"1.0.0", @"version", nil];

		// prep the post data
		NSString *post = [NSString stringWithFormat:@"plugin=featured_resources&payload=%@", [self urlEncodeValue:[P31 jsonStringFromObject:dict]]];
		NSData *postData = [post dataUsingEncoding:NSASCIIStringEncoding allowLossyConversion:YES];
		NSString *postLength = [NSString stringWithFormat:@"%d", postData.length];

		// prep the request
		NSMutableURLRequest *request = [[[NSMutableURLRequest alloc] init] autorelease];
		[request setURL:[NSURL URLWithString:@"https://www.facebook.com/impression.php"]];
		[request setHTTPMethod:@"POST"];
		[request setValue:postLength forHTTPHeaderField:@"Content-Length"];
		[request setValue:@"application/x-www-form-urlencoded" forHTTPHeaderField:@"Content-Type"];
		[request setHTTPBody:postData];

		// send the request
		NSURLResponse *response = nil;
		[NSURLConnection sendSynchronousRequest:request returningResponse:&response error:NULL];
	});
}


///////////////////////////////////////////////////////////////////////////////////////////////////
#pragma mark - SLComposer

- (void)showFacebookComposerWithMessage:(NSString*)message image:(UIImage*)image link:(NSString*)link
{
	if( ![FacebookManager userCanUseFacebookComposer] )
		return;

	if( !NSClassFromString( @"SLComposeViewController" ) )
		return;

	SLComposeViewController* slComposer = [SLComposeViewController composeViewControllerForServiceType:SLServiceTypeFacebook];

	// Add a message
	if( message )
		[slComposer setInitialText:message];

	// add an image
	if( image )
		[slComposer addImage:image];

	// add a link
	if( link )
		[slComposer addURL:[NSURL URLWithString:link]];

	// set a blocking handler for the tweet sheet
	slComposer.completionHandler = ^( SLComposeViewControllerResult result )
	{
		[P31 unityPause:NO];
		[UnityGetGLViewController() dismissViewControllerAnimated:YES completion:nil];

		if( result == SLComposeViewControllerResultDone )
			UnitySendMessage( "FacebookManager", "facebookComposerCompleted", "1" );
		else if( result == SLComposeViewControllerResultCancelled )
			UnitySendMessage( "FacebookManager", "facebookComposerCompleted", "0" );
	};

	// Show the tweet sheet
	[P31 unityPause:YES];
	[UnityGetGLViewController() presentViewController:slComposer animated:YES completion:nil];
}



///////////////////////////////////////////////////////////////////////////////////////////////////
#pragma mark Public

- (void)renewCredentialsForAllFacebookAccounts
{
	if( !NSClassFromString( @"ACAccountStore" ) )
		return;

	ACAccountStore *accountStore = [[ACAccountStore alloc] init];

	if( !accountStore )
		return;

    ACAccountType *accountTypeFB = [accountStore accountTypeWithAccountTypeIdentifier:ACAccountTypeIdentifierFacebook];
	NSArray *facebookAccounts = [accountStore accountsWithAccountType:accountTypeFB];

	if( facebookAccounts.count == 0 )
		return;

	for( ACAccount *fbAccount in facebookAccounts )
	{
		[accountStore renewCredentialsForAccount:fbAccount completion:^( ACAccountCredentialRenewResult renewResult, NSError *error )
		{
			/*
			if( error )
				NSLog( @"account %@ failed to renew: %@", fbAccount, error );
			else
				NSLog( @"account %@ renewed successfully", fbAccount );
			*/
		}];
	}
}

- (void)loginWithReadPermissions:(NSMutableArray*)permissions
{
	[[FacebookManager sharedLoginManager] logInWithReadPermissions:permissions fromViewController:UnityGetGLViewController() handler:^( FBSDKLoginManagerLoginResult* result, NSError* error )
	 {
		 if( error )
		 {
			 NSLog( @"session creation error: %@ userInfo: %@", error, error.userInfo ? error.userInfo : @"no userInfo" );
			 UnitySendMessage( "FacebookManager", "loginFailed", [P31 jsonFromError:error] );
		 }
		 else if( result.isCancelled )
		 {
			 UnitySendMessage( "FacebookManager", "loginFailed", "canceled" );
		 }
		 else
		 {
			 UnitySendMessage( "FacebookManager", "sessionOpened", result.token.tokenString.UTF8String );
		 }
	 }];
}


- (void)loginWithPublishPermissions:(NSMutableArray*)permissions
{
	[[FacebookManager sharedLoginManager] logInWithPublishPermissions:permissions fromViewController:UnityGetGLViewController() handler:^( FBSDKLoginManagerLoginResult* result, NSError* error )
	 {
		 if( error )
		 {
			 NSLog( @"session creation error: %@ userInfo: %@", error, error.userInfo ? error.userInfo : @"no userInfo" );
			 UnitySendMessage( "FacebookManager", "loginFailed", [P31 jsonFromError:error] );
		 }
		 else if( result.isCancelled )
		 {
			 UnitySendMessage( "FacebookManager", "loginFailed", "canceled" );
		 }
		 else
		 {
			 UnitySendMessage( "FacebookManager", "sessionOpened", result.token.tokenString.UTF8String );
		 }
	 }];
}


- (void)requestWithGraphPath:(NSString*)path httpMethod:(NSString*)method params:(NSDictionary*)params
{
	FBSDKGraphRequest *request = [[FBSDKGraphRequest alloc] initWithGraphPath:path parameters:params HTTPMethod:method];
	[request startWithCompletionHandler:^( FBSDKGraphRequestConnection* connection, id result, NSError* error)
	{
		if( error )
		{
			UnitySendMessage( "FacebookManager", "graphRequestFailed", [P31 jsonFromError:error] );
		}
		else
		{
			UnitySendMessage( "FacebookManager", "graphRequestCompleted", [P31 jsonStringFromObject:result].UTF8String );
		}
	}];
}



///////////////////////////////////////////////////////////////////////////////////////////////////
#pragma mark - FBSDKSharingDelegate

- (void)sharer:(id<FBSDKSharing>)sharer didCompleteWithResults:(NSDictionary *)results
{
	NSLog( @"Share Dialog completed with results: %@", results );
	if( results == nil )
		results = [NSDictionary dictionary];
	UnitySendMessage( "FacebookManager", "shareDialogSucceeded", [P31 jsonStringFromObject:results].UTF8String );
}


- (void)sharer:(id<FBSDKSharing>)sharer didFailWithError:(NSError *)error
{
	NSLog( @"Share Dialog error: %@", error );
	UnitySendMessage( "FacebookManager", "shareDialogFailed", [P31 jsonFromError:error] );
}


- (void)sharerDidCancel:(id<FBSDKSharing>)sharer
{
	UnitySendMessage( "FacebookManager", "shareDialogFailed", "Cancelled" );
}



///////////////////////////////////////////////////////////////////////////////////////////////////
#pragma mark - FBSDKAppInviteDialogDelegate

- (void)appInviteDialog:(FBSDKAppInviteDialog *)appInviteDialog didCompleteWithResults:(NSDictionary *)results
{
	NSLog( @"Share Dialog completed with results: %@", results );
	if( results == nil )
		results = [NSDictionary dictionary];
	UnitySendMessage( "FacebookManager", "shareDialogSucceeded", [P31 jsonStringFromObject:results].UTF8String );
}


- (void)appInviteDialog:(FBSDKAppInviteDialog *)appInviteDialog didFailWithError:(NSError *)error
{
	NSLog( @"Share Dialog error: %@", error );
	UnitySendMessage( "FacebookManager", "shareDialogFailed", [P31 jsonFromError:error] );
}



///////////////////////////////////////////////////////////////////////////////////////////////////
#pragma mark - FBSDKGameRequestDialogDelegate

- (void)gameRequestDialog:(FBSDKGameRequestDialog *)gameRequestDialog didCompleteWithResults:(NSDictionary *)results
{
	NSLog( @"game request dialog completed with results: %@", results );
	if( results == nil )
		results = [NSDictionary dictionary];
	UnitySendMessage( "FacebookManager", "gameDialogSucceeded", [P31 jsonStringFromObject:results].UTF8String );
}


- (void)gameRequestDialog:(FBSDKGameRequestDialog *)gameRequestDialog didFailWithError:(NSError *)error
{
	NSLog( @"Game Dialog error: %@", error );
	UnitySendMessage( "FacebookManager", "gameDialogFailed", [P31 jsonFromError:error] );
}


- (void)gameRequestDialogDidCancel:(FBSDKGameRequestDialog *)gameRequestDialog
{
	NSLog( @"Game Dialog canceled" );
	NSError* error = [NSError errorWithDomain:@"com.facebook.gamedialog" code:-1 userInfo:nil];
	UnitySendMessage( "FacebookManager", "gameDialogFailed", [P31 jsonFromError:error] );
}



@end


