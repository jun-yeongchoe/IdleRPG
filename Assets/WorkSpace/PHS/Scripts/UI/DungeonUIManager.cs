using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DungeonUIManager : MonoBehaviour
{
    [Header("UI연결")]
    public TextMeshProUGUI goldTicketText;
    public Button enterGold;

    public TextMeshProUGUI bossTicketText;
    public Button enterBoss;

    public TextMeshProUGUI dwarfTicketText;
    public Button enterDwarf;

    public void Start()
    {
        if (enterGold != null) enterGold.onClick.AddListener(EnterGoldDungeon);
        if (enterBoss != null) enterBoss.onClick.AddListener(EnterBossRush);
        if (enterDwarf != null) enterDwarf.onClick.AddListener(EnterDwarfKing);

        RefreshUI();
    }

    public void RefreshUI()
    {
        if (DataManager.Instance == null) return;

        goldTicketText.text =$"{ DataManager.Instance.GoldDungeonTicket}/2";
        bossTicketText.text =$"{ DataManager.Instance.BossRushTicket}/2";
        dwarfTicketText.text =$"{ DataManager.Instance.DwarfKingTicket}/2";

        enterGold.interactable = DataManager.Instance.GoldDungeonTicket > 0;
        enterBoss.interactable = DataManager.Instance.BossRushTicket > 0;
        enterDwarf.interactable = DataManager.Instance.DwarfKingTicket > 0;
    }

    public void EnterGoldDungeon()
    {
        if (DataManager.Instance.GoldDungeonTicket > 0)
        {
            DataManager.Instance.GoldDungeonTicket--;

            SceneManager.LoadScene("GoldDungeon");
        }
        else
            CommonPopup.Instance.ShowAlert("입장권 부족!", "입장권이 부족합니다!");
    }
    public void EnterBossRush()
    {
        if (DataManager.Instance.BossRushTicket > 0)
        {
            DataManager.Instance.BossRushTicket--;

            SceneManager.LoadScene("BossRushManager");
        }
        else
            CommonPopup.Instance.ShowAlert("입장권 부족!", "입장권이 부족합니다!");
    }

    public void EnterDwarfKing()
    {
        if (DataManager.Instance.DwarfKingTicket > 0)
        {
            DataManager.Instance.DwarfKingTicket--;

            SceneManager.LoadScene("DwarfKingDungeon");
        }
        else
            CommonPopup.Instance.ShowAlert("입장권 부족!", "입장권이 부족합니다!");
    }
}
