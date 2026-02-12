using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public static SkillManager Instance;

    [Header("전체 스킬 데이터베이스 (기획서 순서대로 등록)")]
    public List<SkillData> allSkills;

    [Header("장착된 스킬 (저장 데이터 연동 예정)")]
    public int[] equippedSkillIndexes = new int[] { 0, 1, 2, -1, -1 };

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (DataManager.Instance != null)
        {
            System.Array.Copy(DataManager.Instance.SkillSlot, equippedSkillIndexes, equippedSkillIndexes.Length);
        }
    }

    public void SyncToDataManager()
    {
        if (DataManager.Instance != null)
        {
            DataManager.Instance.SkillSlot = equippedSkillIndexes;
        }
    }

    public SkillData GetSkillAt(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= equippedSkillIndexes.Length) return null;

        int skillIndex = equippedSkillIndexes[slotIndex];

        if (skillIndex < 0 || skillIndex >= allSkills.Count) return null;

        return allSkills[skillIndex];
    }

    public bool IsSlotUnlocked(int unlockStage)
    {
        if (DataManager.Instance == null) return false;

        return DataManager.Instance.currentStageNum >= unlockStage;
    }

    public int currentStage
    {
        get
        {
            if (DataManager.Instance == null) return 1;

            return DataManager.Instance.currentStageNum;
        }
    }
}
