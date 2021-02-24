using System;
using FirebaseEssentials;
using Xamarin.Forms;

namespace FirebaseSample
{
	public partial class UserInfoPage : ContentPage
	{
		FirebaseUser user;

		public UserInfoPage()
		{
			InitializeComponent();
		}

		protected override async void OnAppearing()
		{
			base.OnAppearing();

			user = await CrossFirebaseEssentials.Authentication.GetUser();
			indicator.IsRunning = false;

			if (!string.IsNullOrEmpty(user.PhotoUrl)) {
				image.Source = new UriImageSource { Uri = new Uri(user.PhotoUrl) };
			} else {
				imgFrame.IsVisible = false;
			}

			lblName.Text = user.DisplayName;
			nameView.IsVisible = !string.IsNullOrEmpty(user.DisplayName);

			lblEmail.Text = user.Email;
			emailView.IsVisible = !string.IsNullOrEmpty(user.Email);

			lblPhone.Text = user.PhoneNumber;
			phoneView.IsVisible = !string.IsNullOrEmpty(user.PhoneNumber);
		}

		void LogoutClicked(object sender, EventArgs e)
		{
			CrossFirebaseEssentials.Authentication.SignOut(user.Type);
			Navigation.PopAsync();
		}
	}
}
