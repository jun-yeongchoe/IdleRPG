using UnityEngine;

public class BossFSM : MonoBehaviour
{
    enum State { Idle, Move, Attack, Die }

    [Header("References")]
    public BossStats stats;

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
        // 플레이어 참조
        EnemyManager manager = FindObjectOfType<EnemyManager>();
        if (manager != null)
            player = manager.GetPlayerTransform();

        //사망 이벤트 구독
        stats.OnDeath += OnDead;

        ChangeState(State.Idle);
    }

    private void Update()
    {
        if (isDead || player == null) return;

        attackTimer += Time.deltaTime;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= stats.attackRange)
        {
            if (attackTimer >= stats.attackCooldown)
            {
                ChangeState(State.Attack);
            }
        }
        else
        {
            ChangeState(State.Move);
        }
    }

    private void FixedUpdate()
    {
        if (isDead) return;

        if (currentState == State.Move)
            MoveToPlayer();
    }

    void MoveToPlayer()
    {
        Vector2 dir = (player.position - transform.position).normalized;
        rb.velocity = dir * stats.moveSpeed;
        anim.SetBool("Move", true);
    }

    //애니메이션 이벤트에서 호출
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
                stats.AttackDamage,
                SendMessageOptions.DontRequireReceiver
            );
        }
    }

    //BossStats가 사망 판단 → FSM은 반응만
    void OnDead()
    {
        if (isDead) return;

        isDead = true;
        rb.velocity = Vector2.zero;
        ChangeState(State.Die);
    }

    void ChangeState(State newState)
    {
        if (currentState == newState) return;

        currentState = newState;

        // 공통 초기화
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

    // ===== 애니메이션 이벤트 =====

    // 공격 판정 프레임
    public void OnAttackFrame()
    {
        if (!isDead)
            PerformRaycastAttack();
    }

    // 공격 종료
    public void OnAttackEnd()
    {
        if (!isDead)
            ChangeState(State.Idle);
    }

    // 사망 애니메이션 종료
    public void OnDieEnd()
    {
        gameObject.SetActive(false);
    }
}