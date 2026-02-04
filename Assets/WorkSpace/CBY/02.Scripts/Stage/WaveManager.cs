using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaveManager : MonoBehaviour
{
    [Header("Spawner")]
    public EnemySpawner spawner;

    // 현재 살아있는 적 목록
    private List<Enemy> aliveEnemies = new List<Enemy>();

    // 웨이브 클리어 콜백
    private System.Action onWaveCleared;

    /// <summary>
    /// 외부(StageManager 등)에서 웨이브 시작
    /// </summary>
    public void StartWave(WaveData data, System.Action clearCallback)
    {
        onWaveCleared = clearCallback;

        StopAllCoroutines();          // 혹시 남아있는 웨이브 정리
        StartCoroutine(SpawnWave(data));
    }

    /// <summary>
    /// 웨이브 적 스폰 코루틴
    /// </summary>
    IEnumerator SpawnWave(WaveData data)
    {
        aliveEnemies.Clear();

        for (int i = 0; i < data.enemyCount; i++)
        {
            Enemy enemy = spawner.Spawn(
                data.enemyPrefab,
                data.isBossWave
            );

            // ⭐ Enemy 구조에 맞게 수정된 부분
            enemy.SetWaveManager(this);

            aliveEnemies.Add(enemy);

            yield return new WaitForSeconds(data.spawnDelay);
        }
    }

    /// <summary>
    /// Enemy 사망 시 호출됨
    /// </summary>
    public void OnEnemyDead(Enemy enemy)
    {
        if (!aliveEnemies.Contains(enemy))
            return;

        aliveEnemies.Remove(enemy);

        // 웨이브 종료 조건
        if (aliveEnemies.Count == 0)
        {
            onWaveCleared?.Invoke();
        }
    }
}
