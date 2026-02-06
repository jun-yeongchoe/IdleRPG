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
        string[] lines = csvText.Split('\n');
        playerStatDataList.Clear();

        for(int i = 1; i < lines.Length; i++)
        {
            if(string.IsNullOrWhiteSpace(lines[i]))
                continue;

            string[] values = lines[i].Split(',');

            PlayerStatData_CSV data = new PlayerStatData_CSV
            {
                ID = int.Parse(values[0]),
                StatName = values[1].Trim(),
                BaseValue = float.Parse(values[2]),
                GrowthPerLevel = float.Parse(values[3]),
                StartCost = float.Parse(values[4]),
                CostGrowthRate = float.Parse(values[5]),
                LimitLevel = int.Parse(values[6]),
                UnlockCondition = values[7].Trim() == "" ? 0 : int.Parse(values[7]),
                UnlockLevel = int.Parse(values[8])
            };

            playerStatDataList.Add(data);
        }
    }

    public PlayerStatData_CSV GetStat(string statName)
    {
        return playerStatDataList.Find(x => x.StatName == statName);
    }
}
