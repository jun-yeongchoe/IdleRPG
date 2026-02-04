public class DeadState : IEnemyState
{
    private Enemy enemy;

    public DeadState(Enemy enemy)
    {
        this.enemy = enemy;
    }

    public void Enter()
    {
        enemy.animator.SetTrigger("Die");
    }

    public void Update()
    {
        // 아무것도 안 함
    }

    public void Exit() { }

    // 애니메이션 이벤트에서 호출
    public void OnDieAnimationEnd()
    {
        enemy.Die();
    }
}