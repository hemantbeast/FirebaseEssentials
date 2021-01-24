using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Firebase.Crashlytics;
using Foundation;

namespace FirebaseEssentials.iOS
{
	public class FirebaseCrashlyticsManager : IFirebaseCrashlytics
	{
		private bool _isUncaughtExceptionHandled;

		public bool DidCrashOnPreviousExecution =>
			Crashlytics.SharedInstance.DidCrashDuringPreviousExecution;

		public void SetCustomKey(string key, bool value)
		{
			Crashlytics.SharedInstance.SetCustomValue(new NSNumber(value), key);
		}

		public void SetCustomKey(string key, int value)
		{
			Crashlytics.SharedInstance.SetCustomValue(new NSNumber(value), key);
		}

		public void SetCustomKey(string key, long value)
		{
			Crashlytics.SharedInstance.SetCustomValue(new NSNumber(value), key);
		}

		public void SetCustomKey(string key, float value)
		{
			Crashlytics.SharedInstance.SetCustomValue(new NSNumber(value), key);
		}

		public void SetCustomKey(string key, double value)
		{
			Crashlytics.SharedInstance.SetCustomValue(new NSNumber(value), key);
		}

		public void SetCustomKey(string key, string value)
		{
			Crashlytics.SharedInstance.SetCustomValue(new NSString(value), key);
		}

		public void SetUserId(string identifier)
		{
			Crashlytics.SharedInstance.SetUserId(identifier);
		}

		public void Log(string message)
		{
			Crashlytics.SharedInstance.Log(message);
		}

		public void LogException(Exception exception)
		{
			var userInfo = new Dictionary<object, object> {
				[NSError.LocalizedDescriptionKey] = exception.Message,
				["StackTrace"] = exception.StackTrace
			};

			var error = new NSError(new NSString(exception.GetType().FullName),
									-1,
									NSDictionary.FromObjectsAndKeys(userInfo.Values.ToArray(), userInfo.Keys.ToArray(), userInfo.Count));

			Crashlytics.SharedInstance.RecordError(error);
		}

		public void SetCrashlyticsCollectionEnabled(bool enabled)
		{
			Crashlytics.SharedInstance.SetCrashlyticsCollectionEnabled(enabled);
		}

		public Task<bool> CheckForUnsentReportsAsync()
		{
			return Crashlytics.SharedInstance.CheckForUnsentReportsAsync();
		}

		public void SendUnsentReports()
		{
			Crashlytics.SharedInstance.SendUnsentReports();
		}

		public void DeleteUnsentReports()
		{
			Crashlytics.SharedInstance.DeleteUnsentReports();
		}

		public void HandleUncaughtException(bool shouldThrowFormattedException = true)
		{
			if (!_isUncaughtExceptionHandled) {
				_isUncaughtExceptionHandled = true;

				AppDomain.CurrentDomain.UnhandledException += (s, e) => {
					if (e.ExceptionObject is Exception exception) {
						LogException(exception);
					}
				};
			}
		}
	}
}
