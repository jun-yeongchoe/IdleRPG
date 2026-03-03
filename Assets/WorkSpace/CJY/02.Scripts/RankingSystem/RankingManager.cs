using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Firebase.Auth;
using Firebase.Database;
using TMPro;
using UnityEngine;

public class RankingManager : MonoBehaviour
{
    [Header("My Info Slot")]
    public TextMeshProUGUI myRankTxt, myNameTxt, myScoreTxt;

    [Header("UI Ref")]
    public GameObject[] rankingSlots;
    public TextMeshProUGUI loadingTxt;

    private DatabaseReference dbRef;

    private void Awake()
    {
        // Start보다 빠른 Awake에서 미리 참조 시도
        InitializeFirebase();
    }

    private void InitializeFirebase()
    {
        if (dbRef == null)
        {
            // 참조 방식 최적화
            dbRef = FirebaseDatabase.DefaultInstance.GetReference("rankings");
            Debug.Log("<color=lime>[Ranking] Firebase Reference 초기화 완료</color>");
        }
    }

    // void Start()
    // {
    //     dbRef = FirebaseDatabase.DefaultInstance.RootReference.Child("rankings");
    //     loadingTxt.gameObject.SetActive(false);
    // }

    public void OnClickOpenRankingBoard()
    {
        gameObject.SetActive(true);
        StopAllCoroutines();
        StartCoroutine(LoadRankingBoard());
    }
    
    public void OnClickClose()
    {
        gameObject.SetActive(false);
    }

   IEnumerator LoadRankingBoard()
    {
        Debug.Log("<color=yellow>[Ranking] 데이터 로드 시작...</color>");
        loadingTxt.text = "LOADING...";
        loadingTxt.gameObject.SetActive(true);
        ClearSlots();

        // 1. Firebase 참조 확인
        if (dbRef == null)
        {
            Debug.LogError("[Ranking] dbRef가 null입니다! Start에서 참조를 가져오지 못했을 수 있습니다.");
            yield break;
        }

        // 2. 데이터 요청
        var task = dbRef.OrderByChild("Score").LimitToLast(100).GetValueAsync();
        yield return new WaitUntil(() => task.IsCompleted);

        if (task.IsCompleted && !task.IsFaulted)
        {
            loadingTxt.gameObject.SetActive(false);
            DataSnapshot snapshot = task.Result;

            // 3. 스냅샷 존재 여부 확인
            if (snapshot == null || !snapshot.Exists)
            {
                Debug.LogWarning("[Ranking] 스냅샷이 없거나 비어있습니다. 'rankings' 노드에 데이터가 있는지 확인하세요.");
                yield break;
            }

            Debug.Log($"[Ranking] 총 {snapshot.ChildrenCount}명의 데이터를 서버에서 가져왔습니다.");

            string myUid = FirebaseAuth.DefaultInstance.CurrentUser?.UserId;
            List<DataSnapshot> allUsers = snapshot.Children.ToList();
            allUsers.Reverse(); // 내림차순

            UpdateMyInfo(allUsers, myUid);

            // 4. 슬롯 업데이트 로그
            for (int i = 0; i < rankingSlots.Length; i++)
            {
                if (i < allUsers.Count)
                {
                    var user = allUsers[i];
                    string displayName = user.Child("Nickname").Exists ? 
                        user.Child("Nickname").Value.ToString() : "User_" + user.Key.Substring(0, 5);

                    double score = 0;
                    if (user.Child("Score").Exists)
                    {
                        double.TryParse(user.Child("Score").Value.ToString(), out score);
                    }
                    else
                    {
                        Debug.LogWarning($"[Ranking] 유저 {user.Key}의 'Score' 필드가 존재하지 않습니다.");
                    }

                    Debug.Log($"[Ranking] 슬롯 {i}번 업데이트: {displayName} / {score}");
                    SetSlotData(rankingSlots[i], i + 1, displayName, score);
                    rankingSlots[i].SetActive(true);
                }
                else
                {
                    rankingSlots[i].SetActive(false);
                }
            }
        }
        else
        {
            loadingTxt.text = "ERROR";
            Debug.LogError($"[Ranking] 데이터 로드 실패: {task.Exception}");
        }
    }

    private void UpdateMyInfo(List<DataSnapshot> allUsers, string myUid)
    {
        if (string.IsNullOrEmpty(myUid)) return;

        int myRank = 0;
        double myScore = 0;
        string myName = "Unknown";

        for (int i = 0; i < allUsers.Count; i++)
        {
            if (allUsers[i].Key == myUid)
            {
                myRank = i + 1;
                
            
                if (allUsers[i].Child("Nickname").Exists)
                {
                    myName = allUsers[i].Child("Nickname").Value.ToString();
                }

                if (allUsers[i].Child("Score").Exists && allUsers[i].Child("Score").Value != null)
                {
                    double.TryParse(allUsers[i].Child("Score").Value.ToString(), out myScore);
                }
                break;
            }
        }

        // UI 텍스트 할당 전 null 체크
        if (myRankTxt != null) myRankTxt.text = myRank > 0 ? myRank.ToString() : "-";
        if (myNameTxt != null) myNameTxt.text = myName;
        if (myScoreTxt != null) myScoreTxt.text = myRank > 0 ? myScore.ToString("N0") : "순위 밖";
    }

    private void SetSlotData(GameObject slot, int rank, string name, double score)
    {
        // [수정] GetComponentsInChildren 대신 이름으로 찾는 방식이 더 안전합니다. (순서 꼬임 방지)
        // 만약 자식 오브젝트 이름이 다르다면 hierarchy에 맞춰 수정하세요.
        TextMeshProUGUI rankTxt = slot.transform.Find("RankTxt")?.GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI nameTxt = slot.transform.Find("NameTxt")?.GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI scoreTxt = slot.transform.Find("ScoreTxt")?.GetComponent<TextMeshProUGUI>();

        // 이름으로 못 찾았을 경우 기존 인덱스 방식 사용 (백업)
        if (rankTxt == null || nameTxt == null || scoreTxt == null)
        {
            TextMeshProUGUI[] texts = slot.GetComponentsInChildren<TextMeshProUGUI>();
            if (texts.Length >= 3)
            {
                texts[0].text = rank.ToString("D2");
                texts[1].text = name;
                texts[2].text = score.ToString("N0");
            }
            else
            {
                Debug.LogError($"[Ranking] {slot.name} 슬롯의 텍스트 컴포넌트 구조가 올바르지 않습니다.");
            }
        }
        else
        {
            rankTxt.text = rank.ToString("D2");
            nameTxt.text = name;
            scoreTxt.text = score.ToString("N0");
        }
    }

    private void ClearSlots()
    {
        foreach(var slot in rankingSlots)
        {
            slot.SetActive(false);
        }
    }
}
