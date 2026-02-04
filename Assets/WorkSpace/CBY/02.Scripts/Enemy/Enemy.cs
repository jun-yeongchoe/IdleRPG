using UnityEngine;

public class Enemy : MonoBehaviour
{
    public EnemyStats stats;
    public EnemyFSM fsm;

    private void Awake()
    {
        stats = GetComponent<EnemyStats>();
        fsm = GetComponent<EnemyFSM>();
    }

    private void OnEnable()
    {
        stats.InitByStage();
        fsm.ChangeState(EnemyStateType.Move);
    }

    public void TakeDamage(float damage)
    {
        stats.hp -= damage;

        if (stats.hp <= 0)
        {
            fsm.ChangeState(EnemyStateType.Die);
        }
    }
}