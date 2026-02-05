using UnityEngine;
using System.Collections;

public class SimpleBossController : MonoBehaviour
{
    public BossStat stat;

    private float currentHP;
    private bool isActing;

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
        if (isActing || currentState == BossState.Dead) return;

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
        if (currentHP <= 0)
        {
            currentState = BossState.Dead;
            Debug.Log("보스 사망");
            return;
        }

        // 단순 루프 구조
        if (currentState == BossState.Idle)
            ChangeState(BossState.Move);
        else if (currentState == BossState.Move)
            ChangeState(BossState.Attack);
        else
            ChangeState(BossState.Idle);
    }

    public void TakeDamage(float damage)
    {
        currentHP -= damage;

        if (currentHP <= 0)
        {
            currentHP = 0;
            currentState = BossState.Dead;
            Debug.Log("보스 사망 처리");
        }
    }
}