using UnityEngine;

public class PlayerStat : MonoBehaviour
{
    public static PlayerStat instance { get; private set; }

    [Header("최종 계산된 스탯 (확인용)")]
    public float atkPower;      // 공격력 (a)
    public float hp;            // 체력 (b)
    public float atkSpeed;      // 공격속도 (c)
    public float hpGen;         // 체력재생 (d)
    public float criticalChance; // 치명타확률 (e)
    public float criticalDamage; // 치명타데미지 (f)

    private const float attackDelayDenominator = 1f;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        // 게임 시작 시 한 번 계산
        UpdateFinalStats();
    }

    /// <summary>
    /// 모든 스탯 공식을 적용하여 최종 수치를 갱신합니다.
    /// </summary>
    public void UpdateFinalStats()
    {
        if (DataManager.Instance == null || ItemDataManager.Instance == null) return;

        // 1. 장비 보너스 합산 변수 (초기값 0 = 증가량 없음)
        float weaponAtkBonus = 0;
        float armorHpBonus = 0;
        float accessoryAtkBonus = 0;
        float accessoryHpBonus = 0;

        // 장착 슬롯 리스트가 null이 아닌지 확인
        if (DataManager.Instance.EquipSlot != null)
        {
            foreach (int itemID in DataManager.Instance.EquipSlot)
            {
                // 빈 슬롯(0 이하)은 계산 건너뜀
                if (itemID <= 0) continue;

                var data = ItemDataManager.Instance.GetEquipment(itemID);
                
                // 데이터 매니저에 해당 ID의 SO가 없는 경우 방어
                if (data == null)
                {
                    Debug.LogWarning($"[PlayerStat] ID {itemID}에 해당하는 장비 데이터를 찾을 수 없습니다.");
                    continue;
                }

                int level = 1;
                if (DataManager.Instance.InventoryDict != null && 
                    DataManager.Instance.InventoryDict.TryGetValue(itemID, out var save))
                {
                    level = save.level;
                }

                // 장비 공식 적용
                float bonusValue = data.BaseStatBoost + (level - 1) * data.StatPerLevel;

                // ID 범위 분류
                if (itemID <= 3999) weaponAtkBonus += bonusValue;
                else if (itemID <= 6999) armorHpBonus += bonusValue;
                else if (itemID <= 9999)
                {
                    accessoryAtkBonus += bonusValue;
                    accessoryHpBonus += bonusValue;
                }
            }
        }

        // 2. 최종 스탯 계산 (장비가 없으면 Bonus 값들이 0이 되어 기본 스탯만 남음)
        
        // 공격력 계산
        float baseAtk = 10f + (DataManager.Instance.AtkLv - 1) * 10f;
        atkPower = baseAtk * (1f + weaponAtkBonus + accessoryAtkBonus);

        // 체력 계산
        float baseHp = 100f + (DataManager.Instance.HpLv - 1) * 100f;
        hp = baseHp * (1f + armorHpBonus + accessoryHpBonus);
        // 공격속도, 재생, 치명타 등은 레벨 기반이므로 장비 유무와 상관없이 계산됨
        atkSpeed = attackDelayDenominator / (1f + (DataManager.Instance.AtSpeedLv - 1) * 0.01f);
        hpGen = 7f + (DataManager.Instance.RecoverLv - 1) * 0.7f;
        criticalChance = 0f + (DataManager.Instance.CritPerLv - 1) * 0.001f;
        criticalDamage = 1.2f + (DataManager.Instance.CritDmgLv - 1) * 0.01f;
    }

    /// <summary>
    /// 공격 시점에 호출하여 최종 가할 데미지를 계산합니다.
    /// </summary>
    public (bool isCrit, float damage) GetAttackDamage()
    {
        // 치명타 발생 체크
        bool isCrit = Random.value <= criticalChance;

        // 치명타 여부에 따른 데미지 결정
        float finalDamage = isCrit ? atkPower * criticalDamage : atkPower;

        return (isCrit, finalDamage);
    }

    #region UI 및 레벨업 호환용 메서드
    public void SetAttackPower() 
    {
        // 레벨 데이터는 DataManager에서 가져오므로 단순히 전체 수치를 다시 계산합니다.
        UpdateFinalStats(); 
    }

    public void SetHP() 
    {
        UpdateFinalStats();
        // 체력이 바뀌었으므로 플레이어 HP바 등을 갱신하는 로직이 필요하다면 여기에 추가
    }

    public void SetHPGen() 
    {
        UpdateFinalStats();
    }

    public void SetAtkSpeed() 
    {
        UpdateFinalStats();
    }

    public void SetCriticalChance() 
    {
        UpdateFinalStats();
    }

    public void SetCriticalDamage() 
    {
        UpdateFinalStats();
    }
    #endregion
}