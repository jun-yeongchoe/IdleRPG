using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PartnerData
{
    public int ID;
    public string Name_KR;
    public string Name_EN;
    public string Rank;
    public float Atk_Damage;
    public float Atk_Speed;

    // 동료 레벨업시 증가하는 스탯과 관련된 파라미터
    public float Base_ComposeStat;
    public float Stat_Per_Lv;
    // 동료 레벨업시 증가하는 스탯과 관련된 파라미터
    public string Prefab_Path;
    public string Description_KR;
}
