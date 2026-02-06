using UnityEngine;

[CreateAssetMenu(menuName = "Boss/Boss Stat")]
public class BossStatSO : ScriptableObject
{
<<<<<<< feat/CBY
    // 보스 기본 스탯
=======
    //기본 스탯
>>>>>>> dev
    [Header("Base Stats")]
    public float baseHp = 3000f;
    public float baseAttack = 50f;
    public float baseMoveSpeed = 0.8f;

<<<<<<< feat/CBY
    // 보스 전투 관련 스탯
=======
    // 전투 관련 스탯
>>>>>>> dev
    [Header("Combat")]
    public float attackRange = 2.5f;
    public float attackCooldown = 2.5f;

<<<<<<< feat/CBY
    // 보스 스테이지 당 증가 스탯
=======
    // 스테이지마다 증가 스탯
>>>>>>> dev
    [Header("Stage Scaling")]
    public float hpPerStage = 300f;
    public float attackPerStage = 5f;
}
