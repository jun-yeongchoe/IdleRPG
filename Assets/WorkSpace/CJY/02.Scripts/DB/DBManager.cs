using Firebase.Auth;
using Firebase.Database;
using TMPro;
using UnityEngine;

public class DBManager : MonoBehaviour
{
    private DatabaseReference dbReference;
    private bool isDataLoadComplete = false;
    private UserData loadedData;
    [SerializeField] TextMeshProUGUI userNameTxt, userStageLevelTxt;

    void Start()
    {
        // 데이터베이스 루트 참조 가져오기
        dbReference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    void Update()
{
    // 데이터 로드가 완료되면 메인 스레드에서 UI를 업데이트합니다.
    if (isDataLoadComplete)
    {
        isDataLoadComplete = false;
        userNameTxt.text = "UserName : "+ loadedData.userName;
        userStageLevelTxt.text = "UserStageLevel : "+ loadedData.stageLevel.ToString();
    }
}

    // 데이터 저장 (유저 UID 사용)
    public void SaveUserData(string userId, UserData data)
    {
        string json = JsonUtility.ToJson(data);
        dbReference.Child("users").Child(userId).SetRawJsonValueAsync(json).ContinueWith(task => {
            if (task.IsCompleted) Debug.Log("데이터 저장 완료!");
        });
    }

    // 데이터 불러오기
    public void LoadUserData(string userId)
    {
        dbReference.Child("users").Child(userId).GetValueAsync().ContinueWith(task => {
        if (task.IsFaulted) {
            Debug.LogError("데이터 로드 실패");
        }
        else if (task.IsCompleted) {
            DataSnapshot snapshot = task.Result;
            
            if (snapshot.Exists) { // 데이터가 존재하는 경우
                string json = snapshot.GetRawJsonValue();
                loadedData = JsonUtility.FromJson<UserData>(json);
                isDataLoadComplete = true; // UI 업데이트 신호
            } 
            else { // 신규 유저인 경우 초기 데이터 생성
                Debug.Log("신규 유저입니다. 초기 데이터를 생성합니다.");
                UserData newData = new UserData(FirebaseAuth.DefaultInstance.CurrentUser.DisplayName, 0, 1, 10);
                SaveUserData(userId, newData);
                
                // 생성 후 다시 로드하거나 직접 할당
                loadedData = newData;
                isDataLoadComplete = true;
            }
        }
    });

        
    }
}