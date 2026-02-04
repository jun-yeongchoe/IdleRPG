using UnityEngine;

// 적의 공격 상태를 나타내는 클래스
public class AttackState : IEnemyState
{
    // 공격을 수행하는 적 객체
    Enemy enemy;
    float attackDelay = 1f;
    float timer;

    // 생성자: 공격 상태를 초기화
    public AttackState(Enemy enemy)
    {
        this.enemy = enemy;
    }

    // 상태에 진입할 때 실행되는 코드
    public void Enter()
    {
        timer = attackDelay;
        Debug.Log("Attack");
    }

    // 상태가 활성화되는 동안 매 프레임 실행되는 코드
    public void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            Debug.Log("공격!");
            timer = attackDelay;
        }

        // 타겟과의 거리를 계산
        float dist = Vector3.Distance(
            enemy.transform.position,
            enemy.target.position
        );

        // 타겟이 공격 범위를 벗어나면 이동 상태로 전환
        if (dist > enemy.attackRange)
            enemy.stateMachine.ChangeState(new MoveState(enemy));
    }

    public void Exit() { }
}
