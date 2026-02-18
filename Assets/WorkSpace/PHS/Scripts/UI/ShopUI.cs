using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    [Header("이 패널의 상점 타입 (인스펙터 지정)")]
    public ShopManager.ShopType shopType;

    [Header("UI 연결")]
    public TextMeshProUGUI levelText;
    public Slider expSlider;
    public TextMeshProUGUI expText;
    public Button btnSummon11;
    public Button btnSummon35;

    void Start()
    {
        if (btnSummon11 != null) btnSummon11.onClick.AddListener(() => OnClickSummon(11));
        if (btnSummon35 != null) btnSummon35.onClick.AddListener(() => OnClickSummon(35));
    }

    void Update()
    {
        RefreshUI();
    }

    void RefreshUI()
    {
        if (DataManager.Instance == null || ShopManager.Instance == null) return;

        int index = (int)shopType;
        int level = DataManager.Instance.ShopLevels[index];
        int exp = DataManager.Instance.ShopExps[index];
        int maxExp = ShopManager.Instance.GetMaxExp(level);

        levelText.text = $"Lv.{level}";

        if (maxExp > 0)
        {
            expSlider.value = (float)exp / maxExp;
            expText.text = $"{exp} / {maxExp}";
        }
        else
        {
            expSlider.value = 1f;
            expText.text = "MAX";
        }
    }

    void OnClickSummon(int count)
    {
        ShopManager.Instance.Summon(shopType, count);
    }
}
