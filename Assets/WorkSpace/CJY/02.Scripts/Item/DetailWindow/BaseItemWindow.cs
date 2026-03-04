using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;



public class BaseItemWindow : MonoBehaviour
{
    [Header("Basic UI Settings")]
    public Image icon;
    public TextMeshProUGUI nameText, rankText, levelText; 

    [Header("Exp bar")]
    public Slider expSlider;
    public TextMeshProUGUI countText;

    [Header("Stats & Description")]
    public TextMeshProUGUI effectValueText_AttackPower, effectValueText_HP;

    public EquipmentDataSO[] equipmentArr;

    public ItemBase currentItemData;

    public void UpdateUI(ItemBase itemData, int currentLevel, int currentEa)
    {
        currentItemData = itemData;
        icon.sprite = itemData.transform.Find("IconImage").GetComponent<Image>().sprite; // 아이콘
        nameText.text = itemData.Name_KR; //이름
        rankText.text = itemData.itemRank.ToString(); //랭크
        rankText.color = GetRankColor(itemData.itemRank); // 랭크 컬러
        levelText.text = $"Lv {currentLevel:D2}"; // 레벨
        int required = itemData.GetRequiredComposeCount(currentLevel); //총 요구량
        countText.text = $"{currentEa} / {required}"; // 보유 수량 / 총 요구량

        expSlider.maxValue = required; // 슬라이더 BG
        expSlider.value = currentEa; // 슬라이더 Fill box

        if(itemData is EquipmentDataSO equipData)
        {
            float finalValue = equipData.GetFinalValue(currentLevel);
            string format = $"+ {(finalValue-1)*100:f2} %";

            switch (equipData.equipmentType)
            {
                case EquipmentType.Weapon:
                    effectValueText_AttackPower.text = $"Attack Power {format}";
                    break;
                case EquipmentType.Armor:
                    effectValueText_HP.text = $"HP {format}";
                    break;
                case EquipmentType.Accessory:
                    effectValueText_AttackPower.text = $"Attack Power {format}";
                    effectValueText_HP.text = $"HP {format}";
                    break;
            }
        }
    }

    private Color GetRankColor(ItemRank rank)
    {
        switch (rank)
        {
            case ItemRank.Common:    return Color.white;
            case ItemRank.Uncommon:  return new Color(0.2f, 1f, 0.2f); // 연초록
            case ItemRank.Rare:      return new Color(0.2f, 0.6f, 1f); // 파랑
            case ItemRank.Epic:      return new Color(0.7f, 0.2f, 1f); // 보라
            case ItemRank.Legendary: return new Color(1f, 0.6f, 0f);    // 주황
            case ItemRank.Mythic:    return Color.red;                 // 빨강
            case ItemRank.Celestial: return Color.cyan;                // 하늘색
            default: return Color.white;
        }
    }

    public void OnClickMergeBtn()
    {
        if(currentItemData is EquipmentDataSO data)
        {
            ItemMerge itemMerge = GetComponent<ItemMerge>();
            bool success = itemMerge.TryMerge(data.ID, data.equipmentType);

            if (success)
            {
                InventoryList invenList = GetComponent<InventoryList>();
                invenList.RefreshInvenUI(data.equipmentType);

                var save = DataManager.Instance.InventoryDict[data.ID];
                UpdateUI(data, save.level, save.value);
            }
        }
        InventoryList list = GetComponent<InventoryList>();
        list.RefreshList();
    }

    public void Refresh()
    {
        ItemSaveData currentDict = DataManager.Instance.InventoryDict[currentItemData.ID];
        UpdateUI(currentItemData, currentDict.level, currentDict.value);
    }


}
