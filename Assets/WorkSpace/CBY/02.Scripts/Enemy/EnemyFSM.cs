using UnityEngine;
using System.Collections.Generic;

public enum EnemyStateType
{
    Idle,
    Move,
    Attack,
    Die
}

public class EnemyFSM : MonoBehaviour
{
    Dictionary<EnemyStateType, IEnemyState> states;
    IEnemyState currentState;

    public Enemy enemy;

    private void Awake()
    {
        enemy = GetComponent<Enemy>();

        states = new Dictionary<EnemyStateType, IEnemyState>()
        {
            { EnemyStateType.Move, new EnemyMoveState(this) },
            { EnemyStateType.Attack, new EnemyAttackState(this) },
            { EnemyStateType.Die, new EnemyDieState(this) }
        };
    }

    private void Update()
    {
        currentState?.Update();
    }

    public void ChangeState(EnemyStateType type)
    {
        currentState?.Exit();
        currentState = states[type];
        currentState.Enter();
    }
}
