using UnityEngine;

public class GoldEnemyAttack : MonoBehaviour
{
    private Collider attackCollider;
    private bool hasHit;

    [SerializeField] private float attackDuration = 0.1f;
    [SerializeField] private int damage = 5;

    void Awake()
    {
        attackCollider = GetComponent<Collider>();
        attackCollider.enabled = false;
    }

    public void DoAttack()
    {
        hasHit = false;
        attackCollider.enabled = true;
        Invoke(nameof(EndAttack), attackDuration);
    }

    void EndAttack()
    {
        attackCollider.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasHit) return;

        if (other.CompareTag("Player"))
        {
            hasHit = true;
            Debug.Log($"플레이어 피격! 데미지 {damage}");

            // PlayerGoldDungeonHP.Instance.TakeDamage(damage);
        }
    }
}
