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
    [SerializeField] TextMeshProUGUI loginTxt, userIdTxt, userEmailTxt, loadingTxt;
    [SerializeField] Image profileImg;
    [SerializeField] GameObject loginPanel;

    // 메인 스레드에서 UI를 업데이트하기 위한 플래그
    private bool isLoginTaskComplete = false;

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

            if (auth.CurrentUser != null) {
                user = auth.CurrentUser;
                isLoginTaskComplete = true; // 메인 스레드 UI 업데이트 신호
                Debug.Log("Firebase 세션이 남아있어 자동 로그인 되었습니다: " + user.DisplayName);
            }
            else {
            
            TrySilentGoogleLogin();
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
            UpdateUI();
            loadingTxt.gameObject.SetActive(true);
            StartCoroutine(Delay());
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
            isLoginTaskComplete = true;
        });
    }

    private void UpdateUI()
    {
        if (user == null) return;
        DBManager DBM = FindObjectOfType<DBManager>();

        if (DBM != null)
        {
            Debug.Log("신규 유저 혹은 초기 설정을 위해 SO 데이터를 서버에 저장합니다.");
            DBM.LoadUserData(user.UserId); 
        }
        else
        {
            Debug.LogError("Hierarchy에 DBManager 오브젝트가 없습니다! 오브젝트를 생성하고 스크립트를 붙여주세요.");
        }

        loginTxt.text = "Sign In Status : Login";
        
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

        if(!loginPanel.activeSelf) loginPanel.SetActive(true);
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

    IEnumerator Delay()
    {
        yield return new WaitForSecondsRealtime(5f);
        loginPanel.SetActive(false);
        yield return null;
        loadingTxt.gameObject.SetActive(false);
    }
}