using UnityEngine;

public class EnemyMoveState : IEnemyState
{
    private EnemyFSM fsm;
    private const float ATTACK_ENTER_MARGIN = 0.9f;

    public bool IsTerminal => false;

    public EnemyMoveState(EnemyFSM fsm)
    {
        this.fsm = fsm;
    }

    public void Enter()
    {
        fsm.enemy.MoveStart();
    }

    public void Update()
    {
        if (fsm.enemy.IsDead()) return;

        Vector3 diff = fsm.target.position - fsm.transform.position;
        float range = fsm.enemy.stats.attackRange;

        if (diff.sqrMagnitude <= range * range * ATTACK_ENTER_MARGIN)
        {
            fsm.ChangeState(EnemyStateType.Attack);
            return;
        }

        if (fsm.enemy.IsAttacking()) return;

        fsm.transform.position += diff.normalized * fsm.enemy.stats.moveSpeed * Time.deltaTime;
    }

    public void Exit()
    {
        fsm.enemy.MoveStop();
    }
}