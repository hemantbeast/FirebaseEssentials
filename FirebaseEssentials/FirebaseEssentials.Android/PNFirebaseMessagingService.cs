using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using Firebase.Messaging;

namespace FirebaseEssentials.Droid
{
	[Service]
	[IntentFilter(new[] { "com.google.firebase.MESSAGING_EVENT" })]
	public class PNFirebaseMessagingService : FirebaseMessagingService
	{
		public override void OnMessageReceived(RemoteMessage message)
		{
			var parameters = new Dictionary<string, object>();
			var notification = message.GetNotification();

			if (notification != null) {
				if (!string.IsNullOrEmpty(notification.Body)) {
					parameters.Add(DefaultPushNotificationHandler.BodyKey, notification.Body);
				}

				if (!string.IsNullOrEmpty(notification.BodyLocalizationKey)) {
					parameters.Add(DefaultPushNotificationHandler.BodyLocKey, notification.BodyLocalizationKey);
				}

				var bodyLocArgs = notification.GetBodyLocalizationArgs();
				if (bodyLocArgs != null && bodyLocArgs.Any()) {
					parameters.Add(DefaultPushNotificationHandler.BodyLocArgsKey, bodyLocArgs);
				}

				if (!string.IsNullOrEmpty(notification.Title)) {
					parameters.Add(DefaultPushNotificationHandler.TitleKey, notification.Title);
				}

				if (!string.IsNullOrEmpty(notification.TitleLocalizationKey)) {
					parameters.Add(DefaultPushNotificationHandler.TitleLocKey, notification.TitleLocalizationKey);
				}

				var titleLocArgs = notification.GetTitleLocalizationArgs();
				if (titleLocArgs != null && titleLocArgs.Any()) {
					parameters.Add(DefaultPushNotificationHandler.TitleLocArgsKey, titleLocArgs);
				}

				if (!string.IsNullOrEmpty(notification.Tag)) {
					parameters.Add(DefaultPushNotificationHandler.TagKey, notification.Tag);
				}

				if (!string.IsNullOrEmpty(notification.Sound)) {
					parameters.Add(DefaultPushNotificationHandler.SoundKey, notification.Sound);
				}

				if (!string.IsNullOrEmpty(notification.Icon)) {
					parameters.Add(DefaultPushNotificationHandler.IconKey, notification.Icon);
				}

				if (notification.Link != null) {
					parameters.Add(DefaultPushNotificationHandler.LinkPathKey, notification.Link.Path);
				}

				if (!string.IsNullOrEmpty(notification.ClickAction)) {
					parameters.Add(DefaultPushNotificationHandler.ActionKey, notification.ClickAction);
				}

				if (!string.IsNullOrEmpty(notification.Color)) {
					parameters.Add(DefaultPushNotificationHandler.ColorKey, notification.Color);
				}

				if (notification.ImageUrl != null && !string.IsNullOrEmpty(notification.ImageUrl.ToString())) {
					parameters.Add(DefaultPushNotificationHandler.ImageKey, notification.ImageUrl.ToString());
				}
			}

			foreach (var d in message.Data) {
				if (!parameters.ContainsKey(d.Key)) {
					parameters.Add(d.Key, d.Value);
				}
			}

			FirebasePushNotificationManager.RegisterData(parameters);
			CrossFirebaseEssentials.Notifications.NotificationHandler?.OnReceived(parameters);
		}

		private void ScheduleJob()
		{
			// [START dispatch_job]
			/*FirebaseJobDispatcher dispatcher = new FirebaseJobDispatcher(new GooglePlayDriver(this));
            Job myJob = dispatcher.newJobBuilder()
                    .setService(MyJobService.class)
                .setTag("my-job-tag")
                .build();
        dispatcher.schedule(myJob);*/
			// [END dispatch_job]
		}
	}
}