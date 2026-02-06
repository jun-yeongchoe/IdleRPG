using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillToolbar : MonoBehaviour
{
    [Header("오토 버튼 UI")]
    public Button autoButton; // 클릭할 버튼
    public TextMeshProUGUI autoText; // 버튼 안의 글자 (AUTO / AUTO ON)
    public Image autoButtonImage; // 버튼 배경 이미지 (본체도 상관없음)

    [Header("슬롯 연결")]
    public SkillSlot[] skillSlots; // 슬롯 순서대로

    private bool isAutoMode = false; // 오토모드 사용 여부

    private int lastCheckStage = -1; // 마지막 확인한 스테이지
    private SkillData[] cachedSkills; // 마지막 확인한 스킬목록

    void Start()
    {
        // 비교용 스킬 배열 생성
        cachedSkills = new SkillData[skillSlots.Length];

        // 1회 초괴화
        InitSlots();

        // 오토 버튼 기능 연결
        autoButton.onClick.AddListener(OnAutoButtonClicked);
        UpdateAutoButtonUI(); // 버튼 색깔 초기화
    }

    // 게임 시작시 매니저에 데이터를 슬롯에 투입
    void InitSlots()
    {
        if (MockDataHub.Instance == null) return;

        for (int i = 0; i < skillSlots.Length; i++)
        {
            // i 번째 스킬 요구
            SkillData data = MockDataHub.Instance.GetSkillAt(i);

            // 슬롯에 투입
            skillSlots[i].SetSkill(data);

            // 현재 스킬 기억 저장 (비교용)
            cachedSkills[i] = data;
        }
    }

    void Update()
    {
        if (MockDataHub.Instance != null)
        {
            int currentStage = MockDataHub.Instance.currentStage;

            // 스테이지 변경 감지
            if (currentStage != lastCheckStage)
            {
                lastCheckStage = currentStage; // 새로운 스테이지 번호 기억

                // 해금 ㅈ조건 재확인
                foreach (var slot in skillSlots) slot.RefreshSlotState();
            }

            for (int i = 0; i < skillSlots.Length; i++)
            {
                // 현재 매니저 스킬 확인
                SkillData managerSkill = MockDataHub.Instance.GetSkillAt(i);

                // 비교 후 다른지 여부
                if (managerSkill != cachedSkills[i])
                {
                    // 슬롯에 새 스킬 장착 반영
                    skillSlots[i].SetSkill(managerSkill);

                    // 저장공간도 업데이트
                    cachedSkills[i] = managerSkill;
                }
            }
        }

        if (isAutoMode)
        {
            foreach (var slot in skillSlots)
            {
                // 준비되면 실행
                if (slot.IsReady()) slot.UseSkill();
            }
        }
    }

    // 오토 버튼 클릭 시 실행되는 함수
    void OnAutoButtonClicked()
    {
        isAutoMode = !isAutoMode; // 온오프 전환
        UpdateAutoButtonUI(); // 버튼 갱신
    }

    // 오토버튼 연출 코드로 일단 대체
    void UpdateAutoButtonUI()
    {
        if (isAutoMode)
        {
            // 켜진사앹
            autoText.text = "AUTO ON";
            autoText.color = Color.yellow;
            autoButtonImage.color = Color.white;
        }
        else
        {
            // 꺼짐상태
            autoText.text = "AUTO";
            autoText.color = Color.white;
            autoButtonImage.color = new Color(0.7f, 0.7f, 0.7f, 1f);
        }
    }
}