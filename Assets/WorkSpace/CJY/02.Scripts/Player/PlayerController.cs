using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private PlayerMove playerMove;
    private PlayerAttack playerAttack;

    public float attackRange = 1.5f;
    public bool isCombat = false;
    
    [SerializeField]private GameObject enemyManager;
    // 현재 추적 중인 적 리스트
    private List<EnemyBase> targetEnemies = new List<EnemyBase>();
    public GameObject[] partnerSlot;

    private void Awake()
    {
        playerMove = GetComponent<PlayerMove>();
        playerAttack = GetComponent<PlayerAttack>();
        enemyManager = GameObject.Find("EnemyManager");
    }

    void Update()
    {
        CheckEnemyDistance();
    }

    void CheckEnemyDistance()
    {
        if (enemyManager == null) return;
        targetEnemies.RemoveAll(e => e == null || !e.gameObject.activeInHierarchy || e.hp <= 0);
        // 1. 리스트가 비어있으면 갱신
        if (targetEnemies.Count == 0)
        {
            RefreshEnemyList();
            if (targetEnemies.Count == 0) { SetMoveState(); return; }
        }

        // 2. 리스트의 첫 번째 적 상태 확인
        EnemyBase firstEnemy = targetEnemies[0];

        // 객체가 파괴되면 리스트에서 제거
        if (firstEnemy == null || firstEnemy.hp<=0 || !firstEnemy.gameObject.activeInHierarchy) 
        {
            targetEnemies.RemoveAt(0);
            return; 
        }

        // 3. 살아있다면 거리 체크
        float dist = firstEnemy.transform.position.x - transform.position.x;
        if (dist > 0 && dist <= attackRange)
        {
            isCombat = true;
            playerMove.Stop();
            GetComponent<ActiveSkill>().SetTarget(firstEnemy);
            playerAttack.Attack(firstEnemy);
            
        }
        else
        {
            SetMoveState();
        }
    }

    // 계층 구조를 뒤져서 리스트를 채우는 무거운 작업 (필요할 때만 호출)
    void RefreshEnemyList()
    {
        foreach (Transform spawner in enemyManager.transform)
        {
            foreach (Transform monster in spawner)
            {
                EnemyBase enemy = monster.GetComponent<EnemyBase>();
                if (enemy != null && !targetEnemies.Contains(enemy))
                {
                    targetEnemies.Add(enemy);
                }
            }
        }
    }

    void SetMoveState()
    {
        isCombat = false;
        playerMove.Move();
    }

    public EnemyBase GetCurrentTarget()
    {
        if(targetEnemies != null && targetEnemies.Count > 0)
        {
            return targetEnemies[0];
        }
        return null;
    }
}