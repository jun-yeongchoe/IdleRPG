using UnityEngine;

// 실제 적 오브젝트 핵심 클래스
public class Enemy : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Stats")]
    public int maxHp = 10;
    public int hp;
    public float moveSpeed = 2f;
    public float attackRange = 1.5f;

    [Header("Hit & Knockback")]
    public float knockbackForce = 3f;

    [Header("Components")]
    public Animator animator;
    public Rigidbody2D rb;

    // FSM
    public EnemyStateMachine stateMachine;

    // ? 웨이브 매니저 참조 (추가)
    private WaveManager waveManager;

    void Awake()
    {
        stateMachine = new EnemyStateMachine();
        hp = maxHp;

        // 필수 컴포넌트 자동 연결
        if (!rb) rb = GetComponent<Rigidbody2D>();
        if (!animator) animator = GetComponent<Animator>();
    }

    void Start()
    {
        // 시작 상태
        stateMachine.ChangeState(new IdleState(this));
    }

    void Update()
    {
        stateMachine.Update();
    }

    // =========================
    // WaveManager 연동
    // =========================

    /// <summary>
    /// WaveManager가 스폰 직후 호출
    /// </summary>
    public void SetWaveManager(WaveManager manager)
    {
        waveManager = manager;
    }

    // =========================
    // 데미지 처리
    // =========================

    public void TakeDamage(int damage, Vector2 hitDir)
    {
        if (hp <= 0) return;

        hp -= damage;

        if (hp <= 0)
        {
            stateMachine.ChangeState(new DeadState(this));
        }
        else
        {
            stateMachine.ChangeState(new HitState(this, hitDir));
        }
    }

    /// <summary>
    /// 넉백 처리
    /// </summary>
    public void Knockback(Vector2 dir)
    {
        rb.velocity = Vector2.zero;
        rb.AddForce(dir.normalized * knockbackForce, ForceMode2D.Impulse);
    }

    // =========================
    // 사망 확정 처리
    // =========================

    /// <summary>
    /// DeadState에서 호출
    /// </summary>
    public void Die()
    {
        // 웨이브 매니저에게 사망 알림
        waveManager?.OnEnemyDead(this);

        Destroy(gameObject);
    }
}