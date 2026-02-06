using Firebase.Auth;
using Firebase.Database;
using TMPro;
using UnityEngine;

public class DBManager : MonoBehaviour
{
    private DatabaseReference dbReference;
    private bool isDataLoadComplete = false;
    private UserData loadedData;

    [Header("UI Reference")]
    [SerializeField] TextMeshProUGUI userNameTxt, userStageLevelTxt, userGoldTxt, userAttackTxt;

    [Header("Data Reference (SO)")]
    public PlayerStatus playerStatus; 

    void Start()
    {
        // 데이터베이스 루트 참조 가져오기
        dbReference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    void Update()
    {
        if (isDataLoadComplete)
        {
            isDataLoadComplete = false;
            SyncDataAndRefreshUI(); // 동기화 및 UI 갱신
        }
    }

    public void SyncDataAndRefreshUI()
    {
        if (loadedData == null) return;

        // 플레이어 접속할 때, 접속종료할 때, 저장/로드 할 데이터를 새로 정의할것. -> 저장 후에 계산으로 출력가능한 데이터는 저장x

        // 서버 데이터를 SO에 동기화
        playerStatus.userName = loadedData.userName;
        playerStatus.gold = loadedData.gold;
        playerStatus.jewel = loadedData.jewel;
        playerStatus.atkPower =loadedData.atkPower;
        playerStatus.hp =loadedData.hp;
        playerStatus.atkSpeed =loadedData.atkSpeed;
        playerStatus.hpGen =loadedData.hpGen;
        playerStatus.criticalChance =loadedData.criticalChance;
        playerStatus.criticalDamage =loadedData.criticalDamage;

        // UI 텍스트 업데이트
        userNameTxt.text = "UserName : " + playerStatus.userName;
        userStageLevelTxt.text = "UserJewel : " + playerStatus.jewel.ToString();
        userGoldTxt.text = "UserGold : " + playerStatus.gold.ToString();
        userAttackTxt.text = "UserAttackPower : " + playerStatus.atkPower.ToString();
        
        Debug.Log($"[로그인 성공] {playerStatus.userName}님의 데이터를 서버에서 불러와 UI에 표시했습니다.");
    }

    // [저장] SO데이터를 서버 저장
    public void SaveSOData()
    {
        string userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
        if (string.IsNullOrEmpty(userId)) return;

        // SO 데이터를 JSON용 클래스로 변환하여 저장
        UserData data = playerStatus.ToUserData();
        string json = JsonUtility.ToJson(data);

        dbReference.Child("users").Child(userId).SetRawJsonValueAsync(json).ContinueWith(task => {
            if (task.IsCompleted) Debug.Log("서버에 SO 데이터 저장 완료!");
            else 
            {
                Debug.LogError("서버 저장 실패: " + task.Exception);
            }
        });
    }

    // [불러오기] 앱 시작 시 서버에서 데이터를 가져와 SO에 담을 때 사용
    public void LoadUserData(string userId)
    {
        if (dbReference == null)
        dbReference = FirebaseDatabase.DefaultInstance.RootReference;

        dbReference.Child("users").Child(userId).GetValueAsync().ContinueWith(task => {
            if (task.IsFaulted) {
                Debug.LogError("데이터 로드 실패");
            }
            else if (task.IsCompleted) {
                DataSnapshot snapshot = task.Result;
                
                if (snapshot.Exists) { 
                    string json = snapshot.GetRawJsonValue();
                    loadedData = JsonUtility.FromJson<UserData>(json);
                    isDataLoadComplete = true; 
                } 
                else { 
                    Debug.Log("신규 유저입니다. 초기 데이터를 생성합니다.");
                    // 신규 유저는 초기값을 SO에 직접 설정하거나 아래처럼 생성
                    UserData newData = new UserData(FirebaseAuth.DefaultInstance.CurrentUser.DisplayName, 0,0,1,1,1,1,1,1);
                    SaveUserData(userId, newData);
                    
                    loadedData = newData;
                    isDataLoadComplete = true;
                }
            }
        });
    }

    // 기존 단순 저장 로직 (필요 시 내부 호출용)
    private void SaveUserData(string userId, UserData data)
    {
        string json = JsonUtility.ToJson(data);
        dbReference.Child("users").Child(userId).SetRawJsonValueAsync(json).ContinueWith(task => {
            if (task.IsCompleted) Debug.Log("초기 데이터 저장 완료!");
        });
    }
}