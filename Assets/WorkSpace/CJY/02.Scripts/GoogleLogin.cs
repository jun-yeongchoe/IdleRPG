using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Google;
using Firebase.Auth;
using Firebase;
using TMPro;
using System.Collections;
using UnityEngine.Networking;

public class GoogleLogin : MonoBehaviour
{
    [Header("Google Settings")]
    private string webClientId = "817724772015-cpr9qe1clkj5d1b0cj7hie4qaa2lqe9i.apps.googleusercontent.com"; //

    private FirebaseAuth auth;
    private FirebaseUser user;

    [Header("Login UI")]
    [SerializeField] TextMeshProUGUI loginTxt;
    [SerializeField] TextMeshProUGUI userIdTxt;
    [SerializeField] TextMeshProUGUI userEmailTxt;
    [SerializeField] Image profileImg;

    // 메인 스레드에서 UI를 업데이트하기 위한 플래그
    private bool isLoginTaskComplete = false;

    void Start()
    {
        // Firebase 의존성 및 초기화
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
        var dependencyStatus = task.Result;
        if (dependencyStatus == DependencyStatus.Available) {
            auth = FirebaseAuth.DefaultInstance;

            // 2. 이미 Firebase에 로그인된 유저가 있는지 확인 (자동 로그인 1단계)
            if (auth.CurrentUser != null) {
                user = auth.CurrentUser;
                isLoginTaskComplete = true; // 메인 스레드 UI 업데이트 신호
                Debug.Log("Firebase 세션이 남아있어 자동 로그인 되었습니다: " + user.DisplayName);
            }
            else {
            // 3. 세션이 없다면 구글 Silent 로그인 시도 (자동 로그인 2단계)
            TrySilentGoogleLogin();
            }
        }
    });
    }

    // 구글 계정 정보를 조용히 가져오는 함수
    private void TrySilentGoogleLogin()
    {
        GoogleSignInConfiguration config = new GoogleSignInConfiguration {
            WebClientId = webClientId, RequestIdToken = true, RequestEmail = true
        };
        GoogleSignIn.Configuration = config;

        // SignIn 대신 SignInSilently를 사용합니다.
        GoogleSignIn.DefaultInstance.SignInSilently().ContinueWith(task => {
            if (!task.IsFaulted && !task.IsCanceled) {
                Debug.Log("구글 Silent 로그인 성공! Firebase 인증 시도...");
                SignInWithFirebase(task.Result.IdToken);
            }
        });
    }

    void Update()
    {
        // 로그인 작업이 완료되면 메인 스레드인 Update에서 UI를 갱신
        if (isLoginTaskComplete)
        {
            isLoginTaskComplete = false;
            UpdateUI();
        }
    }

    public void OnClickGoogleLogin()
    {
        GoogleSignInConfiguration config = new GoogleSignInConfiguration {
            WebClientId = webClientId,
            RequestIdToken = true,
            RequestEmail = true
        };
        GoogleSignIn.Configuration = config;

        GoogleSignIn.DefaultInstance.SignIn().ContinueWith(task => {
            if (task.IsFaulted) {
                Debug.LogError("구글 로그인 실패");
            } else if (task.IsCanceled) {
                Debug.Log("구글 로그인 취소");
            } else {
                Debug.Log("구글 로그인 성공! Firebase 인증 시도...");
                SignInWithFirebase(task.Result.IdToken);
            }
        });
    }

    private void SignInWithFirebase(string idToken)
    {
        Credential credential = GoogleAuthProvider.GetCredential(idToken, null);

        auth.SignInAndRetrieveDataWithCredentialAsync(credential).ContinueWith(task => {
            if (task.IsFaulted || task.IsCanceled) {
                Debug.LogError("Firebase 인증 실패");
                return;
            }

            user = task.Result.User;
            isLoginTaskComplete = true; // Update 함수에서 UI를 그리도록 신호를 보냄
        });
    }

    private void UpdateUI()
    {
        if (user == null) return;

        loginTxt.text = "Sign In Status : Login";
        // DisplayName 대신 고유 UID를 사용하려면 user.UserId를 쓰세요.
        userIdTxt.text = "UserId : " + user.DisplayName; 
        userEmailTxt.text = "UserEmail : " + user.Email;

        if (user.PhotoUrl != null)
        {
            StartCoroutine(LoadImage(user.PhotoUrl.ToString()));
        }
    }

    public void OnClickLogout()
    {
        GoogleSignIn.DefaultInstance.SignOut();
        auth.SignOut();
        Debug.Log("로그아웃 되었습니다.");

        loginTxt.text = "Sign In Status : Logout";
        userEmailTxt.text = "UserEmail : ";
        userIdTxt.text = "UserId : ";
        profileImg.sprite = null;
    }

    IEnumerator LoadImage(string imageUri)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(imageUri);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            Texture2D texture = DownloadHandlerTexture.GetContent(www);
            profileImg.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        }
        else
        {
            Debug.Log("이미지 로드 실패");
        }
    }
}