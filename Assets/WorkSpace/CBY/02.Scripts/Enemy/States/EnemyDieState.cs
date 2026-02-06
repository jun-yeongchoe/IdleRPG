using UnityEngine;

public class EnemyDieState : IEnemyState
{
    EnemyFSM fsm;

    public EnemyDieState(EnemyFSM fsm)
    {
        this.fsm = fsm;
    }

    public void Enter()
    {
        fsm.enemy.Die();
    }

    public void Update() { }
    public void Exit() { }
}