using UnityEngine;

public class IdleState : IEnemyState
{
    Enemy enemy;

    public IdleState(Enemy enemy)
    {
        this.enemy = enemy;
    }

    public void Enter()
    {
        Debug.Log("Idle");
    }

    public void Update()
    {
        float dist = Vector3.Distance(
            enemy.transform.position,
            enemy.target.position
        );

        if (dist > enemy.attackRange)
            enemy.stateMachine.ChangeState(new MoveState(enemy));
        else
            enemy.stateMachine.ChangeState(new AttackState(enemy));
    }

    public void Exit() { }
}

