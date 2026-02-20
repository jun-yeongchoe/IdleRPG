using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Rank
{
    Common,
    UnCommon,
    Rare,
    Epic,
    Legendary, 
    Mythic,
    Celestial
}

public enum Type
{
    Single,
    Multi
}

public class SkillDataSO : ScriptableObject
{
    [Header("Skill info")]
    public int ID;
    public string Name_KR;
    public string Name_EN;
    public Rank rank;

    [Header("Skill Combat Data")]
    public Type type;
    public int Strike_Count;
    public float Damage_Coef;
    public float Cooltime;

    [Header("Skill FX Prefabs")]
    public string Effectprefab;


    [Header("Skill Type(=Multi) : Splash Range")]
    public float Splash_Radius = 2;
}
