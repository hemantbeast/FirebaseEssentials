using System;
using System.Linq;
using FirebaseEssentials;
using Xamarin.Forms;

namespace FirebaseSample
{
	public partial class SmsVerificationPage : ContentPage
	{
		string smsCode, _phoneNumber;

		public SmsVerificationPage(string phoneNumber)
		{
			InitializeComponent();
			_phoneNumber = phoneNumber;
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();
			CrossFirebaseEssentials.Authentication.OnVerification += PhoneNumberVerification;
		}

		protected override void OnDisappearing()
		{
			base.OnDisappearing();
			CrossFirebaseEssentials.Authentication.OnVerification -= PhoneNumberVerification;
		}

		private void PhoneNumberVerification(object sender, VerificationEventArgs e)
		{
			try {
				switch (e.Status) {
					case VerificationStatus.Success:
						indicator.IsRunning = false;
						indicator.IsVisible = false;
						Navigation.PushAsync(new UserInfoPage());

						var navigationList = Navigation.NavigationStack.ToList();
						var smsPage = navigationList.FirstOrDefault(x => x.GetType() == typeof(SmsVerificationPage));
						Navigation.RemovePage(smsPage);
						break;

					case VerificationStatus.Failed:
						if (!string.IsNullOrEmpty(e.Message)) {
							Application.Current.MainPage.DisplayAlert("Alert", e.Message, "OK");
						} else {
							Application.Current.MainPage.DisplayAlert("Alert", "Verification failed!", "OK");
						}
						break;
				}
			} catch (Exception ex) {
				Console.WriteLine(ex.Message);
			}
		}

		void EntryTextChanged(object sender, TextChangedEventArgs e)
		{
			try {
				var entry = sender as Entry;

				var nextClassId = Convert.ToInt32(entry.ClassId) + 1;
				var nextEntry = grdCode.Children.Where(x => x is Entry).FirstOrDefault(x => x.ClassId == nextClassId.ToString());

				var previousClassId = Convert.ToInt32(entry.ClassId) - 1;
				var previousEntry = grdCode.Children.Where(x => x is Entry).FirstOrDefault(x => x.ClassId == previousClassId.ToString());

				if (string.IsNullOrEmpty(e.NewTextValue)) {
					if (previousEntry != null) {
						SetVerificationCode();
					}
				} else {
					if (entry.ReturnType == ReturnType.Done) {
						SetVerificationCode();
						entry.Unfocus();
					} else if (nextEntry != null) {
						nextEntry.Focus();
						SetVerificationCode();
					}
				}
			} catch (Exception ex) {
				Console.WriteLine(ex.Message);
			}
		}

		private void SetVerificationCode()
		{
			var code = string.Empty;
			var entries = grdCode.Children.Where(x => x is Entry).ToList();

			foreach (var item in entries) {
				code += (item as Entry).Text;
			}
			smsCode = code;
		}

		void NextClicked(object sender, EventArgs e)
		{
			if (string.IsNullOrEmpty(smsCode) || smsCode.Length < 6) {
				Application.Current.MainPage.DisplayAlert("Alert", "Please fill verification code", "OK");
				return;
			}

			indicator.IsRunning = true;
			indicator.IsVisible = true;
			CrossFirebaseEssentials.Authentication.SubmitVerificationCode(smsCode);
		}

		void ResendClicked(object sender, EventArgs e)
		{
			CrossFirebaseEssentials.Authentication.ResendVerificationCode(_phoneNumber);
		}
	}
}
