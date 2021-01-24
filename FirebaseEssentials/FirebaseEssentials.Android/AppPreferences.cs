using System;
using System.Collections.Generic;
using Android.Content;
using Newtonsoft.Json;
using Xamarin.Essentials;

namespace FirebaseEssentials.Droid
{
	public class AppPreferences
	{
		private Context mContext;

		private static string NotificationKey = "NotificationKey";

		public AppPreferences(Context context)
		{
			mContext = context;
		}

		public void SaveNotification(List<NotificationModel> notifications)
		{
			Preferences.Set(NotificationKey, JsonConvert.SerializeObject(notifications));
		}

		public string GetNotifications()
		{
			return Preferences.Get(NotificationKey, string.Empty);
		}
	}

	public class NotificationModel
	{
		public int NotifiyId { get; set; }
		public string Title { get; set; }
		public string Message { get; set; }
	}
}
