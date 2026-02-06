using UnityEngine;

public class BossStats : MonoBehaviour
{
    public BossStatSO statData;

    [Header("Runtime Stats")]
    public float maxHp;
    public float hp;
    public float attack;
    public float moveSpeed;
    public float attackRange;
    public float attackCooldown;

    public void InitByStage()
    {
        int stage = DataManager.Instance.currentStageNum;

        maxHp = statData.baseHp + stage * statData.hpPerStage;
        attack = statData.baseAttack + stage * statData.attackPerStage;
        moveSpeed = statData.baseMoveSpeed;

        attackRange = statData.attackRange;
        attackCooldown = statData.attackCooldown;

        hp = maxHp;
    }

    public void TakeDamage(float damage)
    {
        hp -= damage;
        if (hp <= 0)
            Die();
    }

    void Die()
    {
        Debug.Log("Boss Defeated!");
        gameObject.SetActive(false);
    }
<<<<<<< feat/CBY
}
=======
}
>>>>>>> dev
