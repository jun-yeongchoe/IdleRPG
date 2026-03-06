using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Partner_", menuName = "SO/Item/Partner")]
public class PartnerDataSO : ItemBase
{
    [Header("Partner Info")]
    public float AttackDamage;
    public float AttackSpeed;
    public string PrefabPath;

    [TextArea]
    public string Description;

    public float GetFindDamage(int currentLevel)
    {
        return AttackDamage * GetStatMultiplier(currentLevel);
    }

    public float GetAttackInterval()
    {
        // 0.54라면 약 1.85초마다 한 번 공격
        return 1f / AttackSpeed;
    }
}
