using UnityEngine;

public class EnemyDieState : IEnemyState
{
    EnemyFSM fsm;

    public EnemyDieState(EnemyFSM fsm)
    {
        this.fsm = fsm;
    }

    public void Enter()
    {
        //if (DataManager.Instance != null)
        //{ 
        //    DataManager.Instance.AddGold();
        //}
        fsm.gameObject.SetActive(false);
    }

    public void Update() { }
    public void Exit() { }
}
