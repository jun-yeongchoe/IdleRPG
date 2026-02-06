using UnityEngine;

[CreateAssetMenu(menuName = "Boss/Boss Stat")]
public class BossStatSO : ScriptableObject
{
    [Header("Base Stats")]
    public float baseHp = 3000f;
    public float baseAttack = 50f;
    public float baseMoveSpeed = 0.8f;

    [Header("Combat")]
    public float attackRange = 2.5f;
    public float attackCooldown = 2.5f;

    [Header("Stage Scaling")]
    public float hpPerStage = 300f;
    public float attackPerStage = 5f;
}