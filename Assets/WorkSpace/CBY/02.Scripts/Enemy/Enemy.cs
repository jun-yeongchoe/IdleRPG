using UnityEngine;
using System;
using System.Numerics;

public class Enemy : MonoBehaviour
{
    public EnemyStats stats;
    public EnemyFSM fsm;
    public EnemyManager enemyManager;

    private Animator animator;
    private bool isDead;

    //외부 통보용 이벤트 (골드 던전 / 기타 모드 공용)
    public event Action<Enemy> OnEnemyDead;

    private void Awake()
    {
        stats = GetComponent<EnemyStats>();
        fsm = GetComponent<EnemyFSM>();
        enemyManager = FindObjectOfType<EnemyManager>();
        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        isDead = false;

        int stage = DataManager.Instance.currentStageNum;
        stats.InitByStage(stage);

        ResetAnimator();

        fsm.Init(this, enemyManager);
        fsm.ChangeState(EnemyStateType.Move);
    }

    private void ResetAnimator()
    {
        if (animator == null) return;

        animator.SetBool("IsMoving", false);
        animator.SetBool("IsAttacking", false);
        animator.ResetTrigger("Die");
    }

    public void TakeDamage(BigInteger damage)
    {
        if (isDead) return;

        if (DamageTextManager.Instance != null) 
        { 
            DamageTextManager.Instance.ShowDamage(damage,transform.position);
        }

        if (DwarfManager.Instance != null && DwarfManager.Instance.isPlaying) 
        { 
            DwarfManager.Instance.OnBossTakeDamage(damage);
            return;
        }

        stats.ApplyDamage(damage);

        if (stats.IsDead())
        {
            isDead = true;
            fsm.ChangeState(EnemyStateType.Die);
        }
    }

    // ===== FSM → Animator 제어 =====
    public void MoveStart()
    {
        if (animator == null || isDead) return;
        animator.SetBool("IsMoving", true);
    }

    public void MoveStop()
    {
        if (animator == null) return;
        animator.SetBool("IsMoving", false);
    }

    public void Attack()
    {
        if (animator == null || isDead) return;
        animator.SetBool("IsAttacking", true);
    }

    public void Die()
    {
        if (isDead) return;

        isDead = true;

        if (animator != null)
            animator.SetTrigger("Die");

        if (DataManager.Instance != null)
        { 
            int currentStage=DataManager.Instance.currentStageNum;
            int dropGold = 10 + (currentStage * 5);

            DataManager.Instance.AddGold(dropGold);
        }

        if (QuestManager.Instance != null)
        {
            QuestManager.Instance.AddQuestProgress(QuestGoalType.KillMonster, 1);
        }
    }

    public void ForceDisappear()
    {
        if (!gameObject.activeSelf) return;

        gameObject.SetActive(false);

        OnEnemyDead?.Invoke(this);
    }

    public void OnAttackEnd()
    {
        if (animator == null) return;
        animator.SetBool("IsAttacking", false);
    }

    /// <summary>
    /// Die 애니메이션이 끝났을 때 호출
    /// </summary>
    public void OnDieEnd()
    {
        //여기서만 사망 확정 통보
        OnEnemyDead?.Invoke(this);

        gameObject.SetActive(false);
    }


    public bool IsDead()
    {
        return isDead;
    }

    public bool IsAttacking()
    {
        return animator != null && animator.GetBool("IsAttacking");
    }
}