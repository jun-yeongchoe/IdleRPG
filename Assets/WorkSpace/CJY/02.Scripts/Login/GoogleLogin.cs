using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Google;
using Firebase.Auth;
using Firebase;
using TMPro;
using System.Collections;
using UnityEngine.Networking;
using System;
using Firebase.Extensions;

public class GoogleLogin : MonoBehaviour
{
    [Header("Google Settings")]
    private string webClientId = "817724772015-cpr9qe1clkj5d1b0cj7hie4qaa2lqe9i.apps.googleusercontent.com"; //
    private string gameScene = "Game Scene_1st";
    private FirebaseAuth auth;
    private FirebaseUser user;
    [SerializeField] private TextMeshProUGUI autoLogin;


    // 메인 스레드에서 UI를 업데이트하기 위한 플래그
    private bool isLoginTaskComplete = false;

    // 로그아웃을 통해 돌아온 경우 자동 로그인을 방지하기 위한 변수
    public static bool isLogoutCalled = false;

    void Start()
    {
        autoLogin.gameObject.SetActive(false);
        GoogleSignInConfiguration config = new GoogleSignInConfiguration {
            WebClientId = webClientId,
            RequestIdToken = true,
            RequestEmail = true
        };
        
        GoogleSignIn.Configuration = config;

        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
        if (task.Result == DependencyStatus.Available) {
            
            auth = FirebaseAuth.DefaultInstance; 

            if (!isLogoutCalled) {
                if (auth.CurrentUser != null) {
                    HandleSuccessLogin(auth.CurrentUser);
                } else {
                    TrySilentGoogleLogin();
                }
            }
            else {
                isLogoutCalled = false; 
                if(DBManager.Instance != null) DBManager.Instance.IsDataLoaded = false;
                Debug.Log("로그아웃 상태로 진입.");
                }
            }
        });

        
    }

    // 구글 계정 정보를 조용히 가져오는 함수
    private void TrySilentGoogleLogin()
    {
        // SignIn 대신 SignInSilently를 사용합니다.
        GoogleSignIn.DefaultInstance.SignInSilently().ContinueWithOnMainThread(task => {
            if (!task.IsFaulted && !task.IsCanceled) {
                Debug.Log("구글 Silent 로그인 성공! Firebase 인증 시도...");
                SignInWithFirebase(task.Result.IdToken);
            }
        });
    }


    public void OnClickGoogleLogin()
    {
        GoogleSignIn.DefaultInstance.SignIn().ContinueWithOnMainThread(task => {
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

        auth.SignInAndRetrieveDataWithCredentialAsync(credential).ContinueWithOnMainThread(task => {
            if (task.IsFaulted || task.IsCanceled) {
                Debug.LogError("Firebase 인증 실패");
                return;
            }
            HandleSuccessLogin(task.Result.User);
        });
    }
    private void HandleSuccessLogin(FirebaseUser firebaseUser)
    {
        user = firebaseUser;
        StartCoroutine(WaitAndLoadProcess());
    }

    IEnumerator WaitAndLoadProcess()
    {
        yield return new WaitUntil(() => DBManager.Instance != null);

        DBManager.Instance.IsDataLoaded = false;

        DBManager.Instance.userId = user.UserId;
        DBManager.Instance.userName = user.DisplayName;

        isLoginTaskComplete = true;
        autoLogin.gameObject.SetActive(true);

        Debug.Log("<color=cyan>서버 데이터 로드 시작...</color>");
        yield return StartCoroutine(DBManager.Instance.LoadUserDataCo(user.UserId));

        yield return new WaitUntil(() => DBManager.Instance.IsDataLoaded);

        Debug.Log("<color=lime>모든 데이터 로드 완료. 씬 전환합니다.</color>");
        LoadingSceneController.LoadScene(gameScene);
    }
}