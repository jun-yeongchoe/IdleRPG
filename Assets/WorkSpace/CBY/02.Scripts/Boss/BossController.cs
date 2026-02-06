using UnityEngine;
using System.Collections;

public class BossController : MonoBehaviour
{
<<<<<<< feat/CBY
    public BossStats stats; // ���� ���� ����

    [Header("������ ���")]
=======
    public BossStats stats; // 보스 스탯 참조

    [Header("아이템 드롭")]
>>>>>>> dev
    public GameObject[] dropItems;
    [Range(0f, 1f)]
    public float dropChance = 1.0f;
    public int dropCount = 1;

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
<<<<<<< feat/CBY
        // ���� �ʱ�ȭ (�������� ����)
=======
        // 스탯 초기화 (스테이지 기준)
>>>>>>> dev
        stats.InitByStage();

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
<<<<<<< feat/CBY
        // ��� ���
=======
        // 잠깐 대기
>>>>>>> dev
        yield return new WaitForSeconds(0.8f);
    }

    private IEnumerator Move()
    {
        float moveTime = 1.5f;
        float timer = 0f;

        while (timer < moveTime)
        {
<<<<<<< feat/CBY
            // BossStats�� �̵��ӵ� ���
=======
            // BossStats의 이동속도 사용
>>>>>>> dev
            transform.Translate(Vector3.left * stats.moveSpeed * Time.deltaTime);
            timer += Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator Attack()
    {
<<<<<<< feat/CBY
        // ���� ���� ������ BossAttack ������ �и� ����
        Debug.Log($"���� ����! ������: {stats.attack}");
=======
        // 실제 공격 판정은 BossAttack 쪽으로 분리 가능
        Debug.Log($"보스 공격! 데미지: {stats.attack}");
>>>>>>> dev

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

<<<<<<< feat/CBY
    // �ܺο��� ������ ���� �� ȣ��
=======
    // 외부에서 데미지 들어올 때 호출
>>>>>>> dev
    public void TakeDamage(float damage)
    {
        if (isDead) return;

        stats.TakeDamage(damage);

<<<<<<< feat/CBY
        // BossStats�� HP�� �������� ��� �Ǵ�
=======
        // BossStats의 HP를 기준으로 사망 판단
>>>>>>> dev
        if (stats.hp <= 0)
            Die();
    }

    private void Die()
    {
        isDead = true;
        currentState = BossState.Dead;

        DropItems();

        Debug.Log("���� ���");

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