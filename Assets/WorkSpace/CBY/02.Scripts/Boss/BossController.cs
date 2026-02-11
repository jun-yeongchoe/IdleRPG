using UnityEngine;
using System.Collections;

public class BossController : MonoBehaviour
{
    public BossStats stats; // 보스 스탯 참조

    [Header("아이템 드롭")]
    public GameObject[] dropItems;
    [Range(0f, 1f)]
    public float dropChance = 1.0f;
    public int dropCount = 1;

    private bool isActing;
    private bool isDead;

    // ★ 추가: 보스 사망 콜백 (보스러쉬용)
    public System.Action onDead;

    private enum BossState
    {
        Idle,
        Move,
        Attack,
        Dead
    }

    private BossState currentState;

    private void Start()
    {
        // 스탯 초기화 (일반 스테이지용)
        stats.InitByStage();
        ChangeState(BossState.Idle);
    }

    // ★ 추가: 보스러쉬 매니저에서 호출
    public void Init(System.Action deadCallback)
    {
        onDead = deadCallback;
    }

    private void ChangeState(BossState newState)
    {
        if (isActing || isDead) return;

        currentState = newState;
        StartCoroutine(StateRoutine());
    }

    private IEnumerator StateRoutine()
    {
        isActing = true;

        switch (currentState)
        {
            case BossState.Idle:
                yield return Idle();
                break;

            case BossState.Move:
                yield return Move();
                break;

            case BossState.Attack:
                yield return Attack();
                break;
        }

        isActing = false;
        DecideNextState();
    }

    private IEnumerator Idle()
    {
        yield return new WaitForSeconds(0.8f);
    }

    private IEnumerator Move()
    {
        float moveTime = 1.5f;
        float timer = 0f;

        while (timer < moveTime)
        {
            transform.Translate(Vector3.left * stats.moveSpeed * Time.deltaTime);
            timer += Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator Attack()
    {
        Debug.Log($"보스 공격! 데미지: {stats.attack}");
        yield return new WaitForSeconds(stats.attackCooldown);
    }

    private void DecideNextState()
    {
        if (isDead) return;

        if (currentState == BossState.Idle)
            ChangeState(BossState.Move);
        else if (currentState == BossState.Move)
            ChangeState(BossState.Attack);
        else
            ChangeState(BossState.Idle);
    }

    // 외부에서 데미지 들어올 때 호출
    public void TakeDamage(float damage)
    {
        if (isDead) return;

        stats.TakeDamage(damage);

        if (stats.hp <= 0)
            Die();
    }

    private void Die()
    {
        isDead = true;
        currentState = BossState.Dead;

        DropItems();

        Debug.Log("보스 사망");

        onDead?.Invoke();   // ★ 추가: 보스러쉬 매니저에게 알림
        Destroy(gameObject);
    }

    private void DropItems()
    {
        if (dropItems.Length == 0) return;

        for (int i = 0; i < dropCount; i++)
        {
            if (Random.value > dropChance) continue;

            GameObject item = dropItems[Random.Range(0, dropItems.Length)];

            Vector3 dropPos = transform.position;
            dropPos.x += Random.Range(-0.5f, 0.5f);

            Instantiate(item, dropPos, Quaternion.identity);
        }
    }
}