using System.Security.Cryptography;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [System.Serializable]
    public class MenuSet
    {
        public string menuName;
        public GameObject panel;
        public GameObject normalIcon;
        public GameObject closeIcon;

        [Header("True=안꺼짐")]
        public bool ignoreClose;
    }

    [Header("기본 BG 설정")]
    [SerializeField] private GameObject bgWindow;

    [Header("메뉴 세트 설정")]
    [SerializeField] private MenuSet charMenu;
    [SerializeField] private MenuSet partnerMenu;
    [SerializeField] private MenuSet dungeonMenu;
    [SerializeField] private MenuSet shopMenu;
    [SerializeField] private MenuSet questMenu;

#region 최준영 추가
    [Header("In charMenu")]
    [SerializeField] private MenuSet charMenu_Inventory;
    [SerializeField] private MenuSet charMenu_Skill;
    [SerializeField] private MenuSet charMenu_SP;
    [SerializeField] private MenuSet charMenu_Inventory_Slot_W, charMenu_Inventory_Slot_A,charMenu_Inventory_Slot_AC1,charMenu_Inventory_Slot_AC2;
    [SerializeField] private MenuSet TopMenu;
#endregion


    public void OnClickCharacter()
    {
        ToggleMenu(charMenu);
    }

    public void OnClickPartner()
    {
        ToggleMenu(partnerMenu);
    }

    public void OnClickDungeon()
    {
        ToggleMenu(dungeonMenu);
        EventManager.Instance.TriggerEvent("OpenDungeonTab");
    }

    public void OnClickShop()
    {
        ToggleMenu(shopMenu);
    }

    public void OnClickQuest()
    {
        ToggleMenu(questMenu);
    }

    #region 최준영 추가

    //CharMenu 1차 버튼 할당
    public void OnClickCharMenu_Inventory()
    {
        if(charMenu_SP.ignoreClose) charMenu_SP.ignoreClose = !charMenu_SP.ignoreClose;
        if(charMenu_Skill.ignoreClose) charMenu_Skill.ignoreClose = !charMenu_Skill.ignoreClose;

        CloseAllowed(charMenu_SP);
        CloseAllowed(charMenu_Skill);
        ToggleMenu(charMenu_Inventory);
        
        if(!charMenu_SP.ignoreClose) charMenu_SP.ignoreClose = !charMenu_SP.ignoreClose;
        if(!charMenu_Skill.ignoreClose) charMenu_Skill.ignoreClose = !charMenu_Skill.ignoreClose;
    }
    public void OnClickCharMenu_Skill()
    {
        if(charMenu_SP.ignoreClose) charMenu_SP.ignoreClose = !charMenu_SP.ignoreClose;
        if(charMenu_Inventory.ignoreClose) charMenu_Inventory.ignoreClose = !charMenu_Inventory.ignoreClose;

        CloseAllowed(charMenu_Inventory);
        CloseAllowed(charMenu_SP);
        ToggleMenu(charMenu_Skill);

        if(!charMenu_SP.ignoreClose) charMenu_SP.ignoreClose = !charMenu_SP.ignoreClose;
        if(!charMenu_Inventory.ignoreClose) charMenu_Inventory.ignoreClose = !charMenu_Inventory.ignoreClose;
    }
    public void OnClickCharMenu_SP()
    {
        if(charMenu_Inventory.ignoreClose) charMenu_Inventory.ignoreClose = !charMenu_Inventory.ignoreClose;
        if(charMenu_Skill.ignoreClose) charMenu_Skill.ignoreClose = !charMenu_Skill.ignoreClose;

        CloseAllowed(charMenu_Inventory);
        CloseAllowed(charMenu_Skill);
        ToggleMenu(charMenu_SP);

        if(!charMenu_Inventory.ignoreClose) charMenu_Inventory.ignoreClose = !charMenu_Inventory.ignoreClose;
        if(!charMenu_Skill.ignoreClose) charMenu_Skill.ignoreClose = !charMenu_Skill.ignoreClose;
    }

    //CharMenu 2차 버튼 할당
    public void OnClickCharMenu_Inventory_Slot_W()
    {
        ToggleMenu(charMenu_Inventory_Slot_W);
    }
    public void OnClickCharMenu_Inventory_Slot_A()
    {
        ToggleMenu(charMenu_Inventory_Slot_A);
    }
    public void OnClickCharMenu_Inventory_Slot_AC1()
    {
        ToggleMenu(charMenu_Inventory_Slot_AC1);
    }
    public void OnClickCharMenu_Inventory_Slot_AC2()
    {
        ToggleMenu(charMenu_Inventory_Slot_AC2);
    }

    public void OnClickTopMenu()
    {
        ToggleMenu(TopMenu);
    }
    #endregion

    private void ToggleMenu(MenuSet selected)
    {
        // 이미 켜져있는 메뉴를 눌렀을 때
        if (selected.panel.activeSelf == true)
        {
            selected.panel.SetActive(false); // 내용물 끄기
            UpdateIcons(selected, false); // 아이콘 복구
            if(!selected.ignoreClose) bgWindow.SetActive(false); // 배경끄기
        }
        // 꺼져있는 메뉴를 눌렀을 때
        else
        {
            if(!selected.ignoreClose) AllClose(); // 전부 닫고 켜기
            // 배경 온
            if (!selected.ignoreClose) bgWindow.SetActive(true);

            selected.panel.SetActive(true); // 판넬 켜기
            UpdateIcons(selected, true); // 아이콘 변경
        }
    }

    private void UpdateIcons(MenuSet menu, bool isOpen)
    {
        // 오픈이면 닫기 아이콘
        if (isOpen == true)
        {
            menu.normalIcon.SetActive(false);
            menu.closeIcon.SetActive(true);
        }
        // 오픈아니면 기본 아이콘
        else
        {
            menu.normalIcon.SetActive(true);
            menu.closeIcon.SetActive(false);
        } 
    }

    public void AllClose()
    {
        // 모든 판넬 끄기
        bgWindow.SetActive(false);
        CloseAllowed(charMenu);
        CloseAllowed(partnerMenu);
        CloseAllowed(dungeonMenu);
        CloseAllowed(shopMenu);
        CloseAllowed(questMenu);
    }

    private void CloseAllowed(MenuSet menu)
    {
        if (menu != null && menu.panel != null && !menu.ignoreClose)
        { 
            menu.panel.SetActive(false);
            UpdateIcons(menu, false);
        }
    }
}