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
        // 이전 상태 종료
        currentState?.Exit();

        // 새 상태 생성
        switch (type)
        {
            case EnemyStateType.Move:
                currentState = new EnemyMoveState(this);
                enemy.MoveStart(); // Move 애니메이션 시작
                break;

            case EnemyStateType.Attack:
                currentState = new EnemyAttackState(this);
                enemy.Attack(); // Attack 애니메이션 시작
                break;

            case EnemyStateType.Die:
                currentState = new EnemyDieState(this);
                enemy.Die(); // Die 애니메이션 시작
                break;
        }

        currentState.Enter();
    }

    private void Update()
    {
        currentState?.Update();

        // 상태에 따라 Move 애니 제어 (자동 Stop)
        if (currentState is EnemyMoveState)
        {
            // 적 이동 중이라면 Move 애니 활성
            enemy.MoveStart();
        }
        else
        {
            enemy.MoveStop();
        }
    }
}