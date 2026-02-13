using UnityEngine;

public class EnemyDieState : IEnemyState
{
    private EnemyFSM fsm;

    public EnemyDieState(EnemyFSM fsm)
    {
        this.fsm = fsm;
    }

    public void Enter()
    {
        // Die 애니메이션 실행
        fsm.enemy.Die();

        // 이동/공격 애니 초기화
        fsm.enemy.MoveStop();
        fsm.enemy.OnAttackEnd();
    }

    public void Update()
    {
        // Die 상태에서는 별도 행동 없음
        // 필요시 추가 로직(사망 효과, 아이템 드롭 등) 가능
    }

    public void Exit()
    {
        // Die 상태 종료 시
        // Enemy.OnDieEnd()가 Animation Event에서 호출되면 자동 비활성화
        // 혹은 FSM Exit에서 강제로 처리 가능
    }
}