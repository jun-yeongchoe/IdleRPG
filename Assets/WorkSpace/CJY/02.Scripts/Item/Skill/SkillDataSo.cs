using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum SkillTargetType
{
    Single,
    Multi
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
    public float SplashRadius = 2f;
}
