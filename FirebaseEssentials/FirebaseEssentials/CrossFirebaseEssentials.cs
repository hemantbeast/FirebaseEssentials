using System;

namespace FirebaseEssentials
{
	public static class CrossFirebaseEssentials
	{
		public static IFirebasePushNotification Notifications { get; set; }

		public static IFirebaseAnalytics Analytics { get; set; }

		public static IFirebaseCrashlytics Crashlytics { get; set; }

		public static IFirebaseAuth Authentication { get; set; }
	}
}
