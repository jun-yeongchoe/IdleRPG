using UnityEngine;

public class EnemyFSM : MonoBehaviour
{
    public Enemy enemy;
    public Transform target;

    private IEnemyState currentState;

    public void Init(Enemy enemy, EnemyManager manager)
    {
        this.enemy = enemy;
        target = manager.GetPlayerTransform();
    }

    public void ChangeState(EnemyStateType type)
    {
        currentState?.Exit();

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
        currentState?.Update();
    }
}
