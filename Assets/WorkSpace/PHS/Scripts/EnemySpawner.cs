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

    private GameObject[] enemyPool;

    private bool isWaiting=false;

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
        for (int i = 0; i < poolSize; i++)
        {
            if (!enemyPool[i].activeSelf)
            {
                Spawn();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
       if(isWaiting) return;

        int activeCount = 0;
        for (int i = 0; i < poolSize; i++) 
        {
            if (enemyPool[i].activeSelf) activeCount++;
        }

        if (activeCount ==0)
        {
            StartCoroutine(NextStage());
        }
    }
    private IEnumerator NextStage()
    {
        isWaiting = true;

        if (DataManager.Instance != null)
        { 
            DataManager.Instance.currentStageNum++;
        }

        yield return new WaitForSeconds(respawnInterval);

        SpawnWave();

        isWaiting = false;
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

        //스테이지 맞춰서 스텟 초기화해주는거 여기 추가하기

        selectEnemy.SetActive(true);
    }
}
