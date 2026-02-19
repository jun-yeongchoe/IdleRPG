using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public enum TabType { Equipment, Skill, Companion }

    [Header("현재 탭 설정")]
    public TabType currentTab;

    [Header("슬롯 생성 구역")]
    public Transform contentTarget;
    public GameObject slotPrefab;   //ItemUI 프리팹

    [Header("선택/장착 정보 패널")]
    public GameObject infoPanel;             //아이템 누르면 뜰 정보창
    public TextMeshProUGUI infoNameText;     //선택한 아이템 ID 표시
    public Button equipButton;               //장착 버튼

    private int selectedItemID = -1; //현재 클릭한 아이템 ID

    private void OnEnable()
    {
        //인벤토리 창이 켜질 때마다 UI 갱신 및 정보창 숨기기
        if (infoPanel != null) infoPanel.SetActive(false);
        RefreshUI();
    }

    public void RefreshUI()
    {
        if (DataManager.Instance == null) return;

        foreach (Transform child in contentTarget) Destroy(child.gameObject);

        Dictionary<int, ItemSaveData> targetDict = null;
        if (currentTab == TabType.Equipment) targetDict = DataManager.Instance.InventoryDict;
        else if (currentTab == TabType.Skill) targetDict = DataManager.Instance.SkillDict;
        else if (currentTab == TabType.Companion) targetDict = DataManager.Instance.CompanionDict;

        if (targetDict == null) return;

        foreach (var pair in targetDict)
        {
            GameObject slotObj = Instantiate(slotPrefab, contentTarget);
            ItemUI slot = slotObj.GetComponent<ItemUI>();

            if (slot != null)
            {
                bool isEquipped = CheckIfEquipped(pair.Key);
                slot.Setup(pair.Key, pair.Value.value, isEquipped, OnSlotClick);
            }
        }
    }

    private bool CheckIfEquipped(int itemID)
    {
        if (currentTab == TabType.Equipment)
        {
            foreach (int id in DataManager.Instance.EquipSlot)
                if (id == itemID) return true;
        }
        else if (currentTab == TabType.Skill)
        {
            foreach (int id in DataManager.Instance.SkillSlot)
                if (id == itemID) return true;
        }
        return false;
    }

    private void OnSlotClick(int itemID)
    {
        selectedItemID = itemID;

        if (infoPanel != null) infoPanel.SetActive(true);
        if (infoNameText != null) infoNameText.text = $"선택한 무기 ID:\n{itemID}";

        if (equipButton != null)
        {
            equipButton.onClick.RemoveAllListeners();
            equipButton.onClick.AddListener(EquipSelectedItem);
        }
    }

    private void EquipSelectedItem()
    {
        if (selectedItemID == -1) return;

        if (currentTab == TabType.Equipment)
        {
            int categoryBase = (selectedItemID / 1000) * 1000;
            int slotIndex = 0;

            if (categoryBase == 4000) slotIndex = 1;
            else if (categoryBase == 7000) slotIndex = 2;

            DataManager.Instance.EquipItem(slotIndex, selectedItemID);
            Debug.Log($"장비 장착 완료! [슬롯 {slotIndex}] 에 {selectedItemID} 번 장착됨.");
        }
        else if (currentTab == TabType.Skill)
        {
            //스킬 장착 로직(지금은 일단 0번 슬롯)
            DataManager.Instance.SkillSlot[0] = selectedItemID;
            Debug.Log($"스킬 장착 완료! [슬롯 0] 에 {selectedItemID} 번 장착됨.");
        }

        //나중에 팀원이 구현할 플레이어 스탯 재계산 함수 호출
        RefreshUI();
    }
}
