using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Gms.Auth.Api.SignIn;
using Android.Gms.Extensions;
using Firebase;
using Firebase.Auth;
using Java.Lang;
using Java.Util.Concurrent;
using Xamarin.Facebook;
using Xamarin.Facebook.Login;

namespace FirebaseEssentials.Droid
{
	public class FirebaseAuthenticationManager : Java.Lang.Object, IFirebaseAuth, IFacebookCallback
	{
		static int RC_SIGN_IN = 9001;
		static AuthType authType;
		static string googleToken, facebookToken;

		// Google
		GoogleSignInClient signInClient;

		// Facebook
		static ICallbackManager callbackManager;
		readonly List<string> permissions = new List<string> { "public_profile", "email" };

		// Phone
		readonly PhoneAuthCallback phoneCallback;
		public string VerificationId { get; set; }
		public PhoneAuthProvider.ForceResendingToken ResendToken { get; set; }

		public FirebaseAuthenticationManager()
		{
			phoneCallback = new PhoneAuthCallback(this);
		}

		public void Initialize(AuthType type, string clientId = "")
		{
			authType = type;

			switch (authType) {
				case AuthType.Google:
					var gso = new GoogleSignInOptions.Builder(GoogleSignInOptions.DefaultSignIn)
						.RequestIdToken(clientId)
						.RequestProfile()
						.RequestEmail()
						.Build();

					signInClient = GoogleSignIn.GetClient(Xamarin.Essentials.Platform.CurrentActivity, gso);
					break;

				case AuthType.Facebook:
					callbackManager = CallbackManagerFactory.Create();
					LoginManager.Instance.RegisterCallback(callbackManager, this);
					break;
			}
		}

		public static async Task OnActivityResult(int requestCode, Result resultCode, Intent data)
		{
			if (requestCode == RC_SIGN_IN) {
				var result = await GoogleSignIn.GetSignedInAccountFromIntent(data).AsAsync<GoogleSignInAccount>();

				if (result != null) {
					googleToken = result.IdToken;

					var credential = GoogleAuthProvider.GetCredential(result.IdToken, null);
					var firebaseResult = await FirebaseAuth.Instance.SignInWithCredentialAsync(credential);

					if (firebaseResult != null) {
						(CrossFirebaseEssentials.Authentication as FirebaseAuthenticationManager).SetVerificationStatus(VerificationStatus.Success);
					} else {
						(CrossFirebaseEssentials.Authentication as FirebaseAuthenticationManager).SetVerificationStatus(VerificationStatus.Failed);
					}
				} else {
					googleToken = string.Empty;
					(CrossFirebaseEssentials.Authentication as FirebaseAuthenticationManager).SetVerificationStatus(VerificationStatus.Failed);
				}
			} else {
				callbackManager?.OnActivityResult(requestCode, (int)resultCode, data);
			}
		}

		public async Task<FirebaseUser> GetUser(bool isFirebaseToken)
		{
			var user = FirebaseAuth.Instance.CurrentUser;

			if (user != null) {
				var token = string.Empty;
				var tokenResult = await user.GetIdToken(true).AsAsync<GetTokenResult>();

				if (tokenResult != null) {
					token = tokenResult.Token;
				}

				var firebaseUser = new FirebaseUser {
					DisplayName = user.DisplayName,
					Email = user.Email,
					Id = user.Uid,
					PhotoUrl = user.PhotoUrl?.ToString(),
					PhoneNumber = user.PhoneNumber,
					Type = authType
				};

				switch (authType) {
					case AuthType.Google:
						firebaseUser.Token = isFirebaseToken ? token : googleToken;
						break;
					case AuthType.Facebook:
						firebaseUser.Token = isFirebaseToken ? token : facebookToken;
						break;
					default:
						firebaseUser.Token = token;
						break;
				}
				return firebaseUser;
			} else {
				return null;
			}
		}

		#region Phone
		public void StartVerification(string phoneNumber)
		{
			authType = AuthType.Phone;

			var options = new PhoneAuthOptions
				.Builder(FirebaseAuth.Instance)
				.SetPhoneNumber(phoneNumber)
				.SetTimeout(Long.ValueOf(60), TimeUnit.Seconds)
				.SetCallbacks(phoneCallback)
				.SetActivity(Xamarin.Essentials.Platform.CurrentActivity)
				.Build();

			PhoneAuthProvider.VerifyPhoneNumber(options);
			SetVerificationStatus(VerificationStatus.Initialized);
		}

		public void ResendVerificationCode(string phoneNumber)
		{
			authType = AuthType.Phone;

			var options = new PhoneAuthOptions
				.Builder(FirebaseAuth.Instance)
				.SetPhoneNumber(phoneNumber)
				.SetTimeout(Long.ValueOf(60), TimeUnit.Seconds)
				.SetCallbacks(phoneCallback)
				.SetActivity(Xamarin.Essentials.Platform.CurrentActivity);

			if (ResendToken != null) {
				options.SetForceResendingToken(ResendToken);
			}

			PhoneAuthProvider.VerifyPhoneNumber(options.Build());
			SetVerificationStatus(VerificationStatus.Initialized);
		}

		public void SubmitVerificationCode(string smsCode)
		{
			authType = AuthType.Phone;

			var credential = PhoneAuthProvider.GetCredential(VerificationId, smsCode);
			SignInWithCredential(credential);
		}

		public async void SignInWithCredential(PhoneAuthCredential credential)
		{
			var firebaseResult = await FirebaseAuth.Instance.SignInWithCredentialAsync(credential);

			if (firebaseResult != null) {
				SetVerificationStatus(VerificationStatus.Success);
			} else {
				SetVerificationStatus(VerificationStatus.Failed);
			}
		}

		public void VerificationFailed(FirebaseException exception)
		{
			string message;

			if (exception is FirebaseAuthInvalidCredentialsException) {
				message = "Invalid phone number";
			} else if (exception is FirebaseTooManyRequestsException) {
				message = "Quota exceeded";
			} else {
				message = exception.Message;
			}
			SetVerificationStatus(VerificationStatus.Failed, message);
		}
		#endregion

		public void SignIn(AuthType type)
		{
			authType = type;

			switch (authType) {
				case AuthType.Google:
					if (signInClient != null) {
						Xamarin.Essentials.Platform.CurrentActivity.StartActivityForResult(signInClient.SignInIntent, RC_SIGN_IN);
						SetVerificationStatus(VerificationStatus.Initialized);
					}
					break;

				case AuthType.Facebook:
					LoginManager.Instance.LogInWithReadPermissions(Xamarin.Essentials.Platform.CurrentActivity, permissions);
					SetVerificationStatus(VerificationStatus.Initialized);
					break;
			}
		}

		public async Task SignOut(AuthType type)
		{
			FirebaseAuth.Instance.SignOut();

			switch (type) {
				case AuthType.Facebook:
					facebookToken = string.Empty;
					LoginManager.Instance.LogOut();
					break;

				case AuthType.Google:
					googleToken = string.Empty;
					if (signInClient != null) {
						await signInClient.SignOutAsync();
					}
					break;
			}
		}

		public async Task Disconnect(AuthType type)
		{
			FirebaseAuth.Instance.SignOut();

			switch (type) {
				case AuthType.Facebook:
					facebookToken = string.Empty;
					LoginManager.Instance.LogOut();
					break;

				case AuthType.Google:
					googleToken = string.Empty;
					if (signInClient != null) {
						await signInClient.RevokeAccessAsync();
					}
					break;
			}
		}

		#region Facebook
		public void OnCancel()
		{
			_ = SignOut(AuthType.Facebook);
			SetVerificationStatus(VerificationStatus.Failed);
		}

		public void OnError(FacebookException error)
		{
			var message = string.Empty;

			if (error != null) {
				Class cls = error.Class;

				if (cls.SimpleName.Equals("FacebookAuthorizationException")) {
					_ = SignOut(AuthType.Facebook);
				} else {
					message = error.Message;
				}
			}
			SetVerificationStatus(VerificationStatus.Failed, message);
		}

		public async void OnSuccess(Java.Lang.Object result)
		{
			if (AccessToken.CurrentAccessToken != null) {
				facebookToken = AccessToken.CurrentAccessToken.Token;
				var credential = FacebookAuthProvider.GetCredential(AccessToken.CurrentAccessToken.Token);
				var firebaseResult = await FirebaseAuth.Instance.SignInWithCredentialAsync(credential);

				if (firebaseResult != null) {
					SetVerificationStatus(VerificationStatus.Success);
				} else {
					SetVerificationStatus(VerificationStatus.Failed);
				}
			} else {
				facebookToken = string.Empty;
				SetVerificationStatus(VerificationStatus.Failed);
			}
		}
		#endregion

		public void SetVerificationStatus(VerificationStatus status, string message = "")
		{
			_onVerification?.Invoke(CrossFirebaseEssentials.Authentication, new VerificationEventArgs {
				Message = message,
				Status = status
			});
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

	public class PhoneAuthCallback : PhoneAuthProvider.OnVerificationStateChangedCallbacks
	{
		readonly FirebaseAuthenticationManager _authManager;

		public PhoneAuthCallback(FirebaseAuthenticationManager authManager)
		{
			_authManager = authManager;
		}

		public override void OnVerificationCompleted(PhoneAuthCredential credential)
		{
			_authManager.SignInWithCredential(credential);
		}

		public override void OnVerificationFailed(FirebaseException exception)
		{
			_authManager.VerificationFailed(exception);
		}

		public override void OnCodeSent(string verificationId, PhoneAuthProvider.ForceResendingToken token)
		{
			base.OnCodeSent(verificationId, token);

			_authManager.VerificationId = verificationId;
			_authManager.ResendToken = token;
			_authManager.SetVerificationStatus(VerificationStatus.CodeSent);
		}
	}
}
