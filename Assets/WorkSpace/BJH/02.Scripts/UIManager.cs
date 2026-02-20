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

    [Header("�⺻ BG ����")]
    [SerializeField] private GameObject bgWindow;

    [Header("�޴� ��Ʈ ����")]
    [SerializeField] private MenuSet charMenu;
    [SerializeField] private MenuSet partnerMenu;
    [SerializeField] private MenuSet dungeonMenu;
    [SerializeField] private MenuSet shopMenu;

#region 최준영 추가
    [Header("In charMenu")]
    [SerializeField] private MenuSet charMenu_Inventory;
    [SerializeField] private MenuSet charMenu_Skill;
    [SerializeField] private MenuSet charMenu_SP;
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
    }

    public void OnClickShop()
    {
        ToggleMenu(shopMenu);
    }

#region 최준영 추가
    public void OnClickChar_Inventory()
    {
        ToggleMenu(charMenu_Inventory);
    }
    public void OnClickChar_Skill()
    {
        ToggleMenu(charMenu_Skill);
    }
    public void OnClickChar_SP()
    {
        ToggleMenu(charMenu_SP);
    }

    // AllClose()가 되지않고 하던, 아니면 UI 구조를 변경해서 해결해야할듯
#endregion

    private void ToggleMenu(MenuSet selected)
    {
        // �̹� �����ִ� �޴��� ������ ��
        if (selected.panel.activeSelf == true)
        {
            selected.panel.SetActive(false); // ���빰 ����
            UpdateIcons(selected, false); // ������ ����
            bgWindow.SetActive(false); // ������
        }
        // �����ִ� �޴��� ������ ��
        else
        {
            AllClose(); // ���� �ݰ� �ѱ�
            // ��� ��
            bgWindow.SetActive(true);

            selected.panel.SetActive(true); // �ǳ� �ѱ�
            UpdateIcons(selected, true); // ������ ����
        }
    }

    private void UpdateIcons(MenuSet menu, bool isOpen)
    {
        // �����̸� �ݱ� ������
        if (isOpen == true)
        {
            menu.normalIcon.SetActive(false);
            menu.closeIcon.SetActive(true);
        }
        // ���¾ƴϸ� �⺻ ������
        else
        {
            menu.normalIcon.SetActive(true);
            menu.closeIcon.SetActive(false);
        } 
    }

    public void AllClose()
    {
        // ��� �ǳ� ����
        bgWindow.SetActive(false);
        charMenu.panel.SetActive(false);
        partnerMenu.panel.SetActive(false);
        dungeonMenu.panel.SetActive(false);
        shopMenu.panel.SetActive(false);

        // ��� �������� �⺻ ���·�
        UpdateIcons(charMenu, false);
        UpdateIcons(partnerMenu, false);
        UpdateIcons(dungeonMenu, false);
        UpdateIcons(shopMenu, false);
    }
}