using System;
using Android.App;
using Android.OS;
using Android.Runtime;
using FirebaseEssentials;
using FirebaseEssentials.Droid;

namespace FirebaseSample.Droid
{
	[Application]
	public class MainApplication : Application
	{
		public MainApplication(IntPtr handle, JniHandleOwnership transer) : base(handle, transer)
		{
		}

		public override void OnCreate()
		{
			base.OnCreate();

			CrossFirebaseEssentials.Notifications = new FirebasePushNotificationManager();
			CrossFirebaseEssentials.Analytics = new FirebaseAnalyticsManager();
			CrossFirebaseEssentials.Crashlytics = new FirebaseCrashlyticsManager();
			CrossFirebaseEssentials.Authentication = new FirebaseAuthenticationManager();

			//Set the default notification channel for your app when running Android Oreo
			if (Build.VERSION.SdkInt >= BuildVersionCodes.O) {
				//Change for your default notification channel id here
				FirebasePushNotificationManager.DefaultNotificationChannelId = "DefaultChannel";

				//Change for your default notification channel name here
				FirebasePushNotificationManager.DefaultNotificationChannelName = "General";
			}

			FirebaseEssentialsManager.Initialize(this);

			//Handle notification when app is closed here
			CrossFirebaseEssentials.Notifications.OnNotificationReceived += (s, p) => {
				System.Diagnostics.Debug.WriteLine("Notification: " + p.Data);
			};
		}
	}
}
