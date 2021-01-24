using System.Collections.Generic;
using Android.Content;

namespace FirebaseEssentials.Droid
{
	[BroadcastReceiver]
	public class PushNotificationDeletedReceiver : BroadcastReceiver
	{
		public override void OnReceive(Context context, Intent intent)
		{
			IDictionary<string, object> parameters = new Dictionary<string, object>();
			var extras = intent.Extras;

			AppPreferences appPreferences = new AppPreferences(context);
			appPreferences.SaveNotification(new List<NotificationModel>());

			if (extras != null && !extras.IsEmpty) {
				foreach (var key in extras.KeySet()) {
					parameters.Add(key, $"{extras.Get(key)}");
					System.Diagnostics.Debug.WriteLine(key, $"{extras.Get(key)}");
				}
			}

			FirebasePushNotificationManager.RegisterDelete(parameters);
		}
	}
}