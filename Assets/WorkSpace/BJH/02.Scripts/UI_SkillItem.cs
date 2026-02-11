using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_SkillItem : MonoBehaviour
{
    [Header("UI 연결")]
    public Image iconImage; // 스킬 아이콘
    public TextMeshProUGUI levelText; // LV
    public TextMeshProUGUI expText; // 경험치
    public Image expBarFill; // 게이지
    public GameObject equippedMark; // 장착중 표시

    // 스킬 데이터
    private SkillData mySkillData;

    // 임시 데이터
    private int currentLevel = 1;
    private int currentExp = 0;
    private int maxExp = 2; // 레벨업에 필요한 개수

    public void Init(SkillData data)
    {
        mySkillData = data;

        if (mySkillData != null)
        {
            iconImage.sprite = mySkillData.skillIcon;
            UpdateUI(); // UI 그리기
        }
    }

    // UI 갱신
    public void UpdateUI()
    {
        // 텍스트 갱신
        levelText.text = $"Lv.{currentLevel}";
        expText.text = $"{currentExp} / {maxExp}";

        // 게이지 바 갱신
        float progress = (float)currentExp / maxExp;
        expBarFill.fillAmount = progress;
    }

    public void AddExp()
    {
        currentExp++; // 1개 획득

        // 레벨업 조건 달성?
        if (currentExp >= maxExp)
        {
            LevelUp();
        }

        UpdateUI(); // 바뀐 값으로 화면 갱신
    }

    void LevelUp()
    {
        currentExp = 0; // 경험치 초기화
        currentLevel++; // 레벨 업
        maxExp = maxExp * 2; // 다음 렙업의 최대 경험치양

    }

    // 버튼 클릭 시 실행할 함수
    public void OnClickItem()
    {
        // 팝업 관리자 호출
        SkillInfoPopup.Instance.Open(mySkillData, false);
    }
}