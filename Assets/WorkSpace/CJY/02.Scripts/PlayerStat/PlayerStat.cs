
using UnityEngine;


// 플레이어 스탯
// 추후 추가될 스킬에서도 atkPower를 참조해야함.

public class PlayerStat : MonoBehaviour
{
    public static PlayerStat instance{get; private set;}

    [SerializeField] PlayerStatus playerStatus;
    public float atkPower, hp, hpGen, atkSpeed, criticalChance, criticalDamage;

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        atkPower = playerStatus.GetAtkPower();
        hp = playerStatus.GetHP();
        hpGen = playerStatus.GetHPGen();
        atkSpeed = playerStatus.GetAtkSpeed();
        criticalChance = playerStatus.GetCriticalChance();
        criticalDamage = playerStatus.GetCriticalDamage();
    }


    // 타 클래스에서 호출하여 값 갱신
    public void SetAttackPower() => atkPower = playerStatus.GetAtkPower();
    public void SetHP() => hp = playerStatus.GetHP();
    public void SetHPGen() => hpGen = playerStatus.GetHPGen();
    public void SetAtkSpeed() => atkSpeed = playerStatus.GetAtkSpeed();
    public void SetCriticalChance() => criticalChance = playerStatus.GetCriticalChance();
    public void SetCriticalDamage() => criticalDamage = playerStatus.GetCriticalDamage();


}
