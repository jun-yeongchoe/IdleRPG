using UnityEngine;

// 에너미가 공격을 맞았을 때 상태
public class HitState : IEnemyState
{
    Enemy enemy;
    Vector2 hitDirection;

    // 피격당하고 경직 유지 시간
    float hitDuration = 0.2f;
    float timer;

    public HitState(Enemy enemy, Vector2 hitDir)
    {
        this.enemy = enemy;
        this.hitDirection = hitDir;
    }

    public void Enter()
    {
        timer = hitDuration;

        // 피격 애니메이션
        enemy.animator.SetTrigger("Hit");

        // 넉백 실행
        enemy.Knockback(hitDirection);
    }

    public void Update()
    {
        timer -= Time.deltaTime;

        // 경직 시간 종료 → 다시 행동
        if (timer <= 0f)
        {
            enemy.stateMachine.ChangeState(new MoveState(enemy));
        }
    }

    public void Exit()
    {
        // 넉백 후 속도 초기화
        enemy.rb.velocity = Vector2.zero;
    }
}
