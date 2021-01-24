using System;
using System.Collections.Generic;
using Android.OS;
using Firebase.Analytics;

namespace FirebaseEssentials.Droid
{
	public class FirebaseAnalyticsManager : IFirebaseAnalytics
	{
		public void LogEvent(string eventId)
		{
			LogEvent(eventId, null);
		}

		public void LogEvent(string eventId, string paramName, string value)
		{
			LogEvent(eventId, new Dictionary<string, string> {
				{ paramName, value }
			});
		}

		public void LogEvent(string eventId, IDictionary<string, string> parameters)
		{
			var fireBaseAnalytics = FirebaseAnalytics.GetInstance(Xamarin.Essentials.Platform.CurrentActivity);

			if (fireBaseAnalytics == null) {
				return;
			}

			if (parameters == null) {
				fireBaseAnalytics.LogEvent(eventId, null);
				return;
			}

			var bundle = new Bundle();

			foreach (var item in parameters) {
				bundle.PutString(item.Key, item.Value);
			}

			fireBaseAnalytics.LogEvent(eventId, bundle);
		}

		public void SetUserId(string userId)
		{
			var fireBaseAnalytics = FirebaseAnalytics.GetInstance(Xamarin.Essentials.Platform.CurrentActivity);
			fireBaseAnalytics?.SetUserId(userId);
		}

		public void TrackScreen(string screenName)
		{
			Bundle bundle = new Bundle();
			bundle.PutString(FirebaseAnalytics.Param.ScreenName, screenName);
			bundle.PutString(FirebaseAnalytics.Param.ScreenClass, Xamarin.Essentials.Platform.CurrentActivity.Title);

			var fireBaseAnalytics = FirebaseAnalytics.GetInstance(Xamarin.Essentials.Platform.CurrentActivity);
			fireBaseAnalytics?.LogEvent(FirebaseAnalytics.Event.ScreenView, bundle);
		}
	}
}
