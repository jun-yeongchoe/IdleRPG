using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Numerics;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set; }

    public enum BackGroundType
        {
            //타입 이름 정하면 그 때 수정 예정
            ABCDEFG=0,
            HIJKLMNOP=1,
            QRSTUV=2
        }

    //실제 스테이지 번호
    public int currentStageNum = 1;

    //챕터당 스테이지 개수
    public int StageCount = 10;

    //배경 개수
    public int backgroundCount = 3;

    public int[] EquipSlot=new int[4];
    public int[] SkillSlot = new int[5];

    //계산 로직(배경 순환: 10스테이지=1챕터, 매 챕터 클리어시 배경 전환을 위한 함수)
    public BackGroundType BackgroundIndex()
    { 
        int chapterNum = (currentStageNum-1)/StageCount;    //1~10는 0, 11~20은 1챕터 이런식
        int index= chapterNum%backgroundCount;              //11, 111, 10001 등등 커져도 문제x

        return (BackGroundType)index;
    }

    [Header("경제")]
    public BigInteger Gold = 0;
    public BigInteger Scrap = 0;
    public BigInteger Gem = 0;

    [Header("스텟")]
    public int AtkLv = 1;
    public int HpLv = 1;
    public int RecoverLv = 1;
    public int AtSpeedLv = 1;
    public int CritPerLv = 1;
    public int CritDmgLv = 1;

    [Header("던전 입장권 (매일 리셋 대상)")]
    public int GoldDungeonTicket = 2;
    public int BossRushTicket = 2;
    public int DwarfKingTicket = 2;

    public Dictionary<int, ItemSaveData> InventoryDict = new Dictionary<int, ItemSaveData>();

    public Dictionary<int, ItemSaveData> CompanionDict = new Dictionary<int, ItemSaveData>();

    public Dictionary<int, ItemSaveData> SkillDict = new Dictionary<int, ItemSaveData>();

    public int[] ShopLevels = new int[3] { 1, 1, 1 };
    public int[] ShopExps = new int[3] { 0, 0, 0 };

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this; DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    public BigInteger GetGoldperSec()
    {
        BigInteger baseGold = 10;
        BigInteger stageBonus = currentStageNum * 5;

        return baseGold + stageBonus;
    }

    public void AddGold(BigInteger amount)
    {
        Gold += amount;
        if (EventManager.Instance != null) EventManager.Instance.TriggerEvent("CurrencyChange");
    }
    public void AddScrap(BigInteger amount)
    {
        Scrap += amount;
        if (EventManager.Instance != null) EventManager.Instance.TriggerEvent("CurrencyChange");
    }

    public void AddGem(BigInteger amount)
    {
        Gem += amount;
        if (EventManager.Instance != null) EventManager.Instance.TriggerEvent("CurrencyChange");
    }

    public string SendJson()
    {
        GameDataDTO data=new GameDataDTO();

        data.Stage=currentStageNum;

        data.Gold = Gold.ToString();
        data.Scrap = Scrap.ToString();
        data.Gem = Gem.ToString();

        data.AtkLv = AtkLv;
        data.HpLv = HpLv;
        data.RecoverLv = RecoverLv;
        data.AtSpeedLv = AtSpeedLv;
        data.CritPerLv = CritPerLv;
        data.CritDmgLv = CritDmgLv;

        if (PlayerPrefs.HasKey("LastExitTime"))
            data.LastExitTime = PlayerPrefs.GetString("LastExitTime");

        data.InventoryList = DictToList(InventoryDict);
        data.CompanionList = DictToList(CompanionDict);
        data.SkillList = DictToList(SkillDict);

        data.EquipSlot=EquipSlot;
        data.SkillSlot = SkillSlot;

        data.GoldDungeonTicket = GoldDungeonTicket;
        data.BossRushTicket = BossRushTicket;
        data.DwarfKingTicket = DwarfKingTicket;

        data.ShopLevels = ShopLevels;
        data.ShopExps = ShopExps;

        return JsonUtility.ToJson(data);
    }

    public void LoadJson(string jsonString) 
    {
        if (string.IsNullOrEmpty(jsonString)) return;

        GameDataDTO data = JsonUtility.FromJson<GameDataDTO>(jsonString);

        currentStageNum = data.Stage;

        BigInteger.TryParse(data.Gold, out Gold);
        BigInteger.TryParse(data.Scrap, out Scrap);
        BigInteger.TryParse(data.Gem, out Gem);

        AtkLv = data.AtkLv;
        HpLv = data.HpLv;
        RecoverLv = data.RecoverLv;
        AtSpeedLv = data.AtSpeedLv;
        CritPerLv = data.CritPerLv;
        CritDmgLv = data.CritDmgLv;

        if (!string.IsNullOrEmpty(data.LastExitTime))
        {
            PlayerPrefs.SetString("LastExitTime", data.LastExitTime);
            PlayerPrefs.Save();
        }

        InventoryDict = ListToDict(data.InventoryList);
        CompanionDict = ListToDict(data.CompanionList);
        SkillDict = ListToDict(data.SkillList);

        if (data.EquipSlot != null && data.EquipSlot.Length == 4)
        {
            EquipSlot = data.EquipSlot;
        }
        else
        {
            EquipSlot = new int[4];
        }

        if (data.SkillSlot != null && data.SkillSlot.Length == 5)
        {
            SkillSlot = data.SkillSlot;
        }
        else
        {
            SkillSlot = new int[] { 0, 1, 2, -1, -1 };
        }

        if (data.GoldDungeonTicket != -1) GoldDungeonTicket = data.GoldDungeonTicket;
        else GoldDungeonTicket = 2;

        if (data.BossRushTicket != -1) BossRushTicket = data.BossRushTicket;
        else BossRushTicket = 2;

        if (data.DwarfKingTicket != -1) DwarfKingTicket = data.DwarfKingTicket;
        else DwarfKingTicket = 2;

        if (data.ShopLevels != null && data.ShopLevels.Length == 3) ShopLevels = data.ShopLevels;
        if (data.ShopExps != null && data.ShopExps.Length == 3) ShopExps = data.ShopExps;

        Debug.Log("데이터 로드 완료!");
        if (EventManager.Instance != null) EventManager.Instance.TriggerEvent("CurrencyChange");
    }

    private List<ItemSaveData> DictToList(Dictionary<int, ItemSaveData> dict)
    {
        List<ItemSaveData> list = new List<ItemSaveData>();
        if (dict == null) return list;

        foreach (var pair in dict)
        {
            list.Add(pair.Value);
        }
        return list;
    }

    private Dictionary<int, ItemSaveData> ListToDict(List<ItemSaveData> list)
    {
        Dictionary<int, ItemSaveData> dict = new Dictionary<int, ItemSaveData>();
        if (list == null) return dict;

        foreach (var item in list)
        {
            if (!dict.ContainsKey(item.id))
            {
                dict.Add(item.id, item);
            }
        }
        return dict;
    }

    public void EquipItem(int slotIndex, int itemID)
    { 
        if(slotIndex<0||slotIndex>=4)return;

        EquipSlot[slotIndex] = itemID;
    }

    public void UnEquiptItem(int slotIndex) 
    { 
        EquipItem(slotIndex, 0);
    }
}

[System.Serializable]
public class GameDataDTO
{
    public int Stage;

    public string Gold;
    public string Scrap;
    public string Gem;

    public int AtkLv;
    public int HpLv;
    public int RecoverLv;
    public int AtSpeedLv;
    public int CritPerLv;
    public int CritDmgLv;

    public string LastExitTime;

    public List<ItemSaveData> InventoryList;
    public List<ItemSaveData> CompanionList;
    public List<ItemSaveData> SkillList;

    public int GoldDungeonTicket = -1;
    public int BossRushTicket = -1;
    public int DwarfKingTicket = -1;

    public int[] EquipSlot;
    public int[] SkillSlot;

    public int[] ShopLevels;
    public int[] ShopExps;
}
[System.Serializable]
public class ItemSaveData
{
    public int id;
    public int value;
    public int level;
}
