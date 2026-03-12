using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
/// <summary>
/// 보스러시에도 enemymanager 필수임
/// </summary>
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

        int spawnIndex = 0;

        if (infiniteMode)
        {
            spawnIndex = Random.Range(0, bossPrefabs.Length);
        }
        else
        {
            if (currentIndex >= bossPrefabs.Length)
            {
                ClearBossRush();
                return;
            }
            spawnIndex = currentIndex;
        }

        GameObject bossObj = Instantiate(bossPrefabs[spawnIndex], spawnPoint.position, Quaternion.identity, transform);

        //EnemyBase의 OnEnable에서 어짜피 스텟 알아서 초기화해줌
        EnemyBase bossEnemy = bossObj.GetComponent<EnemyBase>();
        if (bossEnemy != null)
        {
            bossEnemy.OnEnemyDead += OnBossDead;
        }

        currentIndex++;
        BossRushUI.Instance.UpdateBossInfo(currentIndex, bossPrefabs.Length);
    }

    private void OnBossDead(EnemyBase deadBoss)
    {
        deadBoss.OnEnemyDead -= OnBossDead;

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

        int rewardGem = 500 + (defeatedCount * 10);

        if (DataManager.Instance != null)
        { 
            DataManager.Instance.AddGem(rewardGem);
        }

        if (CommonPopup.Instance != null)
        {
            CommonPopup.Instance.ShowAlert("보스 클리어!", $"보스를 정복했습니다!\n획득 보석: {rewardGem}개", "확인", () =>
            {
                BossRushUI.Instance.ShowClear();
                SceneManager.LoadScene("Game Scene_1st");
            });
        }
        else 
        {
            BossRushUI.Instance.ShowClear();
        }
    }
}