using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestPanelUI : MonoBehaviour
{
    public static QuestPanelUI Instance;

    [Header("UI 연결")]
    public Transform contentTarget;   //슬롯들이 생성될 부모 객체 (Scroll View 안의 Content)
    public GameObject questSlotPrefab; //QuestSlotUI 프리팹

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void OnEnable()
    {
        RefreshUI();
    }

    public void RefreshUI()
    {
        if (QuestManager.Instance == null || DataManager.Instance == null) return;

        foreach (Transform child in contentTarget)
        {
            Destroy(child.gameObject);
        }

        foreach (var quest in QuestManager.Instance.questDB)
        {
            GameObject slotObj = Instantiate(questSlotPrefab, contentTarget);
            QuestSlot slot = slotObj.GetComponent<QuestSlot>();

            if (slot != null)
            {
                QuestSaveData saveData = null;
                if (DataManager.Instance.QuestDict.ContainsKey(quest.questId))
                {
                    saveData = DataManager.Instance.QuestDict[quest.questId];
                }

                slot.Setup(quest, saveData);
            }
        }
    }
}
