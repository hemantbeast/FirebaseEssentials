using System;
using FirebaseEssentials;
using Xamarin.Forms;

namespace FirebaseSample
{
	public partial class MainPage : ContentPage
	{
		public MainPage()
		{
			InitializeComponent();
			Title = "Firebase Sample";
		}

		void Handle_Clicked_LogEvent(object sender, EventArgs e)
		{
			if (!string.IsNullOrEmpty(entryEvent.Text)) {
				var device = Device.RuntimePlatform == Device.Android ? "Android" : "iOS";
				CrossFirebaseEssentials.Analytics.LogEvent("LogEvent", device, entryEvent.Text);
			}
		}

		void Handle_Clicked_Crash(object sender, EventArgs e)
		{
			CrossFirebaseEssentials.Crashlytics.Log("crash");
			CrossFirebaseEssentials.Crashlytics.LogException(new Exception("crash"));
		}

		void Handle_Clicked_NonFatalCrash(object sender, EventArgs e)
		{
			CrossFirebaseEssentials.Crashlytics.Log("Not Fatal Log");
		}

		void Handle_Clicked_CustomLog(object sender, EventArgs e)
		{
			CrossFirebaseEssentials.Crashlytics.Log("custom log");
			CrossFirebaseEssentials.Crashlytics.LogException(new Exception("custom log"));
		}
	}
}
