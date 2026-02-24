using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GachaShop : MonoBehaviour
{
    [Header("CSV")]
    private string csvUrl = "https://docs.google.com/spreadsheets/d/1nBD0pWXwxWAM0WZP5OcrImMrh5f65wtb0LtI2C-qG9M/export?format=csv&gid=1032686388";

    [Header("UI")]
    public Button summon11Btn, summon35Btn;
    private Dictionary<string, float> currentRates = new Dictionary<string, float>();
    private string[] rankKeys = { "Common", "Uncommon", "Rare", "Epic", "Legendary", "Mythic", "Celestial" };

    public List<GachaData_CSV> gachaRates = new List<GachaData_CSV>();

    [Header("Shop Settings")]
    public int shopIndex = 0;
    public static int currentShopIndex = -1;
    private const int BASE_EXP = 35;
    private string[] shopNames = {"Equip", "Skill", "Partner"};
    
    [Header("Item List")]
    [SerializeField] private GameObject[] common;
    [SerializeField] private GameObject[] uncommon;
    [SerializeField] private GameObject[] rare;
    [SerializeField] private GameObject[] epic;
    [SerializeField] private GameObject[] legendary;
    [SerializeField] private GameObject[] mythic;
    [SerializeField] private GameObject[] celestial;

    private GameObject[][] itemLists;

    [Header("Result")]
    [SerializeField] private GameObject resultPanel;
    [SerializeField] private Transform resultContent;

    [Header("Gem")]
    [SerializeField] private TextMeshProUGUI gemText;


    void Awake()
    {
        string objName = gameObject.name;
        if(currentShopIndex == -1)
        {
            if(objName.Contains(shopNames[0])) shopIndex = 0;
            else if(objName.Contains(shopNames[1])) shopIndex = 1;
            else if(objName.Contains(shopNames[2])) shopIndex = 2;
        }
        DataManager.Instance.ShopLevels[0] = 15;
        DataManager.Instance.Gem = 100000;
    }

    void Start()
    {
        if(EventManager.Instance != null) EventManager.Instance.StartList("CurrencyChange", RefreshGemUI);

        RefreshGemUI();

        List<string> options = new List<string>();
        for(int i = 1; i <= 20; i++)
        {
            options.Add(i.ToString());
        }

        itemLists = new GameObject[][] { common, uncommon, rare, epic, legendary, mythic, celestial };
        summon11Btn.onClick.AddListener(() => DoSummon(11));
        summon35Btn.onClick.AddListener(() => DoSummon(35));

        StartCoroutine(DownloadCSV());
    }

    private void RefreshGemUI()
    {
        if(gemText != null && DataManager.Instance != null) gemText.text = DataManager.Instance.Gem.ToString();
    }

    IEnumerator DownloadCSV()
    {
        using (UnityWebRequest www = UnityWebRequest.Get(csvUrl))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("CSV 다운로드 실패: " + www.error);
            }
            else
            {
                string csvData = www.downloadHandler.text;
                ParseCSVData(csvData);
                StartCoroutine(LoadRatesFromCSV());
            }
        }
    }

    private void ParseCSVData(string csvData)
    {
        string[] lines = csvData.Split('\n');
        gachaRates.Clear();

        for(int i = 1; i < lines.Length; i++)
        {
            if(string.IsNullOrWhiteSpace(lines[i])) continue;

            string[] values = lines[i].Split(',');

            GachaData_CSV entry = new GachaData_CSV();
            entry.common = float.Parse(values[1]);
            entry.uncommon = float.Parse(values[2]);
            entry.rare = float.Parse(values[3]);
            entry.epic = float.Parse(values[4]);
            entry.legendary = float.Parse(values[5]);
            entry.mythic = float.Parse(values[6]);
            entry.celestial = float.Parse(values[7]);

            gachaRates.Add(entry);

        }

    }

    IEnumerator LoadRatesFromCSV()
    {
        if(DataManager.Instance == null) yield break;

        int currentShopLevel = DataManager.Instance.ShopLevels[shopIndex];
        int rateIndex = Mathf.Clamp(currentShopLevel -1, 0, gachaRates.Count - 1);

        GachaData_CSV select = gachaRates[rateIndex];
        if (select == null)
        {
            Debug.Log("로드 실패");
        }
        else
        {
            currentRates.Clear();
            currentRates.Add("Common", select.common);
            currentRates.Add("Uncommon", select.uncommon);
            currentRates.Add("Rare", select.rare);
            currentRates.Add("Epic", select.epic);
            currentRates.Add("Legendary", select.legendary);
            currentRates.Add("Mythic", select.mythic);
            currentRates.Add("Celestial", select.celestial);
        }
        
        yield return null;
        
    }

    private void DoSummon(int count)
    {
        if(currentRates.Count == 0)return;
        currentShopIndex = this.shopIndex;

        int cost = (count == 11)? 500 : 1500;
        if(DataManager.Instance.Gem < cost)
        {
            Debug.Log("잔액이 부족합니다.");
            return;
        }

        DataManager.Instance.AddGem(-cost);
        
        if(!resultPanel.activeSelf) resultPanel.SetActive(true);
        foreach(Transform child in resultContent)
        {
            Destroy(child.gameObject);
        }

        int[] results = new int[7];
        List<int> rolledIndices = new List<int>();

        for(int i = 0; i<count; i++)
        {
            int idx = GetRandRankIdx();
            results[idx]++;
            rolledIndices.Add(idx);
        }
        for(int i = 0; i< rankKeys.Length; i++)
        {
            Debug.Log($"{rankKeys[i]} 등급 획득 개수: {results[i]}");
        }
        DisplayResult(rolledIndices);
        // GachaShop.cs 의 DoSummon 메서드 내부 하단

        AddShopExp(count);

        // 1. 자신의 패널 UI 갱신
        GachaShop_Display myDisplay = GetComponent<GachaShop_Display>();
        if (myDisplay != null) myDisplay.UpdateDisplay();

        // 2. 결과창(Panel_Result)의 UI도 실시간 갱신
        // Panel_Result 객체에 GachaShop_Display 스크립트가 붙어있어야 합니다.
        GachaShop_Display resultDisplay = resultPanel.GetComponent<GachaShop_Display>();
        if (resultDisplay != null) resultDisplay.UpdateDisplay();
    }

    private void DisplayResult(List<int> rolledIndices)
    {
        if(resultContent == null) return;

        foreach(int gradeIdx in rolledIndices)
        {
            GameObject[] targetList = itemLists[gradeIdx];
            if(targetList != null && targetList.Length > 0)
            {
                int randItemIdx = Random.Range(0, targetList.Length);
                GameObject prefab = targetList[randItemIdx];

                if(prefab != null)
                {
                    Instantiate(prefab, resultContent);
                }
            }
        }
    }

    private void AddShopExp(int count)
    {
        if(DataManager.Instance == null) return;

        int currentLevel = DataManager.Instance.ShopLevels[shopIndex];
        int currentExp = DataManager.Instance.ShopExps[shopIndex];

        currentExp += count;

        bool isLevelUp = false;

        while(true)
        {
            int requiredExp = BASE_EXP * (int)Mathf.Pow(2, currentLevel -1);

            if(currentExp >= requiredExp)
            {
                currentExp -= requiredExp;
                currentLevel++;
                isLevelUp = true;
                Debug.Log($"<color = yellow>[Shop Level up]</color> Level{currentLevel-1} => {currentLevel}");

            }    
            else break;
        }

        DataManager.Instance.ShopLevels[shopIndex] = currentLevel;
        DataManager.Instance.ShopExps[shopIndex] = currentExp;

        if (isLevelUp)
        {
            StartCoroutine(LoadRatesFromCSV());
        }

        Debug.Log($"현재 상점 레벨: {currentLevel}, 경험치: {currentExp}");
    }

    private int GetRandRankIdx()
    {
        float totalWeight = 0;

        foreach(var rate in currentRates.Values) totalWeight += rate;

        float randValue = Random.Range(0, totalWeight);
        float currentWeight = 0;

        for(int i = 0; i< rankKeys.Length; i++)
        {
            currentWeight += currentRates[rankKeys[i]];
            if(randValue <= currentWeight) return i;

        }

        return 0;

    }

    public void OnClickResultRetry(int count)
    {
        GachaShop[] allShops = FindObjectsOfType<GachaShop>();

        foreach (var shop in allShops)
        {
            
            if (shop.gameObject.name.Contains("Result")) continue;

            if (shop.shopIndex == currentShopIndex)
            {
                shop.DoSummon(count);
                return;
            }
        }
    }

    private void OnDestroy()
    {
        if (EventManager.Instance != null)
        {
            EventManager.Instance.StopList("CurrencyChange", RefreshGemUI);
        }
    }

}
