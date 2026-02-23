using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GachaShop_Display : MonoBehaviour
{

    [Header("UI Ref")]
    [SerializeField] private GameObject resultPanel;
    [SerializeField] private Slider expSlider;
    [SerializeField] private TextMeshProUGUI expText;
    [SerializeField] private TextMeshProUGUI levelText;

    [Header("Shop Settings")]
    [SerializeField] private int shopIndex = 0; // 0: Equip, 1: Skill, 2: Partner
    private const int BASE_EXP = 35;

    void Update()
    {
        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        if(DataManager.Instance == null) return;

        int currentLevel = DataManager.Instance.ShopLevels[shopIndex];
        int currentExp = DataManager.Instance.ShopExps[shopIndex];

        int requiredExp = BASE_EXP * (int)Mathf.Pow(2, currentLevel - 1);

        if (expSlider != null)
        {
            expSlider.value = (float)currentExp / requiredExp;
        }

        if (expText != null)
        {
            expText.text = $"{currentExp} / {requiredExp}";
        }

        if (levelText != null)
        {
            levelText.text = $"{GetShopName()} Lv {currentLevel:D2}";
        }
    }

    private string GetShopName()
    {
        switch (shopIndex)
        {
            case 0: return "Weapon";
            case 1: return "Skill";
            case 2: return "Partner";
            default: return "";
        }
    }

    public void Cancel()
    {
        if(!resultPanel.activeSelf) return;
        resultPanel.SetActive(false);
    }
}
