using UnityEngine;

public class Enemy : MonoBehaviour
{
    public EnemyStats stats;
    public EnemyFSM fsm;
    public EnemyManager enemyManager;

    private Animator animator;
    private bool isDead;

    private void Awake()
    {
        stats = GetComponent<EnemyStats>();
        fsm = GetComponent<EnemyFSM>();
        enemyManager = GetComponentInParent<EnemyManager>();
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

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        stats.ApplyDamage(damage);

        if (stats.IsDead())
        {
            isDead = true;
            fsm.ChangeState(EnemyStateType.Die);
        }
    }

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
        if (animator == null) return;
        animator.SetTrigger("Die");
    }

    // Animation Events
    public void OnAttackEnd()
    {
        if (animator == null) return;
        animator.SetBool("IsAttacking", false);
    }

    public void OnDieEnd()
    {
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