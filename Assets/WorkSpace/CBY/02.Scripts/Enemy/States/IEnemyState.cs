using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 모든 적 상태가 반드시 구현해야 하는 규칙 정의
public interface IEnemyState
{
    // 상태에 진입할 때 실행되는 코드
    void Enter();

    // 상태가 활성화되는 동안 매 프레임 실행되는 코드
    void Update();

    // 상태에서 벗어날 때 실행되는 코드
    void Exit();
}
