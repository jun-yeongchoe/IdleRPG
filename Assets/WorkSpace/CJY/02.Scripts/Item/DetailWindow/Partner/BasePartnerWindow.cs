using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;



public class BasePartnerWindow : MonoBehaviour
{
    [Header("Basic UI Settings")]
    public Image icon;
    public TextMeshProUGUI nameText, rankText, levelText; 

    [Header("Exp bar")]
    public Slider expSlider;
    public TextMeshProUGUI countText;

    [Header("Stats & Description")]
    public TextMeshProUGUI atkDamageText, atkSpeedText;

    public PartnerDataSO[] partnerArr;

    public ItemBase currentItemData;
    public static PartnerDataSO pickedPartner;


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

        if(itemData is PartnerDataSO partnerData)
        {
            atkDamageText.text = $"{partnerData.GetFindDamage(currentLevel)}";
            atkSpeedText.text = $"{partnerData.GetAttackInterval()}";
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
        if(currentItemData is PartnerDataSO data)
        {
            PartnerMerge itemMerge = FindObjectOfType<PartnerMerge>();
            bool success = itemMerge.TryMerge(data.ID);

            if (success)
            {
                PartnerInventoryList invenList = FindObjectOfType<PartnerInventoryList>();
                invenList.RefreshInvenUI();

                var save = DataManager.Instance.CompanionDict[data.ID];
                UpdateUI(data, save.level, save.value);
            }
        }
        PartnerInventoryList list = FindObjectOfType<PartnerInventoryList>();
        list.RefreshList();
    }

    public void OnClickEquipBtn()
    {
        if (currentItemData == null) return;
        if(currentItemData is PartnerDataSO data)
        {
            int[] partnerSlots = DataManager.Instance.CompanionSlot;
            for(int i = 0; i< partnerSlots.Length; i++)
            {
                if(partnerSlots[i] == data.ID) 
                {
                    CommonPopup.Instance.ShowAlert("경고!", "이미 장착된 동료입니다.", "확인");
                    return;
                }
            }
            pickedPartner = data;

            this.gameObject.SetActive(false);
        }

    }

    public void Refresh()
    {
        ItemSaveData currentDict = DataManager.Instance.CompanionDict[currentItemData.ID];
        UpdateUI(currentItemData, currentDict.level, currentDict.value);
    }


}
