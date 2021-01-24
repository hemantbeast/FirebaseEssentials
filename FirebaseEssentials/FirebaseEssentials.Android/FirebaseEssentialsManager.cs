using System;
using Android.Content;
using Firebase;

namespace FirebaseEssentials.Droid
{
	public static class FirebaseEssentialsManager
	{
		public static void Initialize(Context context)
		{
			FirebaseApp.InitializeApp(context);

			if (CrossFirebaseEssentials.Notifications != null) {
				//If debug you should reset the token each time.
#if DEBUG
				FirebasePushNotificationManager.Initialize(context, true);
#else
				FirebasePushNotificationManager.Initialize(context, false);
#endif
			}
		}
	}
}
