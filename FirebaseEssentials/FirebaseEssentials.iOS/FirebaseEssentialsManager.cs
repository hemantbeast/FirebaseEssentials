using System;
using Firebase.Core;
using Firebase.Crashlytics;
using Foundation;
using UserNotifications;

namespace FirebaseEssentials.iOS
{
	public static class FirebaseEssentialsManager
	{
		public static void Initialize(NSDictionary options)
		{
			if (App.DefaultInstance == null) {
				App.Configure();
			}

			if (CrossFirebaseEssentials.Notifications != null) {
				FirebasePushNotificationManager.Initialize(options, true);
				FirebasePushNotificationManager.CurrentNotificationPresentationOption = UNNotificationPresentationOptions.Alert | UNNotificationPresentationOptions.Sound;
			}

			if (CrossFirebaseEssentials.Crashlytics != null) {
				Crashlytics.SharedInstance.Init();
			}
		}
	}
}
