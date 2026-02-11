using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillDataSO : ScriptableObject
{
    public int ID;
    public string Name_KR;
    public string Name_EN;
    public string Rank;
    public string Skill_Type;
    public float Damage_Coef;
    public float Cooltime;
    public string Effect_Prefab;
    // public string Description;
}
