using System.Collections;
using Firebase.Auth;
using Firebase.Database;
using TMPro;
using UnityEngine;

public class DBManager : MonoBehaviour
{
    public static DBManager Instance;

    private DatabaseReference dbReference;
    // private bool isDataLoadComplete = false; --> 기존
    // private UserData loadedData;

    [Header("UI Reference")]
    [SerializeField] TextMeshProUGUI userNameTxt, userStageLevelTxt, userGoldTxt, userAttackTxt;

    [Header("Data Reference (SO)")]
    public PlayerStatus playerStatus; 

    private string tempJsonData;

    [SerializeField] GameObject loadingPanel;
    bool isProcessing = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    void Start()
    {
        // // 데이터베이스 루트 참조 가져오기
        // dbReference = FirebaseDatabase.DefaultInstance.RootReference;

        // // DataManager 연동 
        // string userId = FirebaseAuth.DefaultInstance.CurrentUser?.UserId;
        // if (!string.IsNullOrEmpty(userId))
        // {
        //     StartCoroutine(LoadUserDataCo(userId));
        // }
    }

    void Update()
    {
        // if (isDataLoadComplete)
        // {
        //     isDataLoadComplete = false;
        //     SyncDataAndRefreshUI(); // 동기화 및 UI 갱신
        // }
    }

    private void OnApplicationQuit()
    {
        SaveSOData();
    }

    
    private void OnApplicationFocus(bool focus)
    {
        if (!focus) SaveSOData();
    }

    
    private void OnApplicationPause(bool pause)
    {
        if (pause) SaveSOData();
    }

    public void SyncDataAndRefreshUI()
    {
        
        // 플레이어 접속할 때, 접속종료할 때, 저장/로드 할 데이터를 새로 정의할것. -> 저장 후에 계산으로 출력가능한 데이터는 저장x

        if (playerStatus == null)
        {
            Debug.LogError("playerStatus ScriptableObject가 할당되지 않았습니다! Inspector 확인하세요.");
            return;
        }

        if (DataManager.Instance == null)
        {
            Debug.LogError("DataManager.Instance가 null입니다. Awake 순서나 DontDestroy 확인.");
            return;
        }

        if (FirebaseAuth.DefaultInstance.CurrentUser == null)
        {
            Debug.LogWarning("CurrentUser가 null인데 SyncDataAndRefreshUI 호출됨");
            return;
        }

        // 서버 데이터를 SO에 동기화
        playerStatus.userName = FirebaseAuth.DefaultInstance.CurrentUser.DisplayName;
        playerStatus.gold = DataManager.Instance.Gold;
        playerStatus.gem = DataManager.Instance.Gem;
        playerStatus.atkPower =DataManager.Instance.AtkLv;
        playerStatus.hp =DataManager.Instance.HpLv;
        playerStatus.atkSpeed =DataManager.Instance.AtSpeedLv;
        playerStatus.hpGen =DataManager.Instance.RecoverLv;
        playerStatus.criticalChance =DataManager.Instance.CritPerLv;
        playerStatus.criticalDamage =DataManager.Instance.CritDmgLv;

        // UI 텍스트 업데이트
        userNameTxt.text = "UserName : " + playerStatus.userName;
        userStageLevelTxt.text = "UserJewel : " + playerStatus.gem.ToString();
        userGoldTxt.text = "UserGold : " + playerStatus.gold.ToString();
        userAttackTxt.text = "UserAttackPower : " + playerStatus.atkPower.ToString();
        
        Debug.Log($"[로그인 성공] {playerStatus.userName}님의 데이터를 서버에서 불러와 UI에 표시했습니다.");
    }

    // [저장] SO데이터를 서버 저장
    public void SaveSOData()
    {
        if(FirebaseAuth.DefaultInstance.CurrentUser == null) 
        {
            Debug.Log("로그인 전이므로 저장 불가"); 
            return;
        }

        string userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
        if (string.IsNullOrEmpty(userId)) return;

        if(dbReference == null) 
        {
            dbReference = FirebaseDatabase.DefaultInstance.RootReference;
            
            if(dbReference == null)
            {
                Debug.LogError("Firebase Database 참조를 가져오지 못했습니다. 저장을 중단합니다.");
                return;
            }
        }
        if(DataManager.Instance == null)
        {
            Debug.LogError("DataManager 인스턴스가 없습니다. 저장을 중단합니다.");
            return;
        }

        // SO 데이터를 JSON용 클래스로 변환하여 저장
        // UserData data = playerStatus.ToUserData(); // 기존
        // string json = JsonUtility.ToJson(data);

        string json = DataManager.Instance.SendJson();

         if(string.IsNullOrEmpty(json))
         {
            Debug.LogError("JSON 데이터가 비어 있습니다. 저장을 중단합니다.");
            return;
         }

        dbReference.Child("users").Child(userId).SetRawJsonValueAsync(json).ContinueWith(task => 
        {
            if (task.IsFaulted || task.IsCanceled) 
            {
                Debug.LogError($"서버 저장 실패: {task.Exception?.Message}");
            }
            else if(task.IsCompleted)
            {
                UserData currentData = playerStatus.ToUserData();
                SaveUserData(userId, currentData); // 내부 저장 로직 호출
                Debug.Log("서버에 SO 데이터 저장 완료!");
            }
        });
    }

    //[불러오기] 앱 시작 시 서버에서 데이터를 가져와 SO에 담을 때 사용
    public void LoadUserData(string userId)
    {
        // if (dbReference == null)
        // dbReference = FirebaseDatabase.DefaultInstance.RootReference;

        // dbReference.Child("users").Child(userId).GetValueAsync().ContinueWith(task => {
        //     if (task.IsFaulted) {
        //         Debug.LogError("데이터 로드 실패");
        //     }
        //     else if (task.IsCompleted) {
        //         DataSnapshot snapshot = task.Result;
                
        //         if (snapshot.Exists) { 
        //             string json = snapshot.GetRawJsonValue();
        //             loadedData = JsonUtility.FromJson<UserData>(json);
        //             isDataLoadComplete = true; 
        //         } 
        //         else { 
        //             Debug.Log("신규 유저입니다. 초기 데이터를 생성합니다.");
        //             // 신규 유저는 초기값을 SO에 직접 설정하거나 아래처럼 생성
        //             UserData newData = new UserData(FirebaseAuth.DefaultInstance.CurrentUser.DisplayName,0,0,1,1,1,1,1,1,1,1,1);
        //             SaveUserData(userId, newData);
                    
        //             loadedData = newData;
        //             isDataLoadComplete = true;
        //         }
        //     }
        // }); //--> 기존

        StartCoroutine(LoadUserDataCo(userId));
    }

    // 기존 단순 저장 로직 (필요 시 내부 호출용)
    private void SaveUserData(string userId, UserData data)
    {
        string json = JsonUtility.ToJson(data);
        dbReference.Child("users").Child(userId).SetRawJsonValueAsync(json).ContinueWith(task => {
            if (task.IsCompleted) Debug.Log("초기 데이터 저장 완료!");
        });
    }

    IEnumerator LoadUserDataCo(string userId)
    {
        if(dbReference == null) dbReference = FirebaseDatabase.DefaultInstance.RootReference;

        if(loadingPanel != null) loadingPanel.SetActive(true);
        isProcessing = false;

        var task = dbReference.Child("users").Child(userId).GetValueAsync();
        yield return new WaitUntil(() => task.IsCompleted);

        if (task.IsFaulted)
        {
            Debug.LogError("데이터 로드 실패");
        }
        else if (task.IsCompleted)
        {
            DataSnapshot snapshot = task.Result;
            if (snapshot.Exists)
            {
                tempJsonData = snapshot.GetRawJsonValue();
            }
            else
            {
                Debug.Log("신규 유저 : 초기 데이터 생성");
                tempJsonData = DataManager.Instance.SendJson();
                dbReference.Child("users").Child(userId).SetRawJsonValueAsync(tempJsonData);
            }

            DataManager.Instance.LoadJson(tempJsonData);
            Debug.Log($"LoadJson 후 Gold 예시: {DataManager.Instance.Gold}");
            SyncDataAndRefreshUI();
            Debug.Log("SyncDataAndRefreshUI 호출 완료");
        }

        if(loadingPanel != null) loadingPanel.SetActive(false);
    } 
}