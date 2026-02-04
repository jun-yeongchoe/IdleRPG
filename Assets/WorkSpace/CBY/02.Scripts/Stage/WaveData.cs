using UnityEngine;

[System.Serializable]
public class WaveData
{
    [Header("Wave Info")]
    public bool isBossWave;          // 보스 웨이브 여부

    [Header("Spawn")]
    public GameObject enemyPrefab;   // 스폰할 적
    public int enemyCount = 5;       // 적 수
    public float spawnDelay = 0.5f;  // 스폰 간격
}
