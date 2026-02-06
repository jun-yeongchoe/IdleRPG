using UnityEngine;

[CreateAssetMenu(menuName = "Boss/Boss Stat")]
public class BossStatSO : ScriptableObject
{
    //기본 스탯
    [Header("Base Stats")]
    public float baseHp = 3000f;
    public float baseAttack = 50f;
    public float baseMoveSpeed = 0.8f;

    // 전투 관련 스탯
    [Header("Combat")]
    public float attackRange = 2.5f;
    public float attackCooldown = 2.5f;

    // 스테이지마다 증가 스탯
    [Header("Stage Scaling")]
    public float hpPerStage = 300f;
    public float attackPerStage = 5f;
}
