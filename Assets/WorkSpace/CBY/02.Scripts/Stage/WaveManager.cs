using UnityEngine;
using System;

public class WaveManager : MonoBehaviour
{
    public EnemySpawner enemySpawner;
    public EnemySpawner bossSpawner;

    private int aliveEnemyCount;
    private Action onWaveCleared;

    public void StartWave(WaveData data, Action clearCallback)
    {
        onWaveCleared = clearCallback;
        aliveEnemyCount = 0;

        SpawnWave(data);
    }

    private void SpawnWave(WaveData data)
    {
        // 일반 몬스터
        for (int i = 0; i < data.enemyCount; i++)
        {
            aliveEnemyCount++;
            enemySpawner.Spawn(OnEnemyDead);
        }

        // 5웨이브 전용 보스
        if (data.spawnBoss)
        {
            aliveEnemyCount++;
            bossSpawner.Spawn(OnEnemyDead);
        }
    }

    private void OnEnemyDead()
    {
        aliveEnemyCount--;

        if (aliveEnemyCount <= 0)
        {
            onWaveCleared?.Invoke();
        }
    }
}
