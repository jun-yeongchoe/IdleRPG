using UnityEngine;

public class EnemyAttackState : IEnemyState
{
    EnemyFSM fsm;
    float lastAttackTime;

    public EnemyAttackState(EnemyFSM fsm)
    {
        this.fsm = fsm;
    }

    public void Enter()
    {
        lastAttackTime = Time.time;
    }

    public void Update()
    {
        if (Time.time - lastAttackTime >= fsm.enemy.stats.attackCooldown)
        {
            // 여기서 실제 데미지 처리
            Debug.Log("플레이어 공격!");

            lastAttackTime = Time.time;
        }

        float dist = Vector2.Distance(
            fsm.transform.position,
            GameObject.FindWithTag("Player").transform.position
        );

        if (dist > fsm.enemy.stats.attackRange)
        {
            fsm.ChangeState(EnemyStateType.Move);
        }
    }

    public void Exit() { }
}