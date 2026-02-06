using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("설정")]
    public GameObject enemyPrefab;
    public float respawnInterval = 1.0f;
    public float spawnRadius = 1.0f;
    public int poolSize = 5;

    [Header("보스 설정")]
    public GameObject bossPrefab;
    private GameObject activeBoss;

    private GameObject[] enemyPool;

    private bool isWaiting=false;

    private int currentWave = 0;
    private const int maxNormalWave = 5;

    private void Awake()
    {
        enemyPool=new GameObject[poolSize];

        for (int i = 0; i < poolSize; i++)
        {
            enemyPool[i]=Instantiate(enemyPrefab,transform);
            enemyPool[i].SetActive(false);
        }
    }

    private void OnEnable()
    {
        isWaiting = false;
        currentWave = 0;
        activeBoss = null;
        for (int i = 0; i < poolSize; i++)
        {
            if (!enemyPool[i].activeSelf) Spawn();
        }
    }

    // Update is called once per frame
    void Update()
    {
       if(isWaiting) return;

        if (activeBoss != null)
        {
            if (activeBoss.activeSelf) return;
            StartCoroutine(BossClearRoutine());
            activeBoss = null;
            return;
        }

        int activeCount = 0;
        for (int i = 0; i < poolSize; i++) 
        {
            if (enemyPool[i].activeSelf) activeCount++;
        }

        if (activeCount ==0)
        {
            StartCoroutine(CheckNextWave());
        }
    }
    private IEnumerator CheckNextWave()
    {
        isWaiting = true;
        yield return new WaitForSeconds(respawnInterval);

        currentWave++;

        if (currentWave < 5)
        {
            SpawnWave();
        }
        else
        {
            SpawnBoss();
        }

        isWaiting = false;
    }

    private IEnumerator BossClearRoutine()
    {
        isWaiting = true;

        if (DataManager.Instance != null)
        {
            DataManager.Instance.currentStageNum++;
        }

        yield return new WaitForSeconds(respawnInterval);

        currentWave = 0;
        SpawnWave();

        isWaiting = false;
    }

    void SpawnBoss()
    {
        if (bossPrefab != null)
        {
            activeBoss = Instantiate(bossPrefab, transform.position, Quaternion.identity);
            //보스 스텟 초기화도 여기서 작업 예정
        }
    }

    void SpawnWave()
    {
        for (int i = 0; i < poolSize; i++) 
        {
            if (!enemyPool[i].activeSelf) 
            {
                Spawn();
            }
        }
    }

    void Spawn()
    {
        GameObject selectEnemy = null;
        for (int i = 0; i < poolSize; i++) 
        {
            if (!enemyPool[i].activeSelf) 
            { 
                selectEnemy = enemyPool[i];
                break;
            }
        }

        if (selectEnemy == null) return;

        Vector2 randomPos = Random.insideUnitCircle * spawnRadius;
        selectEnemy.transform.position = transform.position+new Vector3(randomPos.x, randomPos.y,0);

        EnemyStats stat = selectEnemy.GetComponent<EnemyStats>();
        if (stat != null)
        {
            stat.InitByStage();
        }

        selectEnemy.SetActive(true);
    }
}
