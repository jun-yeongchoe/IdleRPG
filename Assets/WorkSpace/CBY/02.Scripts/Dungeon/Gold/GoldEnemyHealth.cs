using UnityEngine;

// 골드 던전 전용 적 체력 스크립트
public class GoldEnemyHealth : MonoBehaviour
{
    [Header("HP")]
    public int maxHP = 20;
    private int currentHP;

    [Header("Reward")]
    public int rewardGold = 10;

    private void OnEnable()
    {
        currentHP = maxHP;
    }

    public void TakeDamage(int damage)
    {
        currentHP -= damage;

        if (currentHP <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // 골드 던전 매니저에 골드 누적
        if (GoldDungeonManager.Instance != null)
        {
            GoldDungeonManager.Instance.AddGold(rewardGold);
        }
        else
        {
            Debug.LogWarning("GoldDungeonManager Instance가 없습니다!");
        }

        // 풀링용 비활성화
        gameObject.SetActive(false);
    }
}
