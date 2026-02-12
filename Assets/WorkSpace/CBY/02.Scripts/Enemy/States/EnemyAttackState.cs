using UnityEngine;

public class EnemyAttackState : IEnemyState
{
    private EnemyFSM fsm;
    private float lastAttackTime;

    public EnemyAttackState(EnemyFSM fsm)
    {
        this.fsm = fsm;
    }

    public void Enter()
    {
        lastAttackTime = Time.time;

        // 공격 상태 진입 시 애니메이션 시작
        fsm.enemy.Attack();
    }

    public void Update()
    {
        Transform target = fsm.target;
        if (target == null || fsm.enemy?.stats == null) return;

        Vector3 diff = target.position - fsm.transform.position;
        float sqrDist = diff.sqrMagnitude;
        float range = fsm.enemy.stats.attackRange;

        // 사거리 밖이면 Move 상태로 전환
        if (sqrDist > range * range)
        {
            fsm.ChangeState(EnemyStateType.Move);
            return;
        }

        // 공격 쿨다운 체크
        if (Time.time - lastAttackTime >= fsm.enemy.stats.attackCooldown)
        {
            fsm.enemy.Attack(); // 공격 애니 + 데미지
            lastAttackTime = Time.time;
        }
    }

    public void Exit()
    {
        // 공격 상태 종료 시 공격 애니 false
        fsm.enemy.OnAttackEnd();
    }
}