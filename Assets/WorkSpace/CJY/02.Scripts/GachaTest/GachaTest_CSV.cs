using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GachaTest_CSV : MonoBehaviour
{
    [Header("CSV")]
    private string csvUrl = "https://docs.google.com/spreadsheets/d/1nBD0pWXwxWAM0WZP5OcrImMrh5f65wtb0LtI2C-qG9M/export?format=csv&gid=1032686388";

    [Header("UI")]
    public TMP_Dropdown levelChoose;
    public TextMeshProUGUI[] result;
    public Button summon11Btn, summon35Btn;
    private Dictionary<string, float> currentRates = new Dictionary<string, float>();
    private string[] rankKeys = { "Common", "Uncommon", "Rare", "Epic", "Legendary", "Mythic", "Celestial" };

    public List<GachaData_CSV> gachaRates = new List<GachaData_CSV>();

    void Start()
    {
        

        levelChoose.ClearOptions();

        List<string> options = new List<string>();
        for(int i = 1; i <= 20; i++)
        {
            options.Add(i.ToString());
        }

        levelChoose.AddOptions(options);

        summon11Btn.onClick.AddListener(() => DoSummon(11));
        summon35Btn.onClick.AddListener(() => DoSummon(35));

        StartCoroutine(DownloadCSV());
        levelChoose.onValueChanged.AddListener
        (
        delegate 
        { 
            StartCoroutine(LoadRatesFromCSV()); 
        }
        );
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
        
        GachaData_CSV select = gachaRates[levelChoose.value];
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

        int[] results = new int[7];

        for(int i = 0; i<count; i++)
        {
            int idx = GetRandRankIdx();
            results[idx]++;
        }
        for(int i = 0; i< result.Length; i++)
        {
            result[i].text = results[i].ToString();
        }
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

    public void OnClickBackStatusWindow()
    {
        SceneManager.LoadScene("LoginTest");
    }
}
