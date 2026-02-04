using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

    void Start()
    {
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

        LevelUIRefresh();

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
        switch (type)
        {
            case StatType.Atk: atk_p_level.Value++; break;
            case StatType.Hp:hp_level.Value++; break;
            case StatType.HpRegen:hp_g_level.Value++; break;
            case StatType.AtkSpeed:atk_s_level.Value++; break;
            case StatType.CritChance:crit_p_level.Value++; break;
            case StatType.CritDamage:crit_d_level.Value++; break;
        }
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
        atk_p_l.SetText("Lv.{0}",atk_p_level.Value); // .text사용에서 SetText 사용으로 변경하여 boxing 방지 -> 최적화
        atk_p.SetText("{0}", statBases[0].GetValue(atk_p_level.Value));
        atk_p_c.SetText("바용 : {0}", statBases[0].GetCost(atk_p_level.Value));
    }
    private void OnChangedLevel2(int value)
    {
        hp_l.SetText("Lv.{0}",hp_level.Value);
        hp.SetText("{0}", statBases[1].GetValue(hp_level.Value));
        hp_c.SetText("바용 : {0}", statBases[1].GetCost(hp_level.Value));
    }
    private void OnChangedLevel3(int value)
    {
        hp_g_l.SetText("Lv.{0}",hp_g_level.Value); 
        hp_g.SetText("{0}", statBases[2].GetValue(hp_g_level.Value));
        hp_g_c.SetText("바용 : {0}", statBases[2].GetCost(hp_g_level.Value));
    }
    private void OnChangedLevel4(int value)
    {
        atk_s_l.SetText("Lv.{0}",atk_s_level.Value);
        atk_s.SetText("{0}", statBases[3].GetValue(atk_s_level.Value));
        atk_s_c.SetText("바용 : {0}", statBases[3].GetCost(atk_s_level.Value));
    }
    private void OnChangedLevel5(int value)
    {
        crit_p_l.SetText("Lv.{0}",crit_p_level.Value);
        crit_p.SetText("{0}", statBases[4].GetValue(crit_p_level.Value));
        crit_p_c.SetText("바용 : {0}", statBases[4].GetCost(crit_p_level.Value));
    }
    private void OnChangedLevel6(int value)
    {
        crit_d_l.SetText("Lv.{0}",crit_d_level.Value);
        crit_d.SetText("{0}", statBases[5].GetValue(crit_d_level.Value));
        crit_d_c.SetText("바용 : {0}", statBases[5].GetCost(crit_d_level.Value));
    }
    #endregion
}
