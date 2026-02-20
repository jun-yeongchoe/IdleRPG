using UnityEngine;
using System;

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

    public float MaxHp { get; private set; }
    public float CurrentHp { get; private set; }
    public float AttackDamage { get; private set; }

    public float HpRatio => CurrentHp / MaxHp;

    // ªÁ∏¡ ¿Ã∫•∆Æ
    public event Action OnDeath;

    public void InitByStage(int stage)
    {
        MaxHp = baseHp + stage * hpPerStage;
        AttackDamage = baseAttack + stage * attackPerStage;
        CurrentHp = MaxHp;
    }

    public void TakeDamage(float damage)
    {
        if (CurrentHp <= 0) return;

        CurrentHp -= damage;

        if (CurrentHp <= 0)
        {
            CurrentHp = 0;
            OnDeath?.Invoke();
        }
    }

    public void Heal(float amount)
    {
        if (CurrentHp <= 0) return;

        CurrentHp = Mathf.Min(CurrentHp + amount, MaxHp);
    }
}