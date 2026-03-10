using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum SkillTargetType
{
    Single,
    Multi
}

public enum SkillSpawnPoint
{
    Player,
    Target,
    Up
}

[CreateAssetMenu(fileName = "Skill_", menuName = "SO/Item/Skill")]
public class SkillDataSo : ItemBase
{
    [Header("Skill Combat Data")]
    public SkillTargetType skillTargetType;
    public int StrikeCount;
    public float Damage_Coef;
    public float Cooltime;
    public string EffectPrefabName;
    public SkillSpawnPoint spawnPoint;
    public float SplashRadius = 2f;

     public override float GetStatMultiplier(int currentLevel)
    {
        return base.GetStatMultiplier(currentLevel);
    }

    public float GetFinalValue(int currentLevel)
    {
        return Damage_Coef * (1f + GetStatMultiplier(currentLevel));
    }

}
