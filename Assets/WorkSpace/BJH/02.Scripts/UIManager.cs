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
    }

    [Header("기본 BG 설정")]
    [SerializeField] private GameObject bgWindow;

    [Header("메뉴 세트 설정")]
    [SerializeField] private MenuSet charMenu;
    [SerializeField] private MenuSet partnerMenu;
    [SerializeField] private MenuSet dungeonMenu;
    [SerializeField] private MenuSet shopMenu;


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
    }

    public void OnClickShop()
    {
        ToggleMenu(shopMenu);
    }

    private void ToggleMenu(MenuSet selected)
    {
        // 이미 켜져있는 메뉴를 눌렀을 때
        if (selected.panel.activeSelf == true)
        {
            selected.panel.SetActive(false); // 내용물 끄기
            UpdateIcons(selected, false); // 아이콘 복구
            bgWindow.SetActive(false); // 배경끄기
        }
        // 꺼져있는 메뉴를 눌렀을 때
        else
        {
            AllClose(); // 전부 닫고 켜기
            // 배경 온
            bgWindow.SetActive(true);

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
        charMenu.panel.SetActive(false);
        partnerMenu.panel.SetActive(false);
        dungeonMenu.panel.SetActive(false);
        shopMenu.panel.SetActive(false);

        // 모든 아이콘을 기본 상태로
        UpdateIcons(charMenu, false);
        UpdateIcons(partnerMenu, false);
        UpdateIcons(dungeonMenu, false);
        UpdateIcons(shopMenu, false);
    }
}