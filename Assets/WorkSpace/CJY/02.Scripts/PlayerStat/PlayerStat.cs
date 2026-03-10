using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class PlayerStat : MonoBehaviour
{
    public static PlayerStat instance { get; private set; }

    [Header("최종 계산된 스탯")]
    public BigInteger atkPower;      // 공격력 (a)
    public BigInteger hp;            // 체력 (b)
    public float atkSpeed;      // 공격속도 (c)
    public float hpGen;         // 체력재생 (d)
    public float criticalChance; // 치명타확률 (e)
    public float criticalDamage; // 치명타데미지 (f)

    private const float attackDelayDenominator = 1f;
    private PlayerStatLoaderFromGoogleSheets PSD_CSV;

    public List<SPPointData> hasSPData = new List<SPPointData>();
    private string[] spType = {"Attack_Damage", "Attack_Speed", "HP", "Critical_Chance","Critical_Damage"};

    public bool isDead = false;
    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }
    void Start()
    {
        DataManager.Instance.Gold = 1000000;
        PSD_CSV = CSV_LoadManager.Instance.playerStats_CSV;
        // 게임 시작 시 한 번 계산
        UpdateFinalStats();
    }

    /// <summary>
    /// 모든 스탯 공식을 적용하여 최종 수치를 갱신합니다.
    /// </summary>
    public void UpdateFinalStats()
    {
        if (DataManager.Instance == null || ItemDataManager.Instance == null) return;
        if (PSD_CSV == null || !PSD_CSV.isLoaded) 
        {
            Debug.LogWarning("CSV 데이터가 아직 로드되지 않아 계산을 건너뜁니다.");
            return;
        }

        // 1. 장비 보너스 합산 변수 (초기값 0 = 증가량 없음)
        float totalAtkMultiplier = 0; // 공격력은 곱
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
            totalAtkMultiplier += bonusValue - 1;
        }
        
        float totalHpMultiplier = 0; // 체력은 곱
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
            totalHpMultiplier += bonusValue - 1;
        }


        float totalAtkSpeedMultiplier = 0; //공속은 합

        float totalCritChanceBonus = 0;   //치적은 합   
        float totalCritDamageBonus = 0;   //치피는 합

        // 2. 특성 계산
        foreach(var spdata in hasSPData)
        {
            if(spdata.Type == spType[0]) totalAtkMultiplier += spdata.Rate;
            else if(spdata.Type == spType[1]) totalAtkSpeedMultiplier += spdata.Rate;
            else if(spdata.Type == spType[2]) totalHpMultiplier += spdata.Rate;
            else if(spdata.Type == spType[3]) totalCritChanceBonus += spdata.Rate;
            else if(spdata.Type == spType[4]) totalCritDamageBonus += spdata.Rate;
        }

        // 3. 최종 스탯 계산 (장비가 없으면 Bonus 값들이 0이 되어 기본 스탯만 남음)

        // 스탯 증가값 및 베이스 값 로드
        var atk_Data = PSD_CSV.GetStat(PSD_CSV.playerStatDataList[0].StatName);
        var hp_Data = PSD_CSV.GetStat(PSD_CSV.playerStatDataList[1].StatName);
        var hp_g_Data = PSD_CSV.GetStat(PSD_CSV.playerStatDataList[2].StatName);
        var atk_s_Data = PSD_CSV.GetStat(PSD_CSV.playerStatDataList[3].StatName);
        var crit_p_Data = PSD_CSV.GetStat(PSD_CSV.playerStatDataList[4].StatName);
        var crit_d_Data = PSD_CSV.GetStat(PSD_CSV.playerStatDataList[5].StatName);

        // 공격력 계산
        BigInteger baseAtk = (BigInteger)atk_Data.BaseValue + (DataManager.Instance.AtkLv - 1) * (BigInteger)atk_Data.GrowthPerLevel;
        BigInteger multiplierInt_Atk = (BigInteger)((1 + totalAtkMultiplier) * 1000);
        atkPower = ((baseAtk * multiplierInt_Atk) / 1000);

        // 체력 계산
        BigInteger baseHp = (BigInteger)hp_Data.BaseValue + (DataManager.Instance.HpLv - 1) * (BigInteger)hp_Data.GrowthPerLevel;
        BigInteger multiplierInt_Hp = (BigInteger)((1 + totalHpMultiplier) * 1000);
        hp = (baseHp * multiplierInt_Hp) / 1000;


        // 공격속도, 재생, 치명타 등은 레벨 기반이므로 장비 유무와 상관없이 계산됨
        atkSpeed = attackDelayDenominator / ((atk_s_Data.BaseValue + (DataManager.Instance.AtSpeedLv - 1) * atk_s_Data.GrowthPerLevel) + totalAtkSpeedMultiplier);
        hpGen = hp_g_Data.BaseValue + (DataManager.Instance.RecoverLv - 1) * hp_g_Data.GrowthPerLevel;
        criticalChance = crit_p_Data.BaseValue + ((DataManager.Instance.CritPerLv - 1) * crit_p_Data.GrowthPerLevel) + totalCritChanceBonus;
        criticalDamage = crit_d_Data.BaseValue + ((DataManager.Instance.CritDmgLv - 1) * crit_d_Data.GrowthPerLevel) + totalCritDamageBonus;

        EventManager.Instance.TriggerEvent("PlayerStatChange");
    }

    /// <summary>
    /// 공격 시점에 호출하여 최종 가할 데미지를 계산합니다.
    /// </summary>
    public (bool isCrit, BigInteger damage) GetAttackDamage()
    {
        // 치명타 발생 체크
        bool isCrit = Random.value <= criticalChance;

        // 치명타 여부에 따른 데미지 결정
        long critMultInt = (long)(criticalDamage * 100);
        BigInteger finalDamage = isCrit ? (atkPower * critMultInt) / 100 : atkPower;

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