using UnityEngine;
using System.Collections;

public class BossController : MonoBehaviour
{
    public BossStat stat;

    [Header("아이템 드롭")]
    public GameObject[] dropItems;   // 드롭 가능한 아이템 프리팹
    [Range(0f, 1f)]
    public float dropChance = 1.0f;  // 1 = 무조건 드롭
    public int dropCount = 1;

    private float currentHP;
    private bool isActing;
    private bool isDead;

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
        currentHP = stat.maxHP;
        ChangeState(BossState.Idle);
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
            transform.Translate(Vector3.left * stat.moveSpeed * Time.deltaTime);
            timer += Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator Attack()
    {
        Debug.Log("보스 공격! 데미지: " + stat.attackDamage);
        yield return new WaitForSeconds(stat.attackCooldown);
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

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHP -= damage;

        if (currentHP <= 0)
        {
            currentHP = 0;
            Die();
        }
    }

    private void Die()
    {
        isDead = true;
        currentState = BossState.Dead;

        DropItems();

        Debug.Log("보스 사망");

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

