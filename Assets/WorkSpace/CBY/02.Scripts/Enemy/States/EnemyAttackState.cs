using UnityEngine;

public class EnemyAttackState : IEnemyState
{
    EnemyFSM fsm;
    float lastAttackTime;

    public EnemyAttackState(EnemyFSM fsm)
    {
        this.fsm = fsm;
    }

    public void Enter()
    {
        lastAttackTime = Time.time;
    }

    public void Update()
    {
        Transform target = fsm.target;
        if (target == null || fsm.enemy?.stats == null) return;

        Vector3 diff = target.position - fsm.transform.position;
        float sqrDist = diff.sqrMagnitude;
        float range = fsm.enemy.stats.attackRange;

        if (sqrDist > range * range)
        {
            fsm.ChangeState(EnemyStateType.Move);
            return;
        }

        if (Time.time - lastAttackTime >= fsm.enemy.stats.attackCooldown)
        {
            fsm.enemy.Attack();
            lastAttackTime = Time.time;
        }
    }

    public void Exit() { }
}

