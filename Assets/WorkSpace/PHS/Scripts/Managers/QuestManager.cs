using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//퀘스트 목표 종류(필요한 대로 계속 추가)
public enum QuestGoalType
{
    KillMonster,    //몬스터 처치
    SummonEquip,    //장비 뽑기
    ClearDungeon    //던전 클리어
}

[System.Serializable]
public class QuestInfo
{
    public int questId;           //퀘스트 ID (예: 101)
    public string questName;      //퀘스트 이름 (예: "고블린 10마리 잡기")
    public QuestGoalType goalType;//목표 종류
    public int maxCount;          //목표치 (예: 10)
    public int rewardGem;         //보상 젬 개수
}

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;

    [Header("퀘스트 DB (인스펙터에서 직접 작성)")]
    public List<QuestInfo> questDB = new List<QuestInfo>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void AddQuestProgress(QuestGoalType type, int amount = 1)
    {
        if (DataManager.Instance == null) return;

        bool isUpdated = false;

        foreach (var quest in questDB)
        {
            if (quest.goalType == type)
            {
                if (!DataManager.Instance.QuestDict.ContainsKey(quest.questId))
                {
                    DataManager.Instance.QuestDict.Add(quest.questId, new QuestSaveData
                    {
                        questId = quest.questId,
                        progressValue = 0,
                        isCleared = false
                    });
                }

                QuestSaveData saveData = DataManager.Instance.QuestDict[quest.questId];

                if (!saveData.isCleared && saveData.progressValue < quest.maxCount)
                {
                    saveData.progressValue += amount;
                    if (saveData.progressValue > quest.maxCount)
                        saveData.progressValue = quest.maxCount; //최대치 고정

                    isUpdated = true;
                    Debug.Log($"퀘스트 [{quest.questName}] 진행도 증가! ({saveData.progressValue} / {quest.maxCount})");
                }
            }
        }

        //진행도가 올랐다면 퀘스트 UI 갱신 (나중에 UI 만들 때 연결할 곳)
        if (isUpdated)
        {
            
        }
    }

    public void ClaimReward(int questId)
    {
        if (!DataManager.Instance.QuestDict.ContainsKey(questId)) return;

        QuestSaveData saveData = DataManager.Instance.QuestDict[questId];
        QuestInfo info = questDB.Find(q => q.questId == questId);

        if (info != null && !saveData.isCleared && saveData.progressValue >= info.maxCount)
        {
            DataManager.Instance.Gem += info.rewardGem;

            saveData.isCleared = true;

            Debug.Log($"퀘스트 완료! 보상으로 젬 {info.rewardGem}개 획득. (현재 젬: {DataManager.Instance.Gem})");

            // UI 갱신
        }
    }
}
