using UnityEngine;

public class Enemy : MonoBehaviour
{
    public EnemyStats stats;
    public EnemyFSM fsm;
    public EnemyManager enemyManager;

    private void Awake()
    {
        stats = GetComponent<EnemyStats>();
        fsm = GetComponent<EnemyFSM>();
        enemyManager = GetComponentInParent<EnemyManager>();
    }

    private void OnEnable()
    {
        stats.InitByStage();
        fsm.Init(this, enemyManager);
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

    public void Attack()
    {
        if (enemyManager == null) return;

        Transform player = enemyManager.GetPlayerTransform();
        if (player == null) return;

        Debug.Log("플레이어에게 데미지!");
        // player.GetComponent<PlayerHealth>().TakeDamage(stats.attackPower);
    }

    public void Die()
    {
        gameObject.SetActive(false);
    }
}