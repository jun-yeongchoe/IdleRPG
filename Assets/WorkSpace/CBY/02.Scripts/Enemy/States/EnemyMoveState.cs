using UnityEngine;

public class EnemyMoveState : IEnemyState
{
    EnemyFSM fsm;
    Transform target;

    public EnemyMoveState(EnemyFSM fsm)
    {
        this.fsm = fsm;
        target = GameObject.FindWithTag("Player").transform;
    }

    public void Enter() { }

    public void Update()
    {
        float dist = Vector2.Distance(fsm.transform.position, target.position);

        if (dist <= fsm.enemy.stats.attackRange)
        {
            fsm.ChangeState(EnemyStateType.Attack);
            return;
        }

        Vector3 dir = (target.position - fsm.transform.position).normalized;
        fsm.transform.position += dir * fsm.enemy.stats.moveSpeed * Time.deltaTime;
    }

    public void Exit() { }
}