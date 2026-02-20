using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Numerics;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance;

    public enum ShopType { Equipment = 0, Skill = 1, Companion = 2 }

    [System.Serializable]
    public class GachaProbability
    {
        public int level;
        public float[] probs = new float[7];
    }

    [Header("확률 테이블 (인스펙터에서 1~20레벨 세팅)")]
    public List<GachaProbability> probabilityTable;

    private const int COST_11 = 500;
    private const int COST_35 = 1500;
    private const int MAX_SHOP_LEVEL = 20;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public int GetMaxExp(int level)
    {
        if (level >= MAX_SHOP_LEVEL) return -1;
        return 35 * (int)Mathf.Pow(2, level - 1);
    }

    //뽑기 함수
    public void Summon(ShopType type, int count)
    {
        BigInteger cost = (count == 11) ? COST_11 : COST_35;

        if (DataManager.Instance.Gem < cost)
        {
            CommonPopup.Instance.ShowAlert("보석 부족", "보석이 부족합니다!");
            return;
        }

        DataManager.Instance.Gem -= cost;
        EventManager.Instance.TriggerEvent("CurrencyChange");

        //현재 상점 레벨에 따른 확률표 가져오기
        int typeIndex = (int)type;
        int currentLevel = DataManager.Instance.ShopLevels[typeIndex];

        int tableIndex = Mathf.Clamp(currentLevel - 1, 0, probabilityTable.Count - 1);
        float[] currentProbs = probabilityTable[tableIndex].probs;

        //뽑기 진행(결과를 Dictionary에 모아둠: Key=등급(ID), Value=개수)
        Dictionary<int, int> results = new Dictionary<int, int>();

        for (int i = 0; i < count; i++)
        {
            int grade = GetRandomGrade(currentProbs);

            int itemID = GetIDByGrade(type, grade);

            if (results.ContainsKey(itemID)) results[itemID]++;
            else results.Add(itemID, 1);
        }

        Debug.Log($"[{type}] {count}회 뽑기 완료! 경험치 +{count} 증가");

        if (GachaResultPopup.Instance != null)
        {
            GachaResultPopup.Instance.ShowResult(results);
        }
    }

    private int GetIDByGrade(ShopType type, int grade)
    {
        if (type == ShopType.Equipment)
        {
            int[] categoryBases = new int[] { 1000, 4000, 7000 };
            int selectedCategory = categoryBases[Random.Range(0, categoryBases.Length)];

            int gradeOffset = grade * 400;
            int itemIndex = Random.Range(1, 4); //1, 2, 3 중 하나

            return selectedCategory + gradeOffset + itemIndex;
        }
        //스킬, 동료는 나중에 공식 나오면 수정하기
        else if (type == ShopType.Skill) return 20000 + (grade * 100) + Random.Range(1, 4);
        else if (type == ShopType.Companion) return 30000 + (grade * 100) + Random.Range(1, 4);

        return 0;
    }

    private int GetRandomGrade(float[] probs)
    {
        float totalWeight = 0f;
        foreach (float p in probs) totalWeight += p;

        float randomValue = Random.Range(0, totalWeight);
        float currentSum = 0f;

        for (int i = 0; i < probs.Length; i++)
        {
            currentSum += probs[i];
            if (randomValue <= currentSum) return i;
        }
        return 0;
    }

    private void AddShopExp(int typeIndex, int amount)
    {
        if (DataManager.Instance.ShopLevels[typeIndex] >= MAX_SHOP_LEVEL) return;

        DataManager.Instance.ShopExps[typeIndex] += amount;

        while (true)
        {
            int currentLv = DataManager.Instance.ShopLevels[typeIndex];
            if (currentLv >= MAX_SHOP_LEVEL) break;

            int maxExp = GetMaxExp(currentLv);

            if (DataManager.Instance.ShopExps[typeIndex] >= maxExp)
            {
                DataManager.Instance.ShopExps[typeIndex] -= maxExp;
                DataManager.Instance.ShopLevels[typeIndex]++;
            }
            else
            {
                break;
            }
        }
    }

    private void AddItemToData(ShopType type, int itemId, int count)
    {
        Dictionary<int, ItemSaveData> targetDict = null;

        switch (type)
        {
            case ShopType.Equipment: targetDict = DataManager.Instance.InventoryDict; break;
            case ShopType.Skill: targetDict = DataManager.Instance.SkillDict; break;
            case ShopType.Companion: targetDict = DataManager.Instance.CompanionDict; break;
        }

        if (targetDict.ContainsKey(itemId))
        {
            targetDict[itemId].value += count;
        }
        else
        {
            //처음 뽑은 아이템이면 레벨을 0으로
            targetDict.Add(itemId, new ItemSaveData { id = itemId, value = count, level = 0 });
        }
    }
}
