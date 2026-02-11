using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BossRushManager : MonoBehaviour
{
    [Header("보스 설정")]
    public GameObject[] bossPrefabs;
    public Transform spawnPoint;

    [Header("옵션")]
    public float spawnDelay = 2f;
    public bool infiniteMode = false;

    private int currentIndex = 0;
    private int defeatedCount = 0;

    private BossController currentBoss;

    private void Start()
    {
        StartBossRush();
    }

    public void StartBossRush()
    {
        currentIndex = 0;
        defeatedCount = 0;
        SpawnNextBoss();
    }

    private void SpawnNextBoss()
    {
        GameObject bossObj;

        if (infiniteMode)
        {
            bossObj = Instantiate(
                bossPrefabs[Random.Range(0, bossPrefabs.Length)],
                spawnPoint.position,
                Quaternion.identity
            );
        }
        else
        {
            if (currentIndex >= bossPrefabs.Length)
            {
                ClearBossRush();
                return;
            }

            bossObj = Instantiate(
                bossPrefabs[currentIndex],
                spawnPoint.position,
                Quaternion.identity
            );
        }

        currentBoss = bossObj.GetComponent<BossController>();
        currentBoss.Init(OnBossDead);

        currentIndex++;
        BossRushUI.Instance.UpdateBossInfo(currentIndex, bossPrefabs.Length);
    }

    private void OnBossDead()
    {
        defeatedCount++;
        BossRushUI.Instance.UpdateDefeated(defeatedCount);

        StartCoroutine(NextBossDelay());
    }

    private IEnumerator NextBossDelay()
    {
        yield return new WaitForSeconds(spawnDelay);
        SpawnNextBoss();
    }

    private void ClearBossRush()
    {
        Debug.Log("보스러쉬 클리어!");
        BossRushUI.Instance.ShowClear();
    }
}
