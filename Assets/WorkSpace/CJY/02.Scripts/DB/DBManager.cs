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
        double scoreToSave = (double) rankingScore;

        Debug.Log($"<color=cyan>[로그 4] 데이터 준비 완료 (Score: {scoreToSave})</color>");

    
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
        rankingData["Score"] = scoreToSave;

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

        ClearAllGameData();
        this.userId = userId;
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

    // 모든 유저 관련 휘발성 데이터를 여기서 초기화
    public void ClearData() {
        userId = "";
        userName = "";
        IsDataLoaded = false;
        tempJsonData = "";
    }


public void ClearAllGameData()
{
    if (DataManager.Instance == null) return;

    Debug.Log("<color=red>[DBManager] 모든 게임 데이터를 강제 초기화합니다.</color>");

    var data = DataManager.Instance;

    // 1. 기초 스탯 및 스테이지 초기화
    data.currentStageNum = 1;
    data.Gold = 0;
    data.Scrap = 0;
    data.Gem = 0;
    
    data.AtkLv = 1;
    data.HpLv = 1;
    data.RecoverLv = 1;
    data.AtSpeedLv = 1;
    data.CritPerLv = 1;
    data.CritDmgLv = 1;

    // 2. 던전 티켓 초기화
    data.GoldDungeonTicket = 2;
    data.BossRushTicket = 2;
    data.DwarfKingTicket = 2;

    // 3. 딕셔너리(인벤토리, 퀘스트 등) 통째로 비우기
    data.InventoryDict = new Dictionary<int, ItemSaveData>();
    data.CompanionDict = new Dictionary<int, ItemSaveData>();
    data.SkillDict = new Dictionary<int, ItemSaveData>();
    data.QuestDict = new Dictionary<int, QuestSaveData>();
    
    // 4. 리스트 및 슬롯 정보 초기화
    data.SynergyName = new List<string>();
    data.EquipSlot = new int[4];
    data.SkillSlot = new int[] { 0, 1, 2, -1, -1, -1 };
    data.CompanionSlot = new int[] { -1, -1, -1 };
    
    // 5. 상점 정보 초기화
    data.ShopLevels = new int[] { 1, 1, 1 };
    data.ShopExps = new int[] { 0, 0, 0 };

    // 6. 특성(Trait) 초기화
    data.TraitSlots = new TraitSaveData[5];
    for (int i = 0; i < 5; i++)
    {
        data.TraitSlots[i] = new TraitSaveData { slotIndex = i, traitId = 0, isLocked = false };
    }

    // 7. DBManager 본인의 정보도 초기화
    this.userId = "";
    this.userName = "";
    this.IsDataLoaded = false;
    this.tempJsonData = "";

    // UI 갱신 이벤트 발생 (필요 시)
    if (EventManager.Instance != null) 
        EventManager.Instance.TriggerEvent("CurrencyChange");
        
    Debug.Log("<color=lime>[DBManager] 메모리 클린업 완료!</color>");
}

}