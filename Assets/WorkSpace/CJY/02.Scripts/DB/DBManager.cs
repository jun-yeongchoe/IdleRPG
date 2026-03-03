using System.Collections;
using System.Collections.Generic;
using Firebase.Auth;
using Firebase.Database;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.Controls;
using Firebase.Extensions;
using System;

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
        // 데이터베이스 루트 참조 가져오기
        dbReference = FirebaseDatabase.DefaultInstance.RootReference;

        // DataManager 연동 
        string userId = FirebaseAuth.DefaultInstance.CurrentUser?.UserId;
        if (!string.IsNullOrEmpty(userId))
        {
            StartCoroutine(LoadUserDataCo(userId));
        }
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

    // [저장] SO데이터를 서버 저장(기존)
    // public void SaveSOData()
    // {
    //     if(FirebaseAuth.DefaultInstance.CurrentUser == null) 
    //     {
    //         Debug.Log("로그인 전이므로 저장 불가"); 
    //         return;
    //     }

    //     string userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
    //     if (string.IsNullOrEmpty(userId)) return;

    //     if(dbReference == null) 
    //     {
    //         dbReference = FirebaseDatabase.DefaultInstance.RootReference;
            
    //         if(dbReference == null)
    //         {
    //             Debug.LogError("Firebase Database 참조를 가져오지 못했습니다. 저장을 중단합니다.");
    //             return;
    //         }
    //     }
    //     if(DataManager.Instance == null)
    //     {
    //         Debug.LogError("DataManager 인스턴스가 없습니다. 저장을 중단합니다.");
    //         return;
    //     }
    //     string json = DataManager.Instance.SendJson();

    //      if(string.IsNullOrEmpty(json))
    //      {
    //         Debug.LogError("JSON 데이터가 비어 있습니다. 저장을 중단합니다.");
    //         return;
    //      }


    //     dbReference.Child("users").Child(userId).SetRawJsonValueAsync(json).ContinueWith(task => 
    //     {
    //         if (task.IsFaulted || task.IsCanceled) 
    //         {
    //             Debug.LogError($"서버 저장 실패: {task.Exception?.Message}");
    //         }
    //         else if(task.IsCompleted)
    //         {
                
    //             UserData currentData = playerStatus.ToUserData();
    //             SaveUserData(userId, currentData); // 내부 저장 로직 호출
    //             Debug.Log("서버에 SO 데이터 저장 완료!");
    //         }
    //     });
        
    // }

    //[불러오기] 앱 시작 시 서버에서 데이터를 가져와 SO에 담을 때 사용
    
    public void SaveSOData()
{
    Debug.Log("<color=yellow>[로그 1] SaveSOData 호출됨</color>");

    if (FirebaseAuth.DefaultInstance.CurrentUser == null) 
    {
        Debug.LogWarning("저장 실패: 로그인된 유저가 없음");
        return;
    }

    // 코루틴 실행
    Debug.Log("<color=yellow>[로그 2] SaveDataRoutine 코루틴 시작 시도</color>");
    StartCoroutine(SaveDataRoutine());
}

private IEnumerator SaveDataRoutine()
{
    Debug.Log("<color=cyan>[로그 3] SaveDataRoutine 진입 성공</color>");

    string userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
    string json = DataManager.Instance.SendJson();

    if (string.IsNullOrEmpty(json)) {
        Debug.LogError("저장 실패: JSON 데이터가 비어있음");
        yield break;
    }

    // 점수 계산 및 검증
    var calc = PlayerStatCalculator.instance;
    double rankingScore = (double)calc.GetRankingScore(calc.FinalAtk, calc.FinalAtkSpeed, calc.FinalCritChance, calc.FinalCritDamage);
    if (double.IsInfinity(rankingScore) || double.IsNaN(rankingScore)) rankingScore = 0;

    Debug.Log($"<color=cyan>[로그 4] 데이터 준비 완료 (Score: {rankingScore})</color>");

    // Step 1: users 저장
    var userTask = dbReference.Child("users").Child(userId).SetRawJsonValueAsync(json);
    yield return new WaitUntil(() => userTask.IsCompleted);

    if (userTask.IsFaulted) {
        Debug.LogError($"[에러] users 저장 실패: {userTask.Exception}");
        yield break;
    }

    Debug.Log("<color=lime>[로그 6] Step 1(users) 저장 완료!</color>");

    
    
    // 닉네임이 null일 경우를 대비해 확실히 처리
    string safeNickname = "UnknownPlayer";
    if (playerStatus != null && !string.IsNullOrEmpty(playerStatus.userName)) {
        safeNickname = playerStatus.userName;
    } 
    else if (FirebaseAuth.DefaultInstance.CurrentUser != null) {
        safeNickname = FirebaseAuth.DefaultInstance.CurrentUser.DisplayName ?? "User_" + userId.Substring(0, 4);
    }

    Debug.Log("<color=yellow>[로그 7] rankings 데이터 구성 중...</color>");

    Dictionary<string, object> rankingData = new Dictionary<string, object>();
    rankingData["Nickname"] = safeNickname;
    rankingData["Score"] = rankingScore;

    Debug.Log("<color=yellow>[로그 8] rankings 노드 업데이트 요청 보냄...</color>");
    var rankingTask = dbReference.Child("rankings").Child(userId).UpdateChildrenAsync(rankingData);
    
    yield return new WaitUntil(() => rankingTask.IsCompleted);

    if (rankingTask.IsFaulted) {
        Debug.LogError($"[에러] rankings 저장 실패: {rankingTask.Exception}");
    } else {
        Debug.Log("<color=orange>[최종 저장 완료] rankings 노드 반영 성공!</color>");
    }
}

    public void LoadUserData(string userId)
    {
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
                DataManager.Instance.Gold = 10000;  //테스트용
                tempJsonData = DataManager.Instance.SendJson();
                dbReference.Child("users").Child(userId).SetRawJsonValueAsync(tempJsonData);
            }

            DataManager.Instance.LoadJson(tempJsonData);

            Debug.Log($"LoadJson 후 Gold 예시: {DataManager.Instance.Gold}");
            Debug.Log("==========================");
            Debug.Log($"JSON 원본 : {tempJsonData}");
            Debug.Log("==========================");
            SyncDataAndRefreshUI();
            Debug.Log("SyncDataAndRefreshUI 호출 완료");
        }

        if(loadingPanel != null) loadingPanel.SetActive(false);
    } 

}