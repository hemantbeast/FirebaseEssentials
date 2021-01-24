using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace FirebaseEssentials
{
	public interface IFirebaseCrashlytics
	{
		bool DidCrashOnPreviousExecution { get; }

		void SetCustomKey(string key, bool value);

		void SetCustomKey(string key, int value);

		void SetCustomKey(string key, long value);

		void SetCustomKey(string key, float value);

		void SetCustomKey(string key, double value);

		void SetCustomKey(string key, string value);

		void SetUserId(string identifier);

		void LogException(Exception exception);

		void Log(string message);

		void SetCrashlyticsCollectionEnabled(bool enabled);

		Task<bool> CheckForUnsentReportsAsync();

		void SendUnsentReports();

		void DeleteUnsentReports();

		void HandleUncaughtException(bool shouldThrowFormattedException = true);
	}
}
