using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    public float maxHp;
    public float hp;
    public float attackPower;
    public float moveSpeed;
    public float attackRange;
    public float attackCooldown;

    public void InitByStage(int stage)
    {
        maxHp = 10f + stage * 5f;
        attackPower = 2f + stage * 1.2f;
        moveSpeed = 1.5f + stage * 0.05f;
        attackCooldown = Mathf.Max(0.5f, 1.5f - stage * 0.02f);

        ResetHp();
    }

    public void ResetHp()
    {
        hp = maxHp;
    }

    public void ApplyDamage(float damage)
    {
        hp = Mathf.Max(0f, hp - damage);
    }

    public bool IsDead()
    {
        return hp <= 0f;
    }
}