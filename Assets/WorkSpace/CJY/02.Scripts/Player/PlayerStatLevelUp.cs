using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Numerics;

public class PlayerStatLevelUp : MonoBehaviour
{
    [Header("stats level")]
    [SerializeField]TextMeshProUGUI atk_p_l, hp_l, hp_g_l, atk_s_l, crit_p_l, crit_d_l;
   
    [Header("stats value")]
    [SerializeField]TextMeshProUGUI atk_p, hp, hp_g, atk_s, crit_p, crit_d;

    [Header("cost value")]
    [SerializeField]TextMeshProUGUI atk_p_c, hp_c, hp_g_c, atk_s_c, crit_p_c, crit_d_c;

    [Header("buttons")]
    [SerializeField] Button atk_p_b, hp_b, hp_g_b, atk_s_b, crit_p_b, crit_d_b;

    [SerializeField] StatBase[] statBases;

    private ReactiveProperty<int> atk_p_level = new ReactiveProperty<int>(1), 
    hp_level = new ReactiveProperty<int>(1), 
    hp_g_level = new ReactiveProperty<int>(1), 
    atk_s_level = new ReactiveProperty<int>(1), 
    crit_p_level = new ReactiveProperty<int>(1), 
    crit_d_level = new ReactiveProperty<int>(1);

    public enum StatType{ Atk, Hp, HpRegen, AtkSpeed, CritChance, CritDamage}
    public PlayerStatus playerStatus; 
    CoinDisplay coinDisplay;


    void Start()
    {
        coinDisplay = FindObjectOfType<CoinDisplay>();

        atk_p_b.onClick.AddListener(() => OnClickLevelUp(StatType.Atk));
        hp_b.onClick.AddListener(() => OnClickLevelUp(StatType.Hp));
        hp_g_b.onClick.AddListener(() => OnClickLevelUp(StatType.HpRegen));
        atk_s_b.onClick.AddListener(() => OnClickLevelUp(StatType.AtkSpeed));
        crit_p_b.onClick.AddListener(() => OnClickLevelUp(StatType.CritChance));
        crit_d_b.onClick.AddListener(() => OnClickLevelUp(StatType.CritDamage));

        atk_p_level.AddAction(OnChangedLevel1);
        hp_level.AddAction(OnChangedLevel2);
        hp_g_level.AddAction(OnChangedLevel3);
        atk_s_level.AddAction(OnChangedLevel4);
        crit_p_level.AddAction(OnChangedLevel5);
        crit_d_level.AddAction(OnChangedLevel6);

        if (DataManager.Instance!=null)
        {
            atk_p_level.Value = DataManager.Instance.AtkLv;
            hp_level.Value = DataManager.Instance.HpLv;
            hp_g_level.Value = DataManager.Instance.RecoverLv;
            atk_s_level.Value = DataManager.Instance.AtSpeedLv;
            crit_p_level.Value = DataManager.Instance.CritPerLv;
            crit_d_level.Value = DataManager.Instance.CritDmgLv;
        }
        else
        {
            atk_p_level.Value = playerStatus.atkPower;
            hp_level.Value = playerStatus.hp;
            hp_g_level.Value = playerStatus.hpGen;
            atk_s_level.Value = playerStatus.atkSpeed;
            crit_p_level.Value = playerStatus.criticalChance;
            crit_d_level.Value = playerStatus.criticalDamage;
        }

            StartCoroutine(WaitForDataLoadAndUIRefresh());

    }

    IEnumerator WaitForDataLoadAndUIRefresh()
    {
        var loader = FindObjectOfType<PlayerStatLoaderFromGoogleSheets>();
        if(loader != null)
        {
            while(!loader.isLoaded)
            {
                yield return null;
            }
            LevelUIRefresh();
        }
    }

    void OnDestroy()
    {
        atk_p_level.RemoveAction(OnChangedLevel1);
        hp_level.RemoveAction(OnChangedLevel2);
        hp_g_level.RemoveAction(OnChangedLevel3);
        atk_s_level.RemoveAction(OnChangedLevel4);
        crit_p_level.RemoveAction(OnChangedLevel5);
        crit_d_level.RemoveAction(OnChangedLevel6);
    }

    public void OnClickLevelUp(StatType type)
    {
        BigInteger cost = GetCurrentCost(type);

        Debug.Log($"[LevelUp] StatType: {type}");
        Debug.Log($"[LevelUp] Current Gold: {DataManager.Instance.Gold}");        
        Debug.Log($"[LevelUp] Required Cost: {cost}");
        Debug.Log($"[LevelUp] Gold >= Cost ? {DataManager.Instance.Gold >= cost}");

        if (DataManager.Instance.Gold < cost)
        {
            Debug.Log("골드가 부족합니다!");
            return;
        }

        // 골드 차감 및 레벨업
        DataManager.Instance.AddGold(-cost);
        coinDisplay.UpdateCoinDisplay();

        switch (type)
        {
            case StatType.Atk:
                DataManager.Instance.AtkLv++;
                atk_p_level.Value= DataManager.Instance.AtkLv; 
                break;
            case StatType.Hp:
                DataManager.Instance.HpLv++;
                hp_level.Value= DataManager.Instance.HpLv; 
                break;
            case StatType.HpRegen:
                DataManager.Instance.RecoverLv++;
                hp_g_level.Value= DataManager.Instance.RecoverLv; 
                break;
            case StatType.AtkSpeed:
                DataManager.Instance.AtSpeedLv++;
                atk_s_level.Value=DataManager.Instance.AtSpeedLv; 
                break;
            case StatType.CritChance:
                if(DataManager.Instance.CritPerLv >= 1000) 
                {
                    Debug.Log("이미 만렙");
                    break;
                }
                DataManager.Instance.CritPerLv++;
                crit_p_level.Value= DataManager.Instance.CritPerLv; 
                break;
            case StatType.CritDamage:
                DataManager.Instance.CritDmgLv++;
                crit_d_level.Value= DataManager.Instance.CritDmgLv; 
                break;
        }

        if (EventManager.Instance != null) EventManager.Instance.TriggerEvent("StatChange");
    }

    private void LevelUIRefresh()
    {
        OnChangedLevel1(atk_p_level.Value);
        OnChangedLevel2(hp_level.Value);
        OnChangedLevel3(hp_g_level.Value);
        OnChangedLevel4(atk_s_level.Value);
        OnChangedLevel5(crit_p_level.Value);
        OnChangedLevel6(crit_d_level.Value);
    }

    #region UI변경 메서드s
    private void OnChangedLevel1(int value)
    {
        playerStatus.atkPower = atk_p_level.Value;  // Player SO의 atkPower(레벨) 값 변경
        PlayerStat.instance.SetAttackPower(); // PlayerStat 싱글톤의 atkPower 값 갱신
        // UI 업데이트
        atk_p_l.SetText("Lv.{0}",atk_p_level.Value); // .text사용에서 SetText 사용으로 변경하여 boxing 방지 -> 최적화
        atk_p.SetText("{0}", PlayerStat.instance.atkPower);
        atk_p_c.SetText("바용 : {0}", playerStatus.GetAtkCost());
    }
    private void OnChangedLevel2(int value)
    {
        playerStatus.hp = hp_level.Value;
        PlayerStat.instance.SetHP();
        hp_l.SetText("Lv.{0}",hp_level.Value);
        hp.SetText("{0}", PlayerStat.instance.hp);
        hp_c.SetText("바용 : {0}", playerStatus.GetHPCost());
    }
    private void OnChangedLevel3(int value)
    {
        playerStatus.hpGen = hp_g_level.Value;
        PlayerStat.instance.SetHPGen();
        hp_g_l.SetText("Lv.{0}",hp_g_level.Value); 
        hp_g.SetText("{0}", PlayerStat.instance.hpGen);
        hp_g_c.SetText("바용 : {0}", playerStatus.GetHPGenCost());
    }
    private void OnChangedLevel4(int value)
    {
        playerStatus.atkSpeed = atk_s_level.Value;
        PlayerStat.instance.SetAtkSpeed();
        atk_s_l.SetText("Lv.{0}",atk_s_level.Value);
        atk_s.SetText("{0}", PlayerStat.instance.atkSpeed);
        atk_s_c.SetText("바용 : {0}", playerStatus.GetAtkSpeedCost());
    }
    private void OnChangedLevel5(int value)
    {
        playerStatus.criticalChance = crit_p_level.Value;
        PlayerStat.instance.SetCriticalChance();
        crit_p_l.SetText("Lv.{0}",crit_p_level.Value);
        crit_p.SetText("{0}", PlayerStat.instance.criticalChance);
        crit_p_c.SetText("바용 : {0}", playerStatus.GetCritChanceCost());
    }
    private void OnChangedLevel6(int value)
    {
        playerStatus.criticalDamage = crit_d_level.Value;
        PlayerStat.instance.SetCriticalDamage();
        crit_d_l.SetText("Lv.{0}",crit_d_level.Value);
        crit_d.SetText("{0}", PlayerStat.instance.criticalDamage);
        crit_d_c.SetText("바용 : {0}", playerStatus.GetCritDamageCost());
    }
    #endregion

    private BigInteger GetCurrentCost(StatType type)
    {
        return type switch
        {
            StatType.Atk => (BigInteger)playerStatus.GetAtkCost(),
            StatType.Hp => (BigInteger)playerStatus.GetHPCost(),
            StatType.HpRegen => (BigInteger)playerStatus.GetHPGenCost(),
            StatType.AtkSpeed => (BigInteger)playerStatus.GetAtkSpeedCost(),
            StatType.CritChance => (BigInteger)playerStatus.GetCritChanceCost(),
            StatType.CritDamage => (BigInteger)playerStatus.GetCritDamageCost(),
            _ => 0
        };
    }
}
