using UnityEngine;

public class EnemyFSM : MonoBehaviour
{
    public Enemy enemy;
    public Transform target;

    private IEnemyState currentState;
    private EnemyStateType currentType = EnemyStateType.None;

    public void Init(Enemy enemy, EnemyManager manager)
    {
        this.enemy = enemy;
        target = manager.GetPlayerTransform();
    }

    public void ChangeState(EnemyStateType type)
    {
        if (currentType == type) return;
        if (currentState != null && currentState.IsTerminal) return;

        currentState?.Exit();

        currentType = type;

        switch (type)
        {
            case EnemyStateType.Move:
                currentState = new EnemyMoveState(this);
                break;
            case EnemyStateType.Attack:
                currentState = new EnemyAttackState(this);
                break;
            case EnemyStateType.Die:
                currentState = new EnemyDieState(this);
                break;
        }

        currentState.Enter();
    }

    private void Update()
    {
        if (currentState == null || currentState.IsTerminal) return;
        currentState.Update();
    }
}