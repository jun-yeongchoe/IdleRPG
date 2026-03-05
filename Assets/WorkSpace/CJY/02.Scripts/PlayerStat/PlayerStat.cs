using System.Collections.Generic;
using UnityEngine;

public class PlayerStat : MonoBehaviour
{
    public static PlayerStat instance { get; private set; }

    [Header("최종 계산된 스탯")]
    public float atkPower;      // 공격력 (a)
    public float hp;            // 체력 (b)
    public float atkSpeed;      // 공격속도 (c)
    public float hpGen;         // 체력재생 (d)
    public float criticalChance; // 치명타확률 (e)
    public float criticalDamage; // 치명타데미지 (f)

    private const float attackDelayDenominator = 1f;
    private PlayerStatLoaderFromGoogleSheets PSD_CSV;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        // 게임 시작 시 한 번 계산
        UpdateFinalStats();
        PSD_CSV = CSV_LoadManager.Instance.playerStats_CSV;
        
    }

    /// <summary>
    /// 모든 스탯 공식을 적용하여 최종 수치를 갱신합니다.
    /// </summary>
    public void UpdateFinalStats()
    {
        if (DataManager.Instance == null || ItemDataManager.Instance == null) return;

        // 1. 장비 보너스 합산 변수 (초기값 0 = 증가량 없음)
        float totalAtkMultiplier = 0;
        int[] atkEquipmentTargetSlot = {0,2,3};

        foreach(int slotIdx in atkEquipmentTargetSlot)
        {
            int itemId = DataManager.Instance.EquipSlot[slotIdx];
            if(itemId <= 0 ) continue;

            var data = ItemDataManager.Instance.GetEquipment(itemId);

            int level = 1;
            if(DataManager.Instance.InventoryDict.TryGetValue(itemId, out var save))
            {
                level = save.level;
            }

            float bonusValue = data.GetFinalValue(level);
            totalAtkMultiplier += (bonusValue -1);
        }
        
        float totalHpMultiplier = 0;
        int[] hpEquipmentTargetSlot = {1,2,3};

        foreach(int slotIdx in hpEquipmentTargetSlot)
        {
            int itemId = DataManager.Instance.EquipSlot[slotIdx];
            if(itemId <= 0 ) continue;

            var data = ItemDataManager.Instance.GetEquipment(itemId);

            int level = 1;
            if(DataManager.Instance.InventoryDict.TryGetValue(itemId, out var save))
            {
                level = save.level;
            }

            float bonusValue = data.GetFinalValue(level);
            totalHpMultiplier += (bonusValue -1);
        }

        // // 장착 슬롯 리스트가 null이 아닌지 확인
        // if (DataManager.Instance.EquipSlot != null)
        // {
        //     foreach (int itemID in DataManager.Instance.EquipSlot)
        //     {
        //         // 빈 슬롯(0 이하)은 계산 건너뜀
        //         if (itemID <= 0) continue;

        //         var data = ItemDataManager.Instance.GetEquipment(itemID);
                
        //         // 데이터 매니저에 해당 ID의 SO가 없는 경우 방어
        //         if (data == null)
        //         {
        //             Debug.LogWarning($"[PlayerStat] ID {itemID}에 해당하는 장비 데이터를 찾을 수 없습니다.");
        //             continue;
        //         }

        //         int level = 1;
        //         if (DataManager.Instance.InventoryDict != null && 
        //             DataManager.Instance.InventoryDict.TryGetValue(itemID, out var save))
        //         {
        //             level = save.level;
        //         }

        //         // 장비 공식 적용
        //         float bonusValue = data.GetFinalValue(level);

        //         // ID 범위 분류
        //         if (itemID <= 3999) totalAtkMultiplier += bonusValue == 0? 0 : bonusValue-1;
        //         else if (itemID <= 6999) totalHpMultiplier += bonusValue == 0? 0 : bonusValue-1;
        //         else if (itemID <= 9999)
        //         {
        //             totalAtkMultiplier += bonusValue == 0? 0 : bonusValue-1;
        //             totalHpMultiplier += bonusValue == 0? 0 : bonusValue-1;
        //         }
        //     }
        // }
        
        // 2. 최종 스탯 계산 (장비가 없으면 Bonus 값들이 0이 되어 기본 스탯만 남음)

        // 스탯 증가값 및 베이스 값 로드
        var atk_Data = PSD_CSV.GetStat(PSD_CSV.playerStatDataList[0].StatName);
        var hp_Data = PSD_CSV.GetStat(PSD_CSV.playerStatDataList[1].StatName);
        var hp_g_Data = PSD_CSV.GetStat(PSD_CSV.playerStatDataList[2].StatName);
        var atk_s_Data = PSD_CSV.GetStat(PSD_CSV.playerStatDataList[3].StatName);
        var crit_p_Data = PSD_CSV.GetStat(PSD_CSV.playerStatDataList[4].StatName);
        var crit_d_Data = PSD_CSV.GetStat(PSD_CSV.playerStatDataList[5].StatName);

        // 공격력 계산
        float baseAtk = atk_Data.BaseValue + (DataManager.Instance.AtkLv - 1) * atk_Data.GrowthPerLevel;
        atkPower = baseAtk * (1+totalAtkMultiplier);

        // 체력 계산
        float baseHp = hp_Data.BaseValue + (DataManager.Instance.HpLv - 1) * hp_Data.GrowthPerLevel;
        hp = baseHp * (1+totalHpMultiplier);


        // 공격속도, 재생, 치명타 등은 레벨 기반이므로 장비 유무와 상관없이 계산됨
        atkSpeed = attackDelayDenominator / (atk_s_Data.BaseValue + (DataManager.Instance.AtSpeedLv - 1) * atk_s_Data.GrowthPerLevel);
        hpGen = hp_g_Data.BaseValue + (DataManager.Instance.RecoverLv - 1) * hp_g_Data.GrowthPerLevel;
        criticalChance = crit_p_Data.BaseValue + (DataManager.Instance.CritPerLv - 1) * crit_p_Data.GrowthPerLevel;
        criticalDamage = crit_d_Data.BaseValue + (DataManager.Instance.CritDmgLv - 1) * crit_d_Data.GrowthPerLevel;
    }

    /// <summary>
    /// 공격 시점에 호출하여 최종 가할 데미지를 계산합니다.
    /// </summary>
    public (bool isCrit, float damage) GetAttackDamage()
    {
        // 치명타 발생 체크
        bool isCrit = Random.value <= criticalChance;

        // 치명타 여부에 따른 데미지 결정
        float finalDamage = isCrit ? atkPower * criticalDamage : atkPower;

        return (isCrit, finalDamage);
    }

    #region UI 및 레벨업 호환용 메서드
    public void SetAttackPower() 
    {
        // 레벨 데이터는 DataManager에서 가져오므로 단순히 전체 수치를 다시 계산합니다.
        UpdateFinalStats(); 
    }

    public void SetHP() 
    {
        UpdateFinalStats();
        // 체력이 바뀌었으므로 플레이어 HP바 등을 갱신하는 로직이 필요하다면 여기에 추가
    }

    public void SetHPGen() 
    {
        UpdateFinalStats();
    }

    public void SetAtkSpeed() 
    {
        UpdateFinalStats();
    }

    public void SetCriticalChance() 
    {
        UpdateFinalStats();
    }

    public void SetCriticalDamage() 
    {
        UpdateFinalStats();
    }
    #endregion
}