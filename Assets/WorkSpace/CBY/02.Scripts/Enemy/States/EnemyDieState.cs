using UnityEngine;

public class EnemyDieState : IEnemyState
{
    private EnemyFSM fsm;
    public bool IsTerminal => true;

    public EnemyDieState(EnemyFSM fsm)
    {
        this.fsm = fsm;
    }

    public void Enter()
    {
        fsm.enemy.MoveStop();
        fsm.enemy.Die();
    }

    public void Update() { }
    public void Exit() { }
}