using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemRank
    {
        Common,
        UnCommon,
        Rare,
        Epic,
        Legendary,
        Mythic,
        Celestial
    }

    public enum ItemType
    {
        Equipment,
        Skill,
        Partner
    }

    public enum EquipmentType
    {
        Weapon,
        Armor,
        Accessory
    }

public class ItemBase : ScriptableObject
{
    [Header("Base Info")]
    public int ID;
    public string Name_KR;
    public string Name_EN;
    public ItemRank itemRank;
    public ItemType itemType;
    public Sprite Icon;

    [Header("Upgrade Formula Data")]
    public int BaseComposeCount;
    public float BaseStatBoost;
    public float StatPerLevel;

    // Upgrade에 필요한 재료 개수 반환
    public int GetRequiredComposeCount(int currentLevel)
    {
        return BaseComposeCount + currentLevel;
    }

    // 현 레벨에 따른 스탯 배율 계산 로직
    public virtual float GetStatMultiplier(int currentLevel)
    {
        return BaseStatBoost + (StatPerLevel * currentLevel);
    }
}
