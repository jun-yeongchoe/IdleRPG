using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MergeTest : MonoBehaviour
{
    [Header("테스트 설정")]
    public int testItemId = 1001;           // Common 무기 예시
    public ItemMergeHandler.ItemType itemType = ItemMergeHandler.ItemType.Inventory;
    public int addCount = 20;               // 테스트용으로 강제로 넣을 개수

    [Header("현재 상태 보기")]
    [SerializeField] private int currentCount;
    [SerializeField] private int currentLevel;

    void Start()
    {
        // 처음에 아이템 강제 추가 (테스트용)
        ForceAddItem(testItemId, addCount, itemType);
        UpdateDisplay();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))    // M 키 누르면 합성 시도
        {
            bool success = ItemMergeHandler.instance.TryMerge(testItemId, itemType);
            Debug.Log(success ? "합성 성공!" : "합성 실패");
            UpdateDisplay();
        }
    }

    private void ForceAddItem(int id, int count, ItemMergeHandler.ItemType type)
    {
        var dict = GetDict(type);
        if (dict == null) return;

        if (dict.TryGetValue(id, out ItemSaveData existing))
        {
            // 이미 있는 경우 → 개수만 증가
            existing.value += count;
        }
        else
        {
            // 처음 추가하는 경우 → 새 ItemSaveData 생성
            var newItem = new ItemSaveData
            {
                id = id,
                value = count,
                level = 1                     // 처음 추가 시 레벨 1로 초기화
            };
            dict[id] = newItem;
        }

        // tempItemLevels는 이제 거의 필요 없음 (ItemSaveData.level로 대체 중)
        // 하지만 전환기간 동안 안전장치로 유지한다면 아래 코드 유지
        var handler = ItemMergeHandler.instance;
        if (!handler.tempItemLevels.ContainsKey(id))
        {
            handler.tempItemLevels[id] = 1;
        }
    }

    private Dictionary<int, ItemSaveData> GetDict(ItemMergeHandler.ItemType type)
    {
        var dm = DataManager.Instance;
        if (dm == null) return null;

        return type switch
        {
            ItemMergeHandler.ItemType.Inventory  => dm.InventoryDict,
            ItemMergeHandler.ItemType.Companion  => dm.CompanionDict,
            ItemMergeHandler.ItemType.Skill      => dm.SkillDict,
            _ => null
        };
    }

    private void UpdateDisplay()
    {
        var dict = GetDict(itemType);
        currentCount = dict != null && dict.TryGetValue(testItemId, out ItemSaveData c) 
            ? c.value 
            : 0;

        currentLevel = ItemMergeHandler.instance.GetLevel(testItemId, itemType);

        Debug.Log($"[현재 상태] ID: {testItemId} / 개수: {currentCount} / 레벨: {currentLevel}");
    }

    // 인스펙터에서 버튼으로 호출하고 싶다면
    [ContextMenu("합성 1회 시도")]
    void ManualMerge() => ItemMergeHandler.instance.TryMerge(testItemId, itemType);

    [ContextMenu("아이템 50개 추가")]
    void Add50() => ForceAddItem(testItemId, 50, itemType);
}