using UnityEngine;

public class BossStats : MonoBehaviour
{
    [Header("Base Stats")]
    public float baseHp = 300f;
    public float hpPerStage = 50f;

    public float baseAttack = 20f;
    public float attackPerStage = 5f;

    public float moveSpeed = 2f;
    public float attackRange = 2f;
    public float attackCooldown = 2f;

    [HideInInspector] public float maxHp;
    [HideInInspector] public float currentHp;
    [HideInInspector] public float attackDamage;

    public void InitByStage()
    {
        int stage = 0;

        if (DataManager.Instance != null)
            stage = DataManager.Instance.currentStageNum;

        maxHp = baseHp + stage * hpPerStage;
        attackDamage = baseAttack + stage * attackPerStage;
        currentHp = maxHp;
    }

    public void TakeDamage(float damage)
    {
        currentHp -= damage;
    }
}
