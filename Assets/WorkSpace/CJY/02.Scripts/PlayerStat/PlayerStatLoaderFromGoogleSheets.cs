using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerStatLoaderFromGoogleSheets : MonoBehaviour
{
    private string csvUrl = "https://docs.google.com/spreadsheets/d/1nBD0pWXwxWAM0WZP5OcrImMrh5f65wtb0LtI2C-qG9M/export?format=csv&gid=286621512";

    public List<PlayerStatData_CSV> playerStatDataList = new List<PlayerStatData_CSV>();
    public bool isLoaded = false;

    void Start()
    {
        StartCoroutine(DownloadCSV());
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
                ParseCSV(www.downloadHandler.text);
                isLoaded = true;
                Debug.Log("플레이어 스탯 데이터 로드 완료. 총 항목 수: " + playerStatDataList.Count);

            }
        }
    }

    void ParseCSV(string csvText)
    {
        string removedText = csvText.Replace("\r", "");
        string[] lines = removedText.Split('\n');
        playerStatDataList.Clear();

        for(int i = 1; i < lines.Length; i++)
        {
            if(string.IsNullOrWhiteSpace(lines[i]))
                continue;

            string[] values = lines[i].Split(',');

            foreach(var v in values)
            {
                Debug.Log(v);
            }

            PlayerStatData_CSV stat = new PlayerStatData_CSV();
            stat.ID = ParseIntOnly(values[0]);
            stat.StatName = values[1];
            stat.BaseValue = float.Parse(values[2]);
            stat.GrowthPerLevel = float.Parse(values[3]);
            stat.StartCost = float.Parse(values[4]);
            stat.CostGrowthRate = float.Parse(values[5]);

            stat.LimitLevel = ParseIntOnly(values[6]);
            stat.UnlockCondition = ParseIntOnly(values[7]);
            stat.UnlockLevel = ParseIntOnly(values[8]);

            playerStatDataList.Add(stat);
        }
    }

    int ParseIntOnly(string s)
{
    if (string.IsNullOrWhiteSpace(s)) return 0;
    
    // 숫자 이외의 모든 문자(공백, \r, \n 등)를 제거합니다.
    string digitsOnly = System.Text.RegularExpressions.Regex.Replace(s, @"[^\d]", "");
    
    if (int.TryParse(digitsOnly, out int result))
    {
        return result;
    }
    return 0;
}

    public PlayerStatData_CSV GetStat(string statName)
    {
        return playerStatDataList.Find(x => x.StatName == statName);
    }
}
