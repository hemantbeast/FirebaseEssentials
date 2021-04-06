# Firebase Essentials

Simple cross platform plugin of Firebase basic implementation like Analytics, Authentication, Crashlytics & Push Notifications for Xamarin Forms project.

Android & iOS platform support for now.

### Setup
* Add firebase `google-service.json` & `GoogleService-Info.plist` in your respective project
* Add FirebaseEssentials solution folder in your project
* Add latest package of Xamarin.GooglePlayServices.Basement in your Android project
* Make sure the build action of `google-service.json` is `GoogleServicesJson` & for `GoogleService-Info.plist` is `BundleResource`
* Add this below line in your Android project's `strings.xml` file
```xml
<?xml version="1.0" encoding="utf-8"?>
<resources>
    <string name="com.google.firebase.crashlytics.mapping_file_id">1.0</string>
</resources>
```

### Authentication
* Follow firebase authentication quickstart for implementing google & facebook authentication
* Setup google & facebook info in your app's manifest & plist

### API Usage
* In your Android project's Application class or in MainActivity's onCreate() method
``` csharp
CrossFirebaseEssentials.Notifications = new FirebasePushNotificationManager();
CrossFirebaseEssentials.Analytics = new FirebaseAnalyticsManager();
CrossFirebaseEssentials.Crashlytics = new FirebaseCrashlyticsManager();
CrossFirebaseEssentials.Authentication = new FirebaseAuthenticationManager();

FirebaseEssentialsManager.Initialize(this);
```

* In your iOS project's AppDelegate class
``` csharp
CrossFirebaseEssentials.Notifications = new FirebasePushNotificationManager();
CrossFirebaseEssentials.Analytics = new FirebaseAnalyticsManager();
CrossFirebaseEssentials.Crashlytics = new FirebaseCrashlyticsManager();
CrossFirebaseEssentials.Authentication = new FirebaseAuthenticationManager();

FirebaseEssentialsManager.Initialize(this);
```
***
**Notifications**
***
* Android: In MainActivity.cs
``` csharp
protected override void OnCreate(Bundle savedInstanceState)
{
	base.OnCreate(savedInstanceState);

	...

	LoadApplication(new App());
	FirebasePushNotificationManager.ProcessIntent(this, Intent);
}

protected override void OnNewIntent(Intent intent)
{
	base.OnNewIntent(intent);
	FirebasePushNotificationManager.ProcessIntent(this, intent);
}
```

* iOS: In AppDelegate.cs
``` csharp
public override void RegisteredForRemoteNotifications(UIApplication application, NSData deviceToken)
{
	FirebasePushNotificationManager.DidRegisterRemoteNotifications(deviceToken);
}

public override void FailedToRegisterForRemoteNotifications(UIApplication application, NSError error)
{
	FirebasePushNotificationManager.RemoteNotificationRegistrationFailed(error);
}

public override void DidReceiveRemoteNotification(UIApplication application, NSDictionary userInfo, Action<UIBackgroundFetchResult> completionHandler)
{
	FirebasePushNotificationManager.DidReceiveMessage(userInfo);
	Console.WriteLine(userInfo);
	completionHandler(UIBackgroundFetchResult.NewData);
}
```

<details>
	<summary>Usage</summary>
	<p>This is used from <a href="https://github.com/CrossGeeks/FirebasePushNotificationPlugin">CrossGreeks/FirebasePushNotificationPlugin</a> with latest firebase packages & fixes.</p>
</details>

***
**Analytics**
***
* To track screen name from your forms project
``` csharp
CrossFirebaseEssentials.Analytics.TrackScreen("screen name");
```

***
**Authentication**
***
* Android: In MainActivity.cs
``` csharp
protected override async void OnActivityResult(int requestCode, Result resultCode, Intent data)
{
	base.OnActivityResult(requestCode, resultCode, data);
	await FirebaseAuthenticationManager.OnActivityResult(requestCode, resultCode, data);
}
```

* iOS: In AppDelegate.cs
``` csharp
public override bool OpenUrl(UIApplication app, NSUrl url, NSDictionary options)
{
	return FirebaseAuthenticationManager.OpenUrl(app, url, options);
}
```

* Initialize google or facebook login in your page's OnAppearing()
``` csharp
CrossFirebaseEssentials.Authentication.Initialize(AuthType.Google, googleClientId);
CrossFirebaseEssentials.Authentication.Initialize(AuthType.Phone)
```

* Then for login/logout, call below method
``` csharp
CrossFirebaseEssentials.Authentication.SignIn(AuthType.Google);
CrossFirebaseEssentials.Authentication.SignOut(AuthType.Google);
```

* To get the status for login, call below action
``` csharp
CrossFirebaseEssentials.Authentication.OnVerification += (sender, args) => {
	switch (args.Status) {
		case VerificationStatus.Initialized:
		 break;
		case VerificationStatus.Success:
		 break;
		case VerificationStatus.CodeSent:
		 break;
		case VerificationStatus.Failed:
		 break;
	}
};
```

* For phone verification, first call below method after getting phone number
``` csharp
CrossFirebaseEssentials.Authentication.StartVerification(phoneNumber);
```
* Then we need to open the sms verification page on `CodeSent` status. And after getting sms code
``` csharp
CrossFirebaseEssentials.Authentication.SubmitVerificationCode(smsCode);
```
* In case we need to resend the verification code again
``` csharp
CrossFirebaseEssentials.Authentication.ResendVerificationCode(phoneNumber);
```

* At last, get user info with access token
``` csharp
var user = await CrossFirebaseEssentials.Authentication.GetUser();
```

***
**Crashlytics**
***
* To log exception from your forms project
``` csharp
CrossFirebaseEssentials.Crashlytics.LogException(new Exception("crash"));
```

<details>
	<summary>Usage</summary>
	<p>This is implemented using the guide <a href="https://github.com/a-imai/XamarinCrashlyticsUpgradeSample">here</a></p>
</details>