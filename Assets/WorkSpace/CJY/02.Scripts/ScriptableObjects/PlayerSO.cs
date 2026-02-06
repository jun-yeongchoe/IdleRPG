using System;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStatus", menuName = "ScriptableObjects/PlayerStatus")]
public class PlayerStatus : ScriptableObject
{

    [Header("User Info")]
    public string userName;
    public int gold, gem;

    [Header("Player Stats Levels")]
    public int atkPower, hp, hpGen, atkSpeed, criticalChance, criticalDamage;

    [Header("Shop Exps")]
    public float invenShopExp, skillShopExp, partnerShopExp;

    private PlayerStatLoaderFromGoogleSheets csvLoader;

    public void Init(PlayerStatLoaderFromGoogleSheets loader)
    {
        csvLoader = loader;
    }

    public float GetAtkPower() => CalculateStat("Attack", atkPower);
    public float GetHP() => CalculateStat("HP", hp);
    public float GetHPGen() => CalculateStat("HPRegen", hpGen);
    public float GetAtkSpeed() => CalculateStat("AtkSpeed", atkSpeed);
    public float GetCriticalChance() => CalculateStat("CritChance", criticalChance);
    public float GetCriticalDamage() => CalculateStat("CritDamage", criticalDamage);

    
    float CalculateStat(string statName, int currentLevel)
    {
        if(csvLoader == null)
        {
            csvLoader = FindObjectOfType<PlayerStatLoaderFromGoogleSheets>();
            if(csvLoader ==null) return 0;
        }

        PlayerStatData_CSV data = csvLoader.GetStat(statName);
        if(data == null) return 0;

        return data.BaseValue + (currentLevel - 1) * data.GrowthPerLevel;
    }


    
    // 서버에 보낼 때 사용할 JSON용 클래스로 변환하는 함수
    public UserData ToUserData()
    {
        return new UserData(userName,gold, gem, atkPower, hp, hpGen, atkSpeed, criticalChance, criticalDamage, invenShopExp, skillShopExp, partnerShopExp);
    }
}