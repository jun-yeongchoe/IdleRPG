using UnityEngine;

public class Enemy : MonoBehaviour
{
    public EnemyStats stats;
    public EnemyFSM fsm;
    public EnemyManager enemyManager;

    private Animator animator; // Animator 추가

    private void Awake()
    {
        stats = GetComponent<EnemyStats>();
        fsm = GetComponent<EnemyFSM>();
        enemyManager = GetComponentInParent<EnemyManager>();
        animator = GetComponent<Animator>(); // Animator 가져오기
    }

    private void OnEnable()
    {
        stats.InitByStage();
        fsm.Init(this, enemyManager);
        fsm.ChangeState(EnemyStateType.Move);
        ResetAnimator(); // 상태 초기화
    }

    private void ResetAnimator()
    {
        animator.SetBool("IsMoving", false);
        animator.SetBool("IsAttacking", false);
        animator.ResetTrigger("Die");
    }

    public void TakeDamage(float damage)
    {
        stats.hp -= damage;

        if (stats.hp <= 0)
        {
            fsm.ChangeState(EnemyStateType.Die);
        }
    }

    public void MoveStart()
    {
        animator.SetBool("IsMoving", true);
    }

    public void MoveStop()
    {
        animator.SetBool("IsMoving", false);
    }

    public void Attack()
    {
        if (enemyManager == null) return;

        Transform player = enemyManager.GetPlayerTransform();
        if (player == null) return;

        animator.SetBool("IsAttacking", true); // 공격 애니메이션 시작
        Debug.Log("플레이어에게 데미지!");
        // player.GetComponent<PlayerHealth>().TakeDamage(stats.attackPower);

        // Attack 애니 종료 후 호출 (Animation Event 추천)
        // animator.SetBool("isAttacking", false);
    }

    public void Die()
    {
        animator.SetTrigger("Die"); // Die 애니 실행
        // Die 애니 끝나는 시점에 OnDieEnd()로 비활성화 처리 가능
    }

    // Animation Event용
    public void OnAttackEnd()
    {
        animator.SetBool("IsAttacking", false);
    }

    public void OnDieEnd()
    {
        gameObject.SetActive(false);
    }
}