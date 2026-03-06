using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;



public class BaseSkillWindow : MonoBehaviour
{
    [Header("Basic UI Settings")]
    public Image icon;
    public TextMeshProUGUI nameText, rankText, levelText; 

    [Header("Exp bar")]
    public Slider expSlider;
    public TextMeshProUGUI countText;

    [Header("Stats & Description")]
    public TextMeshProUGUI descriptionText, cooltimeText;

    public SkillDataSo[] skillArr;

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

        if(itemData is SkillDataSo skillData)
        {
            descriptionText.text = $"{itemData.Name_KR}(을)를 사용하여 적에게 {skillData.GetFinalValue(currentLevel)*100}%만큼의 \n데미지를 {skillData.StrikeCount}회 입힌다.";
            cooltimeText.text = $"{skillData.Cooltime:N0}s";
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
        if(currentItemData is SkillDataSo data)
        {
            SkillMerge itemMerge = FindObjectOfType<SkillMerge>();
            bool success = itemMerge.TryMerge(data.ID);

            if (success)
            {
                SkillInventoryList invenList = FindObjectOfType<SkillInventoryList>();
                invenList.RefreshInvenUI();

                var save = DataManager.Instance.SkillDict[data.ID];
                UpdateUI(data, save.level, save.value);
            }
        }
        SkillInventoryList list = FindObjectOfType<SkillInventoryList>();
        list.RefreshList();
    }

    public void Refresh()
    {
        ItemSaveData currentDict = DataManager.Instance.SkillDict[currentItemData.ID];
        UpdateUI(currentItemData, currentDict.level, currentDict.value);
    }


}
