using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Facebook.CoreKit;
using Facebook.LoginKit;
using Firebase.Auth;
using Firebase.Core;
using Foundation;
using Google.SignIn;
using UIKit;
using GoogleSignIn = Google.SignIn.SignIn;

namespace FirebaseEssentials.iOS
{
	public class FirebaseAuthenticationManager : NSObject, IFirebaseAuth, ISignInDelegate
	{
		AuthType authType;

		// Facebook
		LoginManager loginManager;
		readonly List<string> readPermissions = new List<string> { "public_profile", "email" };

		public void Initialize(AuthType type, string clientId = "")
		{
			authType = type;

			switch (authType) {
				case AuthType.Google:
					GoogleSignIn.SharedInstance.PresentingViewController = GetTopViewController();
					GoogleSignIn.SharedInstance.ClientId = App.DefaultInstance.Options.ClientId;
					GoogleSignIn.SharedInstance.Delegate = this;

					if (GoogleSignIn.SharedInstance.HasPreviousSignIn) {
						GoogleSignIn.SharedInstance.RestorePreviousSignIn();
					}
					break;

				case AuthType.Facebook:
					loginManager = new LoginManager();
					break;
			}
		}

		public static bool OpenUrl(UIApplication app, NSUrl url, NSDictionary options)
		{
			var result = GoogleSignIn.SharedInstance.HandleUrl(url);

			if (result) {
				return result;
			}

			return ApplicationDelegate.SharedInstance.OpenUrl(app, url, options);
		}

		public async Task<FirebaseUser> GetUser()
		{
			var user = Auth.DefaultInstance.CurrentUser;

			if (user != null) {
				var tokenResult = await user.GetIdTokenResultAsync();
				string token = tokenResult?.Token;

				return new FirebaseUser {
					DisplayName = user.DisplayName,
					Email = user.Email,
					Id = user.Uid,
					PhoneNumber = user.PhoneNumber,
					PhotoUrl = user.PhotoUrl?.AbsoluteString,
					Type = authType,
					Token = token
				};
			} else {
				return null;
			}
		}

		#region Phone
		public void StartVerification(string phoneNumber)
		{
			authType = AuthType.Phone;
			PhoneAuthProvider.DefaultInstance.VerifyPhoneNumber(phoneNumber, null, VerifyPhoneNumberOnCompletion);
			SetVerificationStatus(VerificationStatus.Initialized);
		}

		public void ResendVerificationCode(string phoneNumber)
		{
			authType = AuthType.Phone;
			PhoneAuthProvider.DefaultInstance.VerifyPhoneNumber(phoneNumber, null, VerifyPhoneNumberOnCompletion);
			SetVerificationStatus(VerificationStatus.Initialized);
		}

		public void SubmitVerificationCode(string smsCode)
		{
			authType = AuthType.Phone;
			var verificationId = NSUserDefaults.StandardUserDefaults.StringForKey("AuthVerificationID");
			var credential = PhoneAuthProvider.DefaultInstance.GetCredential(verificationId, smsCode);

			Auth.DefaultInstance.SignInWithCredential(credential, SignInOnCompletion);
		}

		void VerifyPhoneNumberOnCompletion(string verificationId, NSError error)
		{
			if (error == null) {
				NSUserDefaults.StandardUserDefaults.SetString(verificationId, "AuthVerificationID");
				SetVerificationStatus(VerificationStatus.CodeSent);
			} else {
				SetVerificationStatus(VerificationStatus.Failed, error.LocalizedDescription);
			}
		}
		#endregion

		public void SignIn(AuthType type)
		{
			authType = type;

			switch (authType) {
				case AuthType.Google:
					GoogleSignIn.SharedInstance.SignInUser();
					SetVerificationStatus(VerificationStatus.Initialized);
					break;

				case AuthType.Facebook:
					if (loginManager != null) {
						loginManager.LogIn(readPermissions.ToArray(), GetTopViewController(), HandleLogin);
						SetVerificationStatus(VerificationStatus.Initialized);
					}
					break;
			}
		}

		public async Task SignOut(AuthType type)
		{
			var signedOut = Auth.DefaultInstance.SignOut(out _);

			switch (type) {
				case AuthType.Google:
					if (signedOut) {
						GoogleSignIn.SharedInstance.SignOutUser();
					}
					break;

				case AuthType.Facebook:
					if (signedOut && loginManager != null) {
						loginManager.LogOut();
					}
					break;
			}
		}

		public async Task Disconnect(AuthType type)
		{
			var signedOut = Auth.DefaultInstance.SignOut(out _);

			switch (type) {
				case AuthType.Google:
					if (signedOut) {
						GoogleSignIn.SharedInstance.DisconnectUser();
					}
					break;

				case AuthType.Facebook:
					if (signedOut && loginManager != null) {
						loginManager.LogOut();
					}
					break;
			}
		}

		#region Google
		public void DidSignIn(GoogleSignIn signIn, GoogleUser user, NSError error)
		{
			if (error == null && user != null) {
				var authentication = user.Authentication;
				var credential = GoogleAuthProvider.GetCredential(authentication.IdToken, authentication.AccessToken);

				Auth.DefaultInstance.SignInWithCredential(credential, SignInOnCompletion);
			} else {
				SetVerificationStatus(VerificationStatus.Failed, error.LocalizedDescription);
			}
		}
		#endregion

		#region Facebook
		private void HandleLogin(LoginManagerLoginResult result, NSError error)
		{
			if (error == null) {
				if (AccessToken.CurrentAccessToken != null) {
					var credential = FacebookAuthProvider.GetCredential(AccessToken.CurrentAccessToken.TokenString);
					Auth.DefaultInstance.SignInWithCredential(credential, SignInOnCompletion);
				}
			} else {
				SetVerificationStatus(VerificationStatus.Failed, error.LocalizedDescription);
			}
		}
		#endregion

		void SignInOnCompletion(AuthDataResult authResult, NSError error)
		{
			if (error == null && authResult != null) {
				if (authType == AuthType.Phone) {
					NSUserDefaults.StandardUserDefaults.RemoveObject("AuthVerificationID");
				}

				SetVerificationStatus(VerificationStatus.Success);
			} else {
				SetVerificationStatus(VerificationStatus.Failed, error.LocalizedDescription);
			}
		}

		private void SetVerificationStatus(VerificationStatus status, string message = "")
		{
			_onVerification?.Invoke(CrossFirebaseEssentials.Authentication, new VerificationEventArgs {
				Message = message,
				Status = status
			});
		}

		private UIViewController GetTopViewController()
		{
			var window = UIApplication.SharedApplication.KeyWindow;
			var vc = window.RootViewController;

			while (vc.PresentedViewController != null) {
				vc = vc.PresentedViewController;
			}

			if (vc is UINavigationController navController) {
				vc = navController.ViewControllers.Last();
			}
			return vc;
		}

		static EventHandler<VerificationEventArgs> _onVerification;
		public event EventHandler<VerificationEventArgs> OnVerification {
			add {
				_onVerification += value;
			}
			remove {
				_onVerification -= value;
			}
		}
	}
}
