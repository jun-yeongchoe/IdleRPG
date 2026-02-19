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

        if (dict.ContainsKey(id))
            dict[id] += count;
        else
            dict[id] = count;

        // level은 처음이라면 1로 초기화 (임시)
        if (!ItemMergeHandler.instance.tempItemLevels.ContainsKey(id))
            ItemMergeHandler.instance.tempItemLevels[id] = 1;
    }

    private Dictionary<int, int> GetDict(ItemMergeHandler.ItemType type)
    {
        var dm = DataManager.Instance;
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
        currentCount = dict != null && dict.TryGetValue(testItemId, out int c) ? c : 0;
        currentLevel = ItemMergeHandler.instance.GetLevel(testItemId, itemType);

        Debug.Log($"[현재 상태] ID: {testItemId} / 개수: {currentCount} / 레벨: {currentLevel}");
    }

    // 인스펙터에서 버튼으로 호출하고 싶다면
    [ContextMenu("합성 1회 시도")]
    void ManualMerge() => ItemMergeHandler.instance.TryMerge(testItemId, itemType);

    [ContextMenu("아이템 50개 추가")]
    void Add50() => ForceAddItem(testItemId, 50, itemType);
}
