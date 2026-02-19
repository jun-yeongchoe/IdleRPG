using UnityEngine;
using System;

public class BossFSM : MonoBehaviour
{
    enum State { Idle, Move, Attack, Die }

    [Header("References")]
    public BossStats stats;

    //보스 사망 콜백
    public Action OnBossDeadCallback;

    private Transform player;
    private Rigidbody2D rb;
    private Animator anim;

    private State currentState;
    private float attackTimer;
    private bool isDead = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    private void Start()
    {
        stats.InitByStage();

        EnemyManager manager = FindObjectOfType<EnemyManager>();
        if (manager != null)
            player = manager.GetPlayerTransform();

        ChangeState(State.Idle);
    }

    private void Update()
    {
        if (isDead || player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);
        attackTimer += Time.deltaTime;

        if (distance <= stats.attackRange)
        {
            if (attackTimer >= stats.attackCooldown)
                ChangeState(State.Attack);
        }
        else
        {
            ChangeState(State.Move);
        }
    }

    private void FixedUpdate()
    {
        if (currentState == State.Move)
            MoveToPlayer();
    }

    void MoveToPlayer()
    {
        Vector2 dir = (player.position - transform.position).normalized;
        rb.velocity = dir * stats.moveSpeed;
        anim.SetBool("Move", true);
    }

    void PerformRaycastAttack()
    {
        Vector2 dir = (player.position - transform.position).normalized;

        RaycastHit2D hit = Physics2D.Raycast(
            transform.position,
            dir,
            stats.attackRange,
            LayerMask.GetMask("Player")
        );

        if (hit.collider != null)
        {
            hit.collider.SendMessage(
                "TakeDamage",
                stats.attackDamage,
                SendMessageOptions.DontRequireReceiver
            );
        }
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        stats.TakeDamage(damage);

        if (stats.currentHp <= 0)
            Die();
    }

    void Die()
    {
        isDead = true;
        rb.velocity = Vector2.zero;
        ChangeState(State.Die);

        //BossRushManager에 사망 알림
        OnBossDeadCallback?.Invoke();
    }

    void ChangeState(State newState)
    {
        if (currentState == newState) return;

        currentState = newState;
        anim.SetBool("Move", false);

        switch (newState)
        {
            case State.Idle:
                rb.velocity = Vector2.zero;
                anim.Play("Idle");
                break;

            case State.Move:
                anim.SetBool("Move", true);
                break;

            case State.Attack:
                rb.velocity = Vector2.zero;
                anim.SetTrigger("Attack");
                attackTimer = 0f;
                break;

            case State.Die:
                anim.SetTrigger("Die");
                break;
        }
    }

    // 애니메이션 이벤트
    public void OnAttackFrame()
    {
        PerformRaycastAttack();
    }

    public void OnAttackEnd()
    {
        if (!isDead)
            ChangeState(State.Idle);
    }

    public void OnDieEnd()
    {
        gameObject.SetActive(false);
    }
}