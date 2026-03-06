using UnityEngine;

public class EnemyAttackState : IEnemyState
{
    private EnemyFSM fsm;
    private float lastAttackTime;

    public bool IsTerminal => false;

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
        if (fsm.enemy.IsDead()) return;

        Vector3 diff = fsm.target.position - fsm.transform.position;
        float range = fsm.enemy.stats.attackRange;

        if (diff.sqrMagnitude > range * range)
        {
            fsm.ChangeState(EnemyStateType.Move);
            return;
        }

        if (fsm.enemy.IsAttacking()) return;

        if (Time.time - lastAttackTime >= fsm.enemy.stats.attackCooldown)
        {
            fsm.enemy.Attack();
            lastAttackTime = Time.time;
        }
    }

    public void Exit() { }
}