# Firebase Essentials

Simple cross platform plugin of Firebase basic implementation like Analytics, Crashlytics & Push Notifications for Xamarin Forms project.

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

### API Usage
* In your Android project's Application class or in MainActivity's onCreate() method
``` csharp
	CrossFirebaseEssentials.Notifications = new FirebasePushNotificationManager();
	CrossFirebaseEssentials.Analytics = new FirebaseAnalyticsManager();
	CrossFirebaseEssentials.Crashlytics = new FirebaseCrashlyticsManager();

	FirebaseEssentialsManager.Initialize(this);
```

* In your iOS project's AppDelegate class
``` csharp
	CrossFirebaseEssentials.Notifications = new FirebasePushNotificationManager();
	CrossFirebaseEssentials.Analytics = new FirebaseAnalyticsManager();
	CrossFirebaseEssentials.Crashlytics = new FirebaseCrashlyticsManager();

	FirebaseEssentialsManager.Initialize(this);
```

* Analytics: To track screen name from your forms project
``` csharp
	CrossFirebaseEssentials.Analytics.TrackScreen("screen name");
```

* Crashlytics: To log exception from your forms project
``` csharp
	CrossFirebaseEssentials.Crashlytics.LogException(new Exception("crash"));
```

**Crashlytics**

This is implemented using the guide [here](https://github.com/a-imai/XamarinCrashlyticsUpgradeSample)


**Push Notifications**

This is used from [CrossGreeks/FirebasePushNotificationPlugin](https://github.com/CrossGeeks/FirebasePushNotificationPlugin) with latest firebase packages & fixes.