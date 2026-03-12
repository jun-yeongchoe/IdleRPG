using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Linq;
public class SPPointCSVLoader : MonoBehaviour
{
    private const string csvURL_Rank = "https://docs.google.com/spreadsheets/d/e/2PACX-1vRyDVW3msTQbFIC8alNBIsTJ54oBNKmlY1-RY5yF-xkKVKORx_-haqwvcTx9jHJfJ9ds5DbWlPsVigp/pub?gid=167356561&single=true&output=csv";
    private const string csvURL_Value = "https://docs.google.com/spreadsheets/d/e/2PACX-1vRyDVW3msTQbFIC8alNBIsTJ54oBNKmlY1-RY5yF-xkKVKORx_-haqwvcTx9jHJfJ9ds5DbWlPsVigp/pub?gid=1869282746&single=true&output=csv";
    private const string csvURL_Synergy = "https://docs.google.com/spreadsheets/d/e/2PACX-1vRyDVW3msTQbFIC8alNBIsTJ54oBNKmlY1-RY5yF-xkKVKORx_-haqwvcTx9jHJfJ9ds5DbWlPsVigp/pub?gid=433299219&single=true&output=csv";

    [Header("Data Tables")]
    public List<SPRankData> rankTable = new List<SPRankData>();
    public List<SPData> valueTable = new List<SPData>();
    public List<SynergyEffectData> synergyTable = new List<SynergyEffectData>();

    public bool isLoaded = false;

    void Start()
    {
        StartCoroutine(LoadAllSPData());
    }

    IEnumerator LoadAllSPData()
    {
        // 3개의 CSV를 순차적으로 로드
        yield return StartCoroutine(DownloadCSV(csvURL_Rank, ParseRankData));
        yield return StartCoroutine(DownloadCSV(csvURL_Value, ParseValueData));
        yield return StartCoroutine(DownloadCSV(csvURL_Synergy, ParseSynergyData));

        isLoaded = true;
        Debug.Log($"[SPLoader] 모든 데이터 로드 완료. (Rank:{rankTable.Count}, Value:{valueTable.Count}, Synergy:{synergyTable.Count})");
    }

    IEnumerator DownloadCSV(string url, Action<string> onComplete)
    {
        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"CSV 다운로드 실패 ({url}): " + www.error);
            }
            else
            {
                onComplete?.Invoke(www.downloadHandler.text);
            }
        }
    }

    #region Parsing Logic
    private void ParseRankData(string text)
    {
        rankTable.Clear();
        string[] lines = text.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
        for (int i = 1; i < lines.Length; i++)
        {
            string[] cols = lines[i].Split(',');
            if (cols.Length < 2) continue;
            rankTable.Add(new SPRankData { rankName = cols[0].Trim(), value = float.Parse(cols[1].Trim()) });
        }
    }

    private void ParseValueData(string text)
    {
        valueTable.Clear();
        string[] lines = text.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
        for (int i = 1; i < lines.Length; i++)
        {
            string[] cols = lines[i].Split(',');
            if (cols.Length < 4) continue;
            valueTable.Add(new SPData {
                id = int.Parse(cols[0].Trim()),
                rank = cols[1].Trim(),
                type = cols[2].Trim(),
                rate = float.Parse(cols[3].Trim())
            });
        }
    }

    private void ParseSynergyData(string text)
    {
        synergyTable.Clear();
        string[] lines = text.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
        for (int i = 1; i < lines.Length; i++)
        {
            string[] cols = lines[i].Split(',');
            if (cols.Length < 5) continue;
            synergyTable.Add(new SynergyEffectData {
                synergyName = cols[1].Trim().ToLower(),
                requiredCount = int.Parse(cols[2].Trim()),
                refStatID = int.Parse(cols[3].Trim()),
                effectValue = float.Parse(cols[4].Trim())
            });
        }
    }
    #endregion

    #region Helper Methods (데이터를 쉽게 가져오는 함수들)
    public string GetRandomRank()
    {
        if (rankTable.Count == 0) return "F";
        float total = rankTable.Sum(x => x.value);
        float randomValue = UnityEngine.Random.Range(0, total);
        float cumulative = 0;

        foreach (var res in rankTable)
        {
            cumulative += res.value;
            if (randomValue <= cumulative) return res.rankName;
        }
        return rankTable[0].rankName;
    }

    public SPData GetRandomSPByRank(string rankName)
    {
        var candidates = valueTable.FindAll(x => x.rank == rankName);
        if (candidates.Count == 0) return null;
        return candidates[UnityEngine.Random.Range(0, candidates.Count)];
    }

    public SPData GetSPDataByID(int id)
    {
        return valueTable.Find(x => x.id == id);
    }

    public float GetSynergyEffect(string name, int required)
    {   
        float synergyEffect = 0;
        foreach(var data in synergyTable)
        {
            if(data.synergyName == name && data.requiredCount == required)
            {
                synergyEffect = data.effectValue;
                if(synergyEffect > 0) break;
            }
        }
        return synergyEffect;
    }
    #endregion
}
