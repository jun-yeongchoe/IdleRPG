using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

// 고려되어야할 스탯
/**********************************************
1. 공격력
    - AttackPower(PlayerStat.instance.atkPower)
    - 장비(무기)의 Value * (1+(BaseStatBoost * StatPerLevel))
    - 장비(악세사리)의 Value * (1+(BaseStatBoost * StatPerLevel))
    - 특성의 공격력 배율 
**********************************************/
/**********************************************
2. 체력
    - Health(PlayerStat.instance.health)
    - 장비(방어구)의 Value * (1+(BaseStatBoost * StatPerLevel))
    - 장비(악세사리)의 Value * (1+(BaseStatBoost * StatPerLevel))
    - 특성의 체력 배율
**********************************************/

/**********************************************
3. 공격 속도
    - AttackSpeed(PlayerStat.instance.atkSpeed)
    - 특성의 공격 속도 배율
**********************************************/
/**********************************************
4. 치명타 확률
    - CriticalChance(PlayerStat.instance.criticalChance)
    - 특성의 치명타 확률 증가
**********************************************/
/**********************************************
5. 치명타 피해
    - CriticalDamage(PlayerStat.instance.criticalDamage)
    - 특성의 치명타 피해 증가
**********************************************/

public class PlayerStatCalculator : MonoBehaviour
{
    public static PlayerStatCalculator instance{get; private set;}

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public BigInteger FinalAtk => CalculateFinalAtk();
    public BigInteger FinalHealth => CalculateFinalHealth();
    public float FinalAtkSpeed => CalculateFinalAtkSpeed();
    public float FinalCritChance => CalculateFinalCritChance();
    public float FinalCritDamage => CalculateFinalCritDamage();

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.F1))
        {
            
            Debug.Log("Final Atk: " + FinalAtk);
            Debug.Log("Final Health: " + FinalHealth);
            Debug.Log("Final Atk Speed: " + FinalAtkSpeed);
            Debug.Log("Final Crit Chance: " + FinalCritChance);
            Debug.Log("Final Crit Damage: " + FinalCritDamage);
        }
        
    }

    private BigInteger CalculateFinalAtk()
    {
        BigInteger baseAtk = new BigInteger(PlayerStat.instance.atkPower);
        float totalMultiplier = 1.0f;

        // 추후 인벤토리에 장착된 무기, 악세사리1, 악세사리2 등을 불러와서 계산
        totalMultiplier += GetEquipmentMultiPlier(GetEquipmentDataSO(EquipmentType.Weapon));
        totalMultiplier += GetEquipmentMultiPlier(GetEquipmentDataSO(EquipmentType.Accessory));
        // 추후 특성포인트에 따른 공격력 배율도 추가
        // 테스트용으로 일단 F등급의 공격력을 추가해둠. 추후에 변경 예정
        totalMultiplier += GetComponent<SPPointCSVLoader>().SPListByType["Attack_Damage"].Find(data => data.Rank == "SS").Rate;

        return BigInteger.Multiply(baseAtk, new BigInteger(totalMultiplier * 100)) / 100;

    }

    private BigInteger CalculateFinalHealth()
    {
        BigInteger baseHP = new BigInteger(PlayerStat.instance.hp);
        float totalMultiplier = 1.0f;

        totalMultiplier += GetEquipmentMultiPlier(GetEquipmentDataSO(EquipmentType.Armor));
        totalMultiplier += GetEquipmentMultiPlier(GetEquipmentDataSO(EquipmentType.Accessory));
        // 추후 특성포인트에 따른 체력 배율도 추가
        totalMultiplier += GetComponent<SPPointCSVLoader>().SPListByType["HP"].Find(data => data.Rank == "F").Rate;
        return BigInteger.Multiply(baseHP, new BigInteger(totalMultiplier * 100)) / 100;
    }

    private float CalculateFinalAtkSpeed()
    {
        float baseAtkSpeed = PlayerStat.instance.atkSpeed;
        float totalBonus = 1.0f;

        totalBonus += GetComponent<SPPointCSVLoader>().SPListByType["Attack_Speed"].Find(data => data.Rank == "F").Rate;
        return baseAtkSpeed * totalBonus;
    }

    private float CalculateFinalCritChance()
    {
        float baseCritChance = PlayerStat.instance.criticalChance;
        float totalBonus = 0.0f;
        totalBonus += GetComponent<SPPointCSVLoader>().SPListByType["Critical_Chance"].Find(data => data.Rank == "F").Rate;
        return baseCritChance + totalBonus;

    }

    private float CalculateFinalCritDamage()
    {
        float baseCritDamage = PlayerStat.instance.criticalDamage;
        float totalBonus = 0.0f;
        totalBonus += GetComponent<SPPointCSVLoader>().SPListByType["Critical_Damage"].Find(data => data.Rank == "F").Rate;
        return baseCritDamage + totalBonus;
    }

    private float GetEquipmentMultiPlier(EquipmentDataSO equipmentData)
    {
        return 0f;
    }

    public EquipmentDataSO GetEquipmentDataSO(EquipmentType equipmentType)
    {
        return null;
    }

    /*****************************
        Value : AtkDamage × AtkSpeed × [1 + Critical Rate× (Critical Damage - 1)]
    *******************************/

    public BigInteger GetRankingScore(BigInteger atk, float atkSpeed, float critChance, float critDamage)
    {
       const int p = 1000;

        // [1 + Critical Rate * (Critical Damage - 1)] 부분 계산
        // float으로 먼저 계산 후 정밀도를 위해 1000을 곱함
        float critExpectation = 1.0f + (critChance * (critDamage - 1.0f));
        BigInteger biCritPart = new BigInteger(critExpectation * p);

        // AtkSpeed 역시 정밀도를 위해 1000을 곱함
        BigInteger biAtkSpeed = new BigInteger(atkSpeed * p);

        // 최종 곱셈: Atk(BigInt) * Speed(BI) * Crit(BI)
        // 여기서 p가 두 번 곱해졌으므로 결과값은 실제보다 1,000,000배 큰 상태
        BigInteger totalScore = atk * biAtkSpeed * biCritPart;

        // 곱한 정밀도만큼 다시 나누기 (p * p = 1,000,000)
        return totalScore / (p * p);
    }
}
