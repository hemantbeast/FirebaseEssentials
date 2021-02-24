using System;
using FirebaseEssentials;
using Xamarin.Forms;

namespace FirebaseSample
{
	public partial class MainPage : ContentPage
	{
		public const string GoogleClientId = "984597696086-umu1ka26tdfdbs71s0l9p2cvjaaoooaf.apps.googleusercontent.com";

		public MainPage()
		{
			InitializeComponent();
			Title = "Firebase Sample";
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();
			CrossFirebaseEssentials.Authentication.Initialize(AuthType.Google, GoogleClientId);
			CrossFirebaseEssentials.Authentication.Initialize(AuthType.Facebook);

			CrossFirebaseEssentials.Authentication.OnVerification += LoginVerification;
		}

		protected override void OnDisappearing()
		{
			base.OnDisappearing();
			CrossFirebaseEssentials.Authentication.OnVerification -= LoginVerification;
		}

		async void LoginVerification(object sender, VerificationEventArgs e)
		{
			try {
				switch (e.Status) {
					case VerificationStatus.Success:
						await Navigation.PushAsync(new UserInfoPage());
						break;

					case VerificationStatus.CodeSent:
						indicator.IsRunning = false;
						indicator.IsVisible = false;
						await Navigation.PushAsync(new SmsVerificationPage(phoneEntry.Text));
						break;

					case VerificationStatus.Failed:
						if (!string.IsNullOrEmpty(e.Message)) {
							await Application.Current.MainPage.DisplayAlert("Alert", e.Message, "OK");
						} else {
							await Application.Current.MainPage.DisplayAlert("Alert", "Login failed!", "OK");
						}
						break;
				}
			} catch (Exception ex) {
				Console.WriteLine(ex.Message);
			}
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

		void PhoneLoginClicked(object sender, EventArgs e)
		{
			if (string.IsNullOrEmpty(phoneEntry.Text) || !phoneEntry.Text.StartsWith("+") || phoneEntry.Text.Length < 10) {
				Application.Current.MainPage.DisplayAlert("Alert", "Please enter phone number with country code", "OK");
				return;
			}

			indicator.IsRunning = true;
			indicator.IsVisible = true;
			CrossFirebaseEssentials.Authentication.StartVerification(phoneEntry.Text);
		}

		void GoogleLoginClicked(object sender, EventArgs e)
		{
			CrossFirebaseEssentials.Authentication.SignIn(AuthType.Google);
		}

		void FacebookLoginClicked(object sender, EventArgs e)
		{
			CrossFirebaseEssentials.Authentication.SignIn(AuthType.Facebook);
		}
	}
}
