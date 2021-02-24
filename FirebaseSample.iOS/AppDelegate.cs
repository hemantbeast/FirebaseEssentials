using System;
using FirebaseEssentials;
using FirebaseEssentials.iOS;
using Foundation;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

namespace FirebaseSample.iOS
{
	[Register("AppDelegate")]
	public partial class AppDelegate : FormsApplicationDelegate
	{
		public override bool FinishedLaunching(UIApplication app, NSDictionary options)
		{
			Forms.Init();

			CrossFirebaseEssentials.Notifications = new FirebasePushNotificationManager();
			CrossFirebaseEssentials.Analytics = new FirebaseAnalyticsManager();
			CrossFirebaseEssentials.Crashlytics = new FirebaseCrashlyticsManager();
			CrossFirebaseEssentials.Authentication = new FirebaseAuthenticationManager();

			FirebaseEssentialsManager.Initialize(options);

			LoadApplication(new App());
			return base.FinishedLaunching(app, options);
		}

		public override void RegisteredForRemoteNotifications(UIApplication application, NSData deviceToken)
		{
			FirebasePushNotificationManager.DidRegisterRemoteNotifications(deviceToken);
		}

		public override void FailedToRegisterForRemoteNotifications(UIApplication application, NSError error)
		{
			FirebasePushNotificationManager.RemoteNotificationRegistrationFailed(error);
		}

		public override void DidReceiveRemoteNotification(UIApplication application, NSDictionary userInfo, Action<UIBackgroundFetchResult> completionHandler)
		{
			FirebasePushNotificationManager.DidReceiveMessage(userInfo);
			Console.WriteLine(userInfo);
			completionHandler(UIBackgroundFetchResult.NewData);
		}

		public override bool OpenUrl(UIApplication app, NSUrl url, NSDictionary options)
		{
			return FirebaseAuthenticationManager.OpenUrl(app, url, options);
		}
	}
}
