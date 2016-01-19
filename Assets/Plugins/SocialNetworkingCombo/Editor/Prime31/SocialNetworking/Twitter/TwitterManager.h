//
//  P31Twitter.h
//  SocialNetworking
//
//  Created by prime31
//

#import <Foundation/Foundation.h>
#include "P31SharedTools.h"


@class OAToken;

@interface TwitterManager : NSObject
@property (nonatomic, copy) NSString *consumerKey;
@property (nonatomic, copy) NSString *consumerSecret;



+ (TwitterManager*)sharedManager;

+ (BOOL)isTweetSheetSupported;

+ (BOOL)userCanTweet;



// these next methods are used by Xauth and Oauth methods
- (NSString*)extractUsernameFromHTTPBody:(NSString*)body;

- (void)completeLoginWithResponseData:(NSString*)data;

- (void)storeAccountIdentifier:(NSString*)accountIdentifier;



- (void)unpauseUnity;

- (BOOL)isLoggedIn;

- (NSString*)loggedInUsername;

- (void)showOauthLoginDialog;

- (void)logout;

- (void)postStatusUpdate:(NSString*)status withImageAtPath:(NSString*)path;

- (void)postStatusUpdate:(NSString*)status withImage:(UIImage*)image;

- (void)performRequest:(NSString*)methodType path:(NSString*)path params:(NSDictionary*)params;

- (void)showTweetComposerWithMessage:(NSString*)message image:(UIImage*)image link:(NSString*)link;

@end
