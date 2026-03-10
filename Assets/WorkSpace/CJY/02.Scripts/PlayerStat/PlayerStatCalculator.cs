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

    public BigInteger FinalAtk => PlayerStat.instance.atkPower;
    public BigInteger FinalHealth => PlayerStat.instance.hp;
    public float FinalAtkSpeed => PlayerStat.instance.atkSpeed;
    public float FinalCritChance => PlayerStat.instance.criticalChance;
    public float FinalCritDamage => PlayerStat.instance.criticalDamage;

    void Update()
    {
        // 디버그용 출력 기능 유지
        if (Input.GetKeyDown(KeyCode.F1))
        {
            Debug.Log($"[Ranking Stats] Atk: {FinalAtk}, HP: {FinalHealth}, Speed: {FinalAtkSpeed}, Crit: {FinalCritChance * 100}%, CritDmg: {FinalCritDamage * 100}%");
            Debug.Log($"[Ranking Score] : {GetCurrentRankingScore()}");
        }
    }

    public BigInteger GetCurrentRankingScore()
    {
        return GetRankingScore(FinalAtk, FinalAtkSpeed, FinalCritChance, FinalCritDamage);
    }

    public BigInteger GetRankingScore(BigInteger atk, float atkSpeed, float critChance, float critDamage)
    {
        BigInteger p = 10000;

        float actualSpeed = 1.0f / atkSpeed;

        BigInteger biSpeed = new BigInteger(actualSpeed * 10000);
        BigInteger biCritChance = new BigInteger(critChance * 10000);
        BigInteger biCritDamage = new BigInteger(critDamage * 10000);

        BigInteger critFactor = p + (biCritChance * (biCritDamage - p) / p);
        BigInteger totalScore = (atk * biSpeed * critFactor) / (p * p);

        return totalScore;
    }
}
