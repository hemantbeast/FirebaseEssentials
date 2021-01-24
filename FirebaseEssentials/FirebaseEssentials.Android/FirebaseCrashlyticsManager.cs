using System;
using System.Threading.Tasks;
using Android.Gms.Extensions;
using Android.Runtime;
using Java.Lang;
using Firebase.Crashlytics;

namespace FirebaseEssentials.Droid
{
	public class FirebaseCrashlyticsManager : IFirebaseCrashlytics
	{
		private EventHandler<RaiseThrowableEventArgs> _handler;

		public bool DidCrashOnPreviousExecution =>
			FirebaseCrashlytics.Instance.DidCrashOnPreviousExecution();

		public void SetCustomKey(string key, bool value)
		{
			FirebaseCrashlytics.Instance.SetCustomKey(key, value);
		}

		public void SetCustomKey(string key, int value)
		{
			FirebaseCrashlytics.Instance.SetCustomKey(key, value);
		}

		public void SetCustomKey(string key, long value)
		{
			FirebaseCrashlytics.Instance.SetCustomKey(key, value);
		}

		public void SetCustomKey(string key, float value)
		{
			FirebaseCrashlytics.Instance.SetCustomKey(key, value);
		}

		public void SetCustomKey(string key, double value)
		{
			FirebaseCrashlytics.Instance.SetCustomKey(key, value);
		}

		public void SetCustomKey(string key, string value)
		{
			FirebaseCrashlytics.Instance.SetCustomKey(key, value);
		}

		public void SetUserId(string identifier)
		{
			FirebaseCrashlytics.Instance.SetUserId(identifier);
		}

		public void SetCrashlyticsCollectionEnabled(bool enabled)
		{
			FirebaseCrashlytics.Instance.SetCrashlyticsCollectionEnabled(enabled);
		}

		public async Task<bool> CheckForUnsentReportsAsync()
		{
			var result = await FirebaseCrashlytics.Instance.CheckForUnsentReports()
				.AsAsync<Java.Lang.Boolean>()
				.ConfigureAwait(false);

			return (bool)result;
		}

		public void SendUnsentReports()
		{
			FirebaseCrashlytics.Instance.SendUnsentReports();
		}

		public void DeleteUnsentReports()
		{
			FirebaseCrashlytics.Instance.DeleteUnsentReports();
		}

		public void HandleUncaughtException(bool shouldThrowFormattedException = true)
		{
			if (_handler != null) {
				AndroidEnvironment.UnhandledExceptionRaiser -= _handler;
			}

			_handler = (s, e) => {
				if (shouldThrowFormattedException && Thread.DefaultUncaughtExceptionHandler != null) {
					Thread.DefaultUncaughtExceptionHandler.UncaughtException(Thread.CurrentThread(), Throwable.FromException(e.Exception));
				} else {
					LogException(e.Exception);
				}
			};

			AndroidEnvironment.UnhandledExceptionRaiser += _handler;
		}

		public void LogException(System.Exception exception)
		{
			FirebaseCrashlytics.Instance.RecordException(Throwable.FromException(exception));
		}

		public void Log(string message)
		{
			FirebaseCrashlytics.Instance.Log(message);
		}
	}
}
