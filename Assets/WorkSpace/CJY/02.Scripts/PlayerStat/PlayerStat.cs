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
    public List<float> hasSynergy = new List<float>(4); // Synergy 증가값만 가지고오는 용도 => List 순서는 Ghost(HP), Vampire(HP gen), Hydra(ATK), Devil(Crit Dmg)
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
        UpdateFinalStats();
    }

    /// <summary>
    /// 모든 스탯 공식을 적용하여 최종 수치를 갱신합니다.
    /// 
    /// </summary>
    public void UpdateFinalStats()
    {
        if (DataManager.Instance == null || ItemDataManager.Instance == null) return;
        if (PSD_CSV == null || !PSD_CSV.isLoaded) 
        {
            Debug.LogWarning("CSV 데이터가 아직 로드되지 않아 계산을 건너뜁니다.");
            return;
        }

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
        // 유사한 스크립트 하나로 합쳐서 정리할것.

        float totalAtkSpeedMultiplier = 0; //공속은 합
        float totalHpGenMultiplier = 0;    // 체젠은 곱
        float totalCritChanceBonus = 0;   //치적은 합   
        float totalCritDamageBonus = 0;   //치피는 합

        foreach(var spdata in hasSPData)
        {
            if(spdata.Type == spType[0]) totalAtkMultiplier += spdata.Rate;
            else if(spdata.Type == spType[1]) totalAtkSpeedMultiplier += spdata.Rate;
            else if(spdata.Type == spType[2]) totalHpMultiplier += spdata.Rate;
            else if(spdata.Type == spType[3]) totalCritChanceBonus += spdata.Rate;
            else if(spdata.Type == spType[4]) totalCritDamageBonus += spdata.Rate;
        } // 간단히 수정할 수 있으면 수정할것 -> 간단한 형태로

        //시너지 계산
        // List<float> hasSynergy 
        // List 순서는 Ghost(HP): 0, Vampire(HP gen): 1, Hydra(ATK): 2, Devil(Crit Dmg): 3
        for(int i = 0; i <= hasSynergy.Count-1; i++)
        {
            if(i == 0) totalHpMultiplier+=hasSynergy[i];
            else if(i == 1) totalHpGenMultiplier+=hasSynergy[i];
            else if(i == 2) totalAtkMultiplier+=hasSynergy[i];
            else if(i == 3) totalCritDamageBonus+=hasSynergy[i];
            else break;
        }

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
        hpGen = (hp_g_Data.BaseValue + (DataManager.Instance.RecoverLv - 1) * hp_g_Data.GrowthPerLevel) * (1+totalHpGenMultiplier);
        criticalChance = crit_p_Data.BaseValue + ((DataManager.Instance.CritPerLv - 1) * crit_p_Data.GrowthPerLevel) + totalCritChanceBonus;
        criticalDamage = crit_d_Data.BaseValue + ((DataManager.Instance.CritDmgLv - 1) * crit_d_Data.GrowthPerLevel) + totalCritDamageBonus;

        EventManager.Instance.TriggerEvent("PlayerStatChange");
        DBManager.Instance.SaveSOData();
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

    // 더티플래그 디자인 패턴을 써서 필요한 시점에만 Refresh 가능하다.
}