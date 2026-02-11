using System.Collections;
using UnityEngine;

//골드 던전에서 일정 간격마다 적을 스폰하는 스크립트
public class GoldEnemySpawner : MonoBehaviour
{
    [Header("Spawn")]
    public GameObject enemyPrefab;
    public int poolSize = 5;
    private GameObject[] enemyPool;

    [Header("Attack")]
    public float attackInterval = 2f;
    private Coroutine attackRoutine;

    private void Awake()
    {
        enemyPool = new GameObject[poolSize];
        for (int i = 0; i < poolSize; i++)
        {
            enemyPool[i] = Instantiate(enemyPrefab, transform);
            enemyPool[i].SetActive(false);
        }
    }

    private void OnEnable()
    {
        SpawnAll();
        attackRoutine = StartCoroutine(AttackLoop());
    }

    private void OnDisable()
    {
        if (attackRoutine != null)
            StopCoroutine(attackRoutine);
    }

    void SpawnAll()
    {
        foreach (var enemy in enemyPool)
        {
            enemy.SetActive(true);
        }
    }

    IEnumerator AttackLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(attackInterval);

            foreach (var enemy in enemyPool)
            {
                if (!enemy.activeSelf) continue;

                GoldEnemyAttack attack =
                    enemy.GetComponentInChildren<GoldEnemyAttack>();

                if (attack != null)
                    attack.DoAttack();
            }
        }
    }
}
