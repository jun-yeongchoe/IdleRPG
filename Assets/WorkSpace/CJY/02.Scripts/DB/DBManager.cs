using System.Collections;
using System.Collections.Generic;
using Firebase.Auth;
using Firebase.Database;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.Controls;
using Firebase.Extensions;
using System;
using System.Numerics;

public class DBManager : MonoBehaviour
{
    public static DBManager Instance;
    public string userId, userName;
    

    private DatabaseReference dbReference;

    [Header("UI Reference")]
    [SerializeField] TextMeshProUGUI userNameTxt, userStageLevelTxt, userGoldTxt, userAttackTxt;

    private string tempJsonData;

    public bool IsDataLoaded = false;

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

        string userId = FirebaseAuth.DefaultInstance.CurrentUser?.UserId;
        if (!string.IsNullOrEmpty(userId))
        {
            StartCoroutine(LoadUserDataCo(userId));
        }
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

    public IEnumerator SaveDataRoutine()
    {
        Debug.Log("<color=cyan>[로그 3] SaveDataRoutine 진입 성공</color>");

        string userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
        string json = DataManager.Instance.SendJson();

        if (dbReference == null)
        {
            Debug.LogWarning("[에러찾기]dbReference가 Null입니다. 다시 가져오기를 시도합니다.");
            dbReference = FirebaseDatabase.DefaultInstance.RootReference;
            
            if (dbReference == null)
            {
                Debug.LogError("[에러찾기]파이어베이스 참조를 가져오지 못했습니다. 저장을 중단합니다.");
                yield break;
            }
        }

        if (FirebaseAuth.DefaultInstance.CurrentUser == null) {
            Debug.LogError("[에러찾기]로그인된 유저가 없습니다.");
            yield break;
        }

        if (string.IsNullOrEmpty(json)) {
            Debug.LogError("저장 실패: JSON 데이터가 비어있음");
            yield break;
        }

        // 점수 계산 및 검증
        var calc = PlayerStatCalculator.instance;
        BigInteger rankingScore = calc.GetRankingScore(calc.FinalAtk, calc.FinalAtkSpeed, calc.FinalCritChance, calc.FinalCritDamage);

        Debug.Log($"<color=cyan>[로그 4] 데이터 준비 완료 (Score: {rankingScore})</color>");

    
        var userTask = dbReference.Child("users").Child(userId).SetRawJsonValueAsync(json);
        yield return new WaitUntil(() => userTask.IsCompleted);

        if (userTask.IsFaulted) {
            Debug.LogError($"[에러] users 저장 실패: {userTask.Exception}");
            yield break;
        }

        Debug.Log("<color=lime>[로그 6] Step 1(users) 저장 완료!</color>");

      
        Debug.Log("<color=yellow>[로그 7] rankings 데이터 구성 중...</color>");

        Dictionary<string, object> rankingData = new Dictionary<string, object>();
        rankingData["Nickname"] = userName;
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

    public IEnumerator LoadUserDataCo(string userId)
    {
        IsDataLoaded = false;
        Debug.Log("<color=white>[DB] 로드 시작</color>");

        if(dbReference == null) dbReference = FirebaseDatabase.DefaultInstance.RootReference;
        
        var task = dbReference.Child("users").Child(userId).GetValueAsync();
        yield return new WaitUntil(() => task.IsCompleted);

        if (task.IsCompleted && !task.IsFaulted)
        {
            DataSnapshot snapshot = task.Result;
            if (snapshot.Exists) {
                tempJsonData = snapshot.GetRawJsonValue();
                Debug.Log($"[DB] 데이터 수신: {tempJsonData}");
            }
            else {
                Debug.Log("[DB] 신규 유저 데이터 생성");
                DataManager.Instance.Gold = 10000;
                tempJsonData = DataManager.Instance.SendJson();
                var setTask = dbReference.Child("users").Child(userId).SetRawJsonValueAsync(tempJsonData);
                yield return new WaitUntil(() => setTask.IsCompleted);
            }

            try {
                Debug.Log("[DB] DataManager.LoadJson 시도");
                DataManager.Instance.LoadJson(tempJsonData);
                
                IsDataLoaded = true; 
                Debug.Log("<color=lime>[DB] IsDataLoaded 완료 처리됨!</color>");
            }
            catch (Exception e) {
                Debug.LogError($"[DB] 로드 중 예외 발생: {e.Message}");
            }
        }
    }

    //서버 저장 & 로드 코루틴
    public IEnumerator WaitServerReq()
    {
        Debug.Log("서버 저장 시작...");
        yield return StartCoroutine(SaveDataRoutine()); 

        EventManager.Instance.TriggerEvent("ServerDataChange");
    }

}