using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemMergeHandler : MonoBehaviour
{
    public static ItemMergeHandler instance { get; private set; }

    // 임시 level 저장소 (DataManager가 바뀌기 전까지 사용)
    public readonly Dictionary<int, int> tempItemLevels = new Dictionary<int, int>();

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);  // 필요하면
        }
        else
        {
            Destroy(gameObject);
        }

        // 로드 시 기존 데이터에서 level 초기화 (필요하면)
        LoadItemLevels();
    }

    public bool TryMerge(int itemId, ItemType itemType = ItemType.Inventory)
    {
        if (itemId <= 0) return false;

        var dm = DataManager.Instance;
        Dictionary<int, ItemSaveData> targetDict = GetItemDataDictionary(itemType);
        if (targetDict == null || !targetDict.TryGetValue(itemId, out ItemSaveData current) || current.value <= 0)
        {
            Debug.Log($"아이템 없음 : {itemId} ({itemType})");
            return false;
        }

        int currentLevel = GetLevel(itemId, itemType);
        int needCount = GetRequiredMergeCount(itemId, currentLevel);

        if (current.value < needCount)
        {
            Debug.Log($"합성 불가 - 필요 {needCount}개 / 보유 {current.value}개 (Lv {currentLevel})");
            return false;
        }

        // 합성 성공
        targetDict[itemId].value = current.value - needCount;
        SetLevel(itemId, itemType, currentLevel + 1);

        if (targetDict[itemId].value <= 0)
        {
            targetDict.Remove(itemId);
            tempItemLevels.Remove(itemId);  // 임시 level도 정리
        }

        Debug.Log($"합성 완료 → {itemId} ({itemType}) Lv.{currentLevel} → Lv.{currentLevel + 1}");
        return true;
    }


   private int GetRequiredMergeCount(int itemId, int currentLevel)
    {
        if (currentLevel < 1) currentLevel = 1;

        int baseCount = GetBaseRequiredCount(itemId);

        // 레벨당 +1 공식
        return baseCount + (currentLevel - 1);
    }

    private int GetBaseRequiredCount(int itemId)
    {
        int tierIndex = -1;

        // weapon / armor / accessory 계열 (1001 ~ 9999)
        if (itemId >= 1001 && itemId <= 9999)
        {
            // 1001을 기준으로 400 단위로 나눔
            tierIndex = (itemId - 1001) / 400;
        }
        // skill_list (10001 ~ 19999)
        else if (itemId >= 10001 && itemId <= 19999)
        {
            tierIndex = (itemId - 10001) / 1000;
        }
        // partner_list (20001 ~ 29999)
        else if (itemId >= 20001 && itemId <= 29999)
        {
            tierIndex = (itemId - 20001) / 1000;
        }
        else
        {
            // 정의되지 않은 범위
            Debug.LogWarning($"정의되지 않은 아이템 ID 등급: {itemId}");
            return 12;
        }

        
        // 0=common, 1=uncommon, 2=rare, 3=epic, 4=legendary, 5=mystic, 6=celestial
        switch (tierIndex)
        {
            case 0: return 12;  // Common
            case 1: return 10;  // Uncommon
            case 2: return  8;  // Rare
            case 3: return  6;  // Epic
            case 4: return  4;  // Legendary
            case 5: return  2;  // Mystic
            case 6: return  1;  // Celestial
            default:
                Debug.LogWarning($"알 수 없는 등급 인덱스 {tierIndex} (ID: {itemId})");
                return 12;
        }
    }

    //DataManager의 InventoryDict,CompanionDict,SkillDict의 형태 int,int에서 int, ItemSaveData형태로 변경 필요
      // ItemMergeHandler 또는 별도 유틸 클래스 안에
    public int GetLevel(int itemId, ItemType itemType)
    {
        // 미래 버전에서 사용할 코드 (주석 처리)
        
        var dict = GetItemDataDictionary(itemType);
        if (dict != null && dict.TryGetValue(itemId, out ItemSaveData data)) return Mathf.Max(1, data.level);
        else return 1;
        
        // 현재 임시 방식
        // tempItemLevels.TryGetValue(itemId, out int level);
        // return Mathf.Max(1, level);
    }

    // 현재 개수를 관리하는 딕셔너리 반환 (임시)
    // private Dictionary<int, int> GetCountDictionary(ItemType itemType)
    // {
    //     var dm = DataManager.Instance;
    //     return itemType switch
    //     {
    //         ItemType.Inventory  => dm.InventoryDict,
    //         ItemType.Companion  => dm.CompanionDict,
    //         ItemType.Skill      => dm.SkillDict,
    //         _                   => null
    //     };
    // }

    // 미래에 Dictionary<int, ItemSaveData> 가 되었을 때 사용할 헬퍼
    private Dictionary<int, ItemSaveData> GetItemDataDictionary(ItemType itemType)
    {
        var dm = DataManager.Instance;
        return itemType switch
        {
            ItemType.Inventory  => dm.InventoryDict,
            ItemType.Companion  => dm.CompanionDict,
            ItemType.Skill      => dm.SkillDict,
            _                   => null
        };
    }

    public enum ItemType
    {
        Inventory,
        Companion,
        Skill
    }

    private void SetLevel(int itemId, ItemType itemType, int newLevel)
    {
        // 미래 버전에서 사용할 코드
        
        var dict = GetItemDataDictionary(itemType);
        if (dict != null && dict.TryGetValue(itemId, out var data))
        {
            data.level = newLevel;
            return;
        }
        
        // 현재 임시 방식
        // tempItemLevels[itemId] = newLevel;
    }

    // 세이브/로드 관련 (나중에 ItemSaveData.level로 옮길 때 참고)
    private void LoadItemLevels()
    {
        // 지금은 기본값 1로 초기화하거나,
        // 기존에 저장된 데이터가 있다면 여기서 로드
        // 예: PlayerPrefs, 별도 json 등
    }

    
}
