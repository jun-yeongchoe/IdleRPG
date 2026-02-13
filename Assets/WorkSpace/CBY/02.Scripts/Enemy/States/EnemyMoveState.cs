using UnityEngine;

public class EnemyMoveState : IEnemyState
{
    private EnemyFSM fsm;

    public EnemyMoveState(EnemyFSM fsm)
    {
        this.fsm = fsm;
    }

    public void Enter()
    {
        // Move 상태 진입 시 애니메이션 시작
        fsm.enemy.MoveStart();
    }

    public void Update()
    {
        Transform target = fsm.target;
        if (target == null || fsm.enemy?.stats == null) return;

        Vector3 diff = target.position - fsm.transform.position;
        float sqrDist = diff.sqrMagnitude;
        float range = fsm.enemy.stats.attackRange;

        // 사거리 안으로 들어오면 Attack 상태로 전환
        if (sqrDist <= range * range)
        {
            fsm.ChangeState(EnemyStateType.Attack);
            return;
        }

        // 플레이어 방향으로 이동
        Vector3 dir = diff.normalized;
        fsm.transform.position += dir * fsm.enemy.stats.moveSpeed * Time.deltaTime;
    }

    public void Exit()
    {
        // Move 상태 종료 시 Move 애니메이션 정지
        fsm.enemy.MoveStop();
    }
}