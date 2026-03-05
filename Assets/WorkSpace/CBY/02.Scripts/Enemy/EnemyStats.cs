using System.Numerics;
using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    public BigInteger maxHp;
    public BigInteger hp;
    public float attackPower;
    public float moveSpeed;
    public float attackRange;
    public float attackCooldown;

    public void InitByStage(int stage)
    {
        maxHp = (BigInteger)(10f + stage * 5f);
        attackPower = 2f + stage * 1.2f;
        moveSpeed = 1.5f + stage * 0.05f;
        attackCooldown = Mathf.Max(0.5f, 1.5f - stage * 0.02f);
        attackRange = 1.5f;
        ResetHp();
    }

    public void ResetHp()
    {
        hp = maxHp;
    }

    public void ApplyDamage(BigInteger damage)
    {
        hp -= damage;
        if(hp < 0) hp = 0;
    }

    public bool IsDead()
    {
        return hp <= 0;
    }
}