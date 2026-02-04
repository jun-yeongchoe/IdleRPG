using UnityEngine;

public class MoveState : IEnemyState
{
    Enemy enemy;

    public MoveState(Enemy enemy)
    {
        this.enemy = enemy;
    }

    public void Enter()
    {
        Debug.Log("Move");
    }


    //플레이어 쪽으로 이동
    public void Update()
    {
        enemy.transform.position = Vector3.MoveTowards(
            enemy.transform.position,
            enemy.target.position,
            enemy.moveSpeed * Time.deltaTime
        );

        //플레이어와의 거리 계산
        float dist = Vector3.Distance(
            enemy.transform.position,
            enemy.target.position
        );

        //공격 범위 내에 들어오면 공격 상태로 전환
        if (dist <= enemy.attackRange)
            enemy.stateMachine.ChangeState(new AttackState(enemy));
    }

    public void Exit() { }
}
