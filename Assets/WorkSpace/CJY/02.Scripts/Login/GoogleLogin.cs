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
    private string gameScene = "Game Scene_1st";
    private FirebaseAuth auth;
    private FirebaseUser user;

    // 메인 스레드에서 UI를 업데이트하기 위한 플래그
    private bool isLoginTaskComplete = false;

    // 로그아웃을 통해 돌아온 경우 자동 로그인을 방지하기 위한 변수
    public static bool isLogoutCalled = false;

    void Start()
    {
        GoogleSignInConfiguration config = new GoogleSignInConfiguration {
            WebClientId = webClientId,
            RequestIdToken = true,
            RequestEmail = true
        };
        
        GoogleSignIn.Configuration = config;

        // Firebase 의존성 및 초기화
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
        var dependencyStatus = task.Result;
        if (dependencyStatus == DependencyStatus.Available) {
            auth = FirebaseAuth.DefaultInstance;

            if (!isLogoutCalled)
            {
                if (auth.CurrentUser != null) 
                {
                user = auth.CurrentUser;
                isLoginTaskComplete = true; // 메인 스레드 UI 업데이트 신호
                Debug.Log("Firebase 세션이 남아있어 자동 로그인 되었습니다: " + user.DisplayName);
                }
                else 
                {
                    TrySilentGoogleLogin();
                    isLogoutCalled = false;
                }
            }
        }
    });
    }

    // 구글 계정 정보를 조용히 가져오는 함수
    private void TrySilentGoogleLogin()
    {
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
        if (isLoginTaskComplete)
        {
                isLoginTaskComplete = false;
                LoadingSceneController.LoadScene(gameScene);
            
        }
    }

    public void OnClickGoogleLogin()
    {
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
            DBManager.Instance.userId = user.UserId;
            DBManager.Instance.userName = user.DisplayName;
            isLoginTaskComplete = true;
        });
    }


}