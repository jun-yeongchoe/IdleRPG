using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class QuestSlot : MonoBehaviour
{
    [Header("UI 연결")]
    public TextMeshProUGUI questNameText; //퀘스트 이름 (예: 고블린 10마리 잡기)
    public TextMeshProUGUI progressText;  //진행도 숫자 (예: 5 / 10)
    public Slider progressBar;            //진행도 게이지 바
    public TextMeshProUGUI rewardText;    //보상 내용 (예: 500 젬)
    public Button claimButton;            //보상 받기 버튼
    public TextMeshProUGUI buttonText;    //버튼 안의 글씨 (진행 중 / 보상 받기 / 완료)

    private int myQuestId;

    public void Setup(QuestInfo info, QuestSaveData saveData)
    {
        myQuestId = info.questId;

        questNameText.text = info.questName;
        rewardText.text = $"보상: {info.rewardGem} 젬";

        int currentProgress = (saveData != null) ? saveData.progressValue : 0;
        bool isCleared = (saveData != null) && saveData.isCleared;

        progressText.text = $"{currentProgress} / {info.maxCount}";
        if (progressBar != null)
        {
            progressBar.maxValue = info.maxCount;
            progressBar.value = currentProgress;
        }

        claimButton.onClick.RemoveAllListeners();

        if (isCleared)
        {
            buttonText.text = "완료됨";
            claimButton.interactable = false;
        }
        else if (currentProgress >= info.maxCount)
        {
            buttonText.text = "보상 받기";
            claimButton.interactable = true;
            claimButton.onClick.AddListener(OnClickClaim);
        }
        else
        {
            buttonText.text = "진행 중";
            claimButton.interactable = false;
        }
    }

    private void OnClickClaim()
    {
        QuestManager.Instance.ClaimReward(myQuestId);

        QuestPanelUI.Instance.RefreshUI();
    }
}
