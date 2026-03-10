using UnityEngine;
using System.Collections.Generic;

public class GoldDungeonEnemySpawner : MonoBehaviour
{
    [Header("스폰 설정")]
    public GameObject enemyPrefab;
    public int spawnCount = 10;
    public float spawnRadius = 2f;

    private readonly List<EnemyBase> aliveEnemies = new();

    private void Start()
    {
        SpawnAll();
    }

    void SpawnAll()
    {
        aliveEnemies.Clear();

        for (int i = 0; i < spawnCount; i++)
        {
            Vector2 randomPos = Random.insideUnitCircle * spawnRadius;
            GameObject go = Instantiate(
                enemyPrefab,
                transform.position + new Vector3(randomPos.x, randomPos.y, 0),
                Quaternion.identity,
                transform
            );

            EnemyBase enemy = go.GetComponent<EnemyBase>();
            if (enemy == null)
            {
                Debug.LogError("Enemy 컴포넌트가 없습니다!");
                continue;
            }

            aliveEnemies.Add(enemy);
            enemy.OnEnemyDead += HandleEnemyDead; //핵심 연결
        }
    }

    void HandleEnemyDead(EnemyBase enemy)
    {
        enemy.OnEnemyDead -= HandleEnemyDead;
        aliveEnemies.Remove(enemy);

        if (GoldDungeonManager.Instance != null)
        {
            GoldDungeonManager.Instance.OnEnemyKilled();
        }

        if (aliveEnemies.Count == 0)
        {
            GoldDungeonManager.Instance.ClearDungeon();
        }
    }
}