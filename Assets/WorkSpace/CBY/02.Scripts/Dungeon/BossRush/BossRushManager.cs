using UnityEngine;
using System.Collections;

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
    private bool waitingNextBoss = false;

    private void Start()
    {
        StartBossRush();
    }

    public void StartBossRush()
    {
        currentIndex = 0;
        defeatedCount = 0;
        waitingNextBoss = false;
        SpawnNextBoss();
    }

    private void SpawnNextBoss()
    {
        if (bossPrefabs.Length == 0) return;

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

        //Stats 세팅
        BossStats stats = bossObj.GetComponent<BossStats>();
        if (stats != null)
        {
            int stage = infiniteMode ? defeatedCount + 1 : currentIndex + 1;
            stats.InitByStage(stage);
            stats.OnDeath += OnBossDead;
        }

        currentIndex++;
        BossRushUI.Instance.UpdateBossInfo(currentIndex, bossPrefabs.Length);
    }

    private void OnBossDead()
    {
        if (waitingNextBoss) return;

        waitingNextBoss = true;
        defeatedCount++;

        BossRushUI.Instance.UpdateDefeated(defeatedCount);
        StartCoroutine(NextBossDelay());
    }

    private IEnumerator NextBossDelay()
    {
        yield return new WaitForSeconds(spawnDelay);
        waitingNextBoss = false;
        SpawnNextBoss();
    }

    private void ClearBossRush()
    {
        Debug.Log("보스 러쉬 클리어!");
        BossRushUI.Instance.ShowClear();
    }
}