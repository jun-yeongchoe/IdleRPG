using UnityEngine;

[CreateAssetMenu(menuName = "Boss/Boss Stat")]
public class BossStat : ScriptableObject
{
    [Header("기본 스탯")]
    public float maxHP = 3000f;     // 일반 몬스터보다 큼
    public float attackDamage = 50f;
    public float moveSpeed = 0.8f;  // 덩치 큰 느낌

    [Header("공격")]
    public float attackCooldown = 2.5f;
}
