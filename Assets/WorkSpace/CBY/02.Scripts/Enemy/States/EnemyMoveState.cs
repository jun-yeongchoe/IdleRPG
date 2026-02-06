using UnityEngine;

public class EnemyMoveState : IEnemyState
{
    EnemyFSM fsm;

    public EnemyMoveState(EnemyFSM fsm)
    {
        this.fsm = fsm;
    }

    public void Enter() { }

    public void Update()
    {
        Transform target = fsm.target;
        if (target == null || fsm.enemy?.stats == null) return;

        Vector3 diff = target.position - fsm.transform.position;
        float sqrDist = diff.sqrMagnitude;
        float range = fsm.enemy.stats.attackRange;

        if (sqrDist <= range * range)
        {
            fsm.ChangeState(EnemyStateType.Attack);
            return;
        }

        Vector3 dir = diff.normalized;
        fsm.transform.position += dir * fsm.enemy.stats.moveSpeed * Time.deltaTime;
    }

    public void Exit() { }
}
