using System;
using System.Diagnostics;
using FirebaseEssentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace FirebaseSample
{
	public partial class App : Application
	{
		public App()
		{
			InitializeComponent();
			CrossFirebaseEssentials.Crashlytics.HandleUncaughtException();

			MainPage = new NavigationPage(new MainPage());
		}

		protected override void OnStart()
		{
			// Handle when your app starts
			CrossFirebaseEssentials.Notifications.Subscribe("general");
			Debug.WriteLine($"TOKEN: {CrossFirebaseEssentials.Notifications.Token}");

			CrossFirebaseEssentials.Notifications.OnTokenRefresh += (s, p) => {
				Debug.WriteLine($"TOKEN REC: {p.Token}");
			};

			CrossFirebaseEssentials.Notifications.OnNotificationReceived += (s, p) => {
				try {
					Debug.WriteLine("Received");

					if (p.Data.ContainsKey("body")) {
					}
				} catch (Exception ex) {
					Debug.WriteLine(ex.Message);
				}
			};

			CrossFirebaseEssentials.Notifications.OnNotificationOpened += (s, p) => {
				//System.Diagnostics.Debug.WriteLine(p.Identifier);
				Debug.WriteLine("Opened");
				foreach (var data in p.Data) {
					Debug.WriteLine($"{data.Key} : {data.Value}");
				}
			};

			CrossFirebaseEssentials.Notifications.OnNotificationDeleted += (s, p) => {
				Debug.WriteLine("Dismissed");
			};
		}

		protected override void OnSleep()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume()
		{
			// Handle when your app resumes
		}
	}
}
