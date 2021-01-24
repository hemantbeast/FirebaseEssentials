using System;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using FirebaseEssentials.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

namespace FirebaseSample.Droid
{
	[Activity(MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	public class MainActivity : FormsAppCompatActivity
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			TabLayoutResource = Resource.Layout.Tabbar;
			ToolbarResource = Resource.Layout.Toolbar;

			base.OnCreate(savedInstanceState);
			Forms.Init(this, savedInstanceState);

			LoadApplication(new App());
			FirebasePushNotificationManager.ProcessIntent(this, Intent);
		}

		protected override void OnNewIntent(Intent intent)
		{
			base.OnNewIntent(intent);
			FirebasePushNotificationManager.ProcessIntent(this, intent);
		}
	}
}