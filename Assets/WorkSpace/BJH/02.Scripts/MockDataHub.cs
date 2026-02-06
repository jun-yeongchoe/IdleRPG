using UnityEngine;

public class MockDataHub : MonoBehaviour // 임시 매니저. 나중에 스킬매니저 등 만들어지면 교체 예시.
{
    // 싱글톤
    public static MockDataHub Instance;

    [Header("현재 내 스테이지")]
    public int currentStage = 1; // 현재 스테이지

    [Header("장착된 스킬들")]
    public SkillData[] equippedSkills = new SkillData[6]; // 스킬데이터 투입

    void Awake()
    {
        // 게임 시작
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // 슬롯 해금에 관한 함수
    public bool IsSlotUnlocked(int unlockStageCondition)
    {
        // 현재 스테이지가 해금 조건보다 크거나 같으면 true
        return currentStage >= unlockStageCondition;
    }

    // 스킬 툴바에서 몇번의 스킬슬롯에 뭐가 있는지 함수
    public SkillData GetSkillAt(int index)
    {
        if (index >= 0 && index < equippedSkills.Length)
        {
            return equippedSkills[index];
        }
        return null;
    }
}