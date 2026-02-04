using UnityEngine;

// 스테이지 흐름만 관리
public class StageManager : MonoBehaviour
{
    public StageData stageData;      // ? 인스펙터에서 설정
    public WaveManager waveManager;

    int currentWaveIndex;

    void Start()
    {
        StartStage();
    }

    void StartStage()
    {
        currentWaveIndex = 0;
        StartNextWave();
    }

    void StartNextWave()
    {
        if (currentWaveIndex >= stageData.waves.Length)
        {
            ClearStage();
            return;
        }

        WaveData wave = stageData.waves[currentWaveIndex];

        waveManager.StartWave(
            wave,
            OnWaveCleared
        );
    }

    void OnWaveCleared()
    {
        currentWaveIndex++;
        StartNextWave();
    }

    void ClearStage()
    {
        Debug.Log($"{stageData.stageName} 클리어!");
        // 보상 / 다음 스테이지
    }
}
