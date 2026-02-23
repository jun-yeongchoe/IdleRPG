using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    Animator anim;
    private float lastAttackTime = 0f;
    private Enemy currentTarget; // 현재 락온된 타겟

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    // 컨트롤러에서 대상을 지정해 공격 호출
    public void Attack(Enemy target)
    {
        currentTarget = target;

        float attackInterval = 1f / PlayerStat.instance.atkSpeed;
        
        if (Time.time - lastAttackTime > attackInterval)
        {
            anim.SetTrigger("swing");
            lastAttackTime = Time.time;
            OnAttackHit();
        }
    }

    public void OnAttackHit()
    {
        if (currentTarget == null) return;

        var attackData = PlayerStat.instance.GetAttackDamage();
        
        // 공격 전 체력
        float hpBefore = currentTarget.stats.hp;
        
        currentTarget.TakeDamage(attackData.damage);
        
        // 공격 후 체력
        float hpAfter = currentTarget.stats.hp;

        Debug.Log($"<color=cyan>[공격 성공]</color> {currentTarget.name} | 데미지: {attackData.damage} | 체력 변화: {hpBefore} -> {hpAfter}");
    }
}