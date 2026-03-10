using UnityEngine;
using System;
using System.Numerics;
using System.Collections;
using Vector3 = UnityEngine.Vector3;

public class EnemyBase : MonoBehaviour
{
    [Header("Base Data")]
    public EnemyDataSO data;

    [Header("Current Stats")]
    public BigInteger maxHp;
    public BigInteger hp;
    public float attackPower;
    public float moveSpeed;
    public float attackRange;
    public float attackCooldown;

    private bool isDead;
    private bool isAttacking;
    private float lastAttackTime;

    private Animator animator;
    private EnemyManager enemyManager;
    private Transform target;

    public event Action<EnemyBase> OnEnemyDead;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }

    public void Init(int stage, EnemyManager Manager)
    { 
        this.enemyManager = Manager;
        this.target=Manager.GetPlayerTransform();

        maxHp = (BigInteger)(data.maxHp + stage * 5f);
        hp = maxHp;
        attackPower = data.attackPower + stage * 1.2f;
        moveSpeed = data.moveSpeed + stage * 0.05f;
        attackRange = data.attackRange;
        attackCooldown = Mathf.Max(0.5f, data.attackCooldown - stage * 0.02f);

        isDead = false;
        isAttacking = false;
        lastAttackTime = 0f;

        animator.SetBool("IsMoving",false);
        animator.SetBool("IsAttacking",false);
        animator.Play("Idle", 0, 0f);
    }

    private void Update()
    {
        if(isDead) return;

        Vector3 diff=target.position-transform.position;
        float sqrDist=diff.sqrMagnitude;

        if (sqrDist < attackRange * attackRange)
        {
            animator.SetBool("IsMoving", false);

            if (!isAttacking && Time.time - lastAttackTime >= attackCooldown)
            {
                Attack();
            }
        }
        else 
        {
            if (isAttacking) return;

            animator.SetBool("IsMoving", true);
            Vector3 dir=diff.normalized;

            dir.y = 0;
            transform.position += dir * moveSpeed * Time.deltaTime;
        }
    }
    private void Attack()
    {
        isAttacking = true;
        lastAttackTime = Time.time;

        animator.SetBool("IsAttacking", true);
        animator.SetTrigger("Attack");

        PlayerHP playerHp = target.GetComponent<PlayerHP>();
        if (playerHp != null)
        {
            playerHp.TakeDamage((BigInteger)attackPower);
        }
    }

    public void OnAttackEnd()
    {
        isAttacking = false;
        animator.SetBool("IsAttacking", false);
    }

    public void TakeDamage(BigInteger damage)
    {
        if (isDead) return;

        hp -= damage;
        if (hp < 0) hp = 0; // 마이너스 방지

        if (DamageTextManager.Instance != null)
            DamageTextManager.Instance.ShowDamage(damage, transform.position);

        if (hp <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;
        animator.SetTrigger("Die");

        //보상 지급 로직
        if (DataManager.Instance != null)
        {
            int currentStage = DataManager.Instance.currentStageNum;
            int dropGold = 10 + (currentStage * 5);
            DataManager.Instance.AddGold(dropGold);
        }

        if (QuestManager.Instance != null)
        {
            QuestManager.Instance.AddQuestProgress(QuestGoalType.KillMonster, 1);
        }

        StartCoroutine(DieRoutine());
    }

    private IEnumerator DieRoutine()
    {
        yield return new WaitForSeconds(1f);
        ForceDisappear();
    }

    public void ForceDisappear()
    {
        OnEnemyDead?.Invoke(this);
        gameObject.SetActive(false);
    }
}
