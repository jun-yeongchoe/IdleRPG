using UnityEngine;

// 보스의 스탯을 관리하는 클래스
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

    // 스테이지에 따라 보스 스탯 초기화
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
