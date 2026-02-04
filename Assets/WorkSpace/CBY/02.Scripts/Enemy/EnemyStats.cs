using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    public float maxHp;
    public float hp;
    public float attack;
    public float moveSpeed;
    public float attackRange;
    public float attackCooldown;

    public void InitByStage()
    {
        int stage = DataManager.Instance.currentStageNum;

        maxHp = 10 + stage * 5;
        attack = 2 + stage * 1.2f;
        moveSpeed = 1.5f + stage * 0.05f;
        attackCooldown = Mathf.Max(0.5f, 1.5f - stage * 0.02f);

        hp = maxHp;
    }
}