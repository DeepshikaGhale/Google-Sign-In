using Android.Content;
using Android.Gms.Auth.Api;
using Android.Gms.Auth.Api.SignIn;
using Android.Gms.Common;
using Android.Gms.Common.Apis;
using Android.Gms.Tasks;
using Firebase;
using Firebase.Auth;
using Java.Lang;

namespace GoogleSignIn;

[Activity(Label = "@string/app_name", MainLauncher = true)]
public class MainActivity : Activity, IOnSuccessListener, IOnFailureListener
{
    Button? signInButton;

    GoogleSignInOptions gso;
    GoogleApiClient googleAPiClient;
    FirebaseAuth firebaseAuth;

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        // Set our view from the "main" layout resource
        SetContentView(Resource.Layout.activity_main);

        signInButton = (Button)FindViewById(Resource.Id.signinButton)!;
        signInButton.Click += onClick_SignIn_Btn;

        gso = new GoogleSignInOptions.Builder(GoogleSignInOptions.DefaultSignIn)
            .RequestIdToken(GetString(Resource.String.server_client_id))
            .RequestEmail()
            .Build();

        googleAPiClient = new GoogleApiClient.Builder(this).AddApi(Auth.GOOGLE_SIGN_IN_API, gso).Build();


        googleAPiClient.Connect();
        firebaseAuth = GetFirebaseAuth();

    }

    private FirebaseAuth GetFirebaseAuth()
    {
        var app = FirebaseApp.InitializeApp(this);
        FirebaseAuth mAuth;

        if (app == null)
        {
            var options = new FirebaseOptions.Builder()
                .SetProjectId("qanswer-e66e4")
                .SetApplicationId("qanswer-e66e4")
                .SetApiKey("AIzaSyC-kucS9Jixk0068u1AIJKy221qLhdzoV0")
                .SetStorageBucket("qanswer-e66e4.appspot.com")
                .Build();

            app = FirebaseApp.InitializeApp(this, options);
            mAuth = FirebaseAuth.Instance;


        }
        else {
            mAuth = FirebaseAuth.Instance;
        }

        return mAuth;
    }



    private void onClick_SignIn_Btn(object sender, System.EventArgs e) {
        var intent = Auth.GoogleSignInApi.GetSignInIntent(googleAPiClient);
        StartActivityForResult(intent, 1);
    }

    protected override void OnActivityResult(int requestCode, Result resultCode, Intent? data)
    {
        base.OnActivityResult(requestCode, resultCode, data);
        if (requestCode == 1)
        {
            GoogleSignInResult result = Auth.GoogleSignInApi.GetSignInResultFromIntent(data);
            if (result.IsSuccess)
            {
                GoogleSignInAccount googleSignInAccount = result.SignInAccount;
                LogInWithFirebase(googleSignInAccount);
            }
        }
    }

    private void LogInWithFirebase(GoogleSignInAccount googleSignInAccount) {
        var credentials = GoogleAuthProvider.GetCredential(googleSignInAccount.IdToken, null);
        firebaseAuth.SignInWithCredential(credentials).AddOnSuccessListener(this).AddOnFailureListener(this);
    }

    public void OnSuccess(Java.Lang.Object result)
    {
        signInButton!.Text = $"hello, {firebaseAuth.CurrentUser.DisplayName}";
    }

    public void OnFailure(Java.Lang.Exception e)
    {
       
    }


}
