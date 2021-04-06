using System;
using System.Threading.Tasks;

namespace FirebaseEssentials
{
	public interface IFirebaseAuth
	{
		void Initialize(AuthType type, string clientId = "");

		void SignIn(AuthType type);

		void StartVerification(string phoneNumber);

		void ResendVerificationCode(string phoneNumber);

		void SubmitVerificationCode(string smsCode);

		Task SignOut(AuthType type);

		Task Disconnect(AuthType type);

		Task<FirebaseUser> GetUser(bool isFirebaseToken = false);

		event EventHandler<VerificationEventArgs> OnVerification;
	}

	public class VerificationEventArgs : EventArgs
	{
		public string Message { get; set; }
		public VerificationStatus Status { get; set; }
	}

	public class FirebaseUser
	{
		public string Id { get; set; }
		public string DisplayName { get; set; }
		public string PhoneNumber { get; set; }
		public string Email { get; set; }
		public string PhotoUrl { get; set; }
		public AuthType Type { get; set; }
		public string Token { get; set; }
	}

	public enum AuthType
	{
		Phone,
		Facebook,
		Google,
		Apple
	}

	public enum VerificationStatus
	{
		Initialized,
		CodeSent,
		Failed,
		Success,
	}
}
