using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public class SPPointCSVLoader : MonoBehaviour
{
    private string csvUrl = "https://docs.google.com/spreadsheets/d/e/2PACX-1vRyDVW3msTQbFIC8alNBIsTJ54oBNKmlY1-RY5yF-xkKVKORx_-haqwvcTx9jHJfJ9ds5DbWlPsVigp/pub?gid=1869282746&single=true&output=csv";

    public Dictionary<string, List<SPPointData>> SPListByType = new Dictionary<string, List<SPPointData>>();

    void Start()
    {
        StartCoroutine(DownloadCSV());
    }

    IEnumerator DownloadCSV()
    {
        using(UnityWebRequest www = UnityWebRequest.Get(csvUrl))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("CSV 다운로드 실패: " + www.error);
            }
            else
            {
                ParseCSV(www.downloadHandler.text);
                Debug.Log("SP 포인트 데이터 로드 완료.");
            }
        }
    }

    private void ParseCSV(string text)
    {
        SPListByType.Clear();

        string[] lines = text.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

        for(int i = 1; i< lines.Length; i++)
        {
            string[] values = lines[i].Split(',');
            if(values.Length < 5) continue;

            int id = int.Parse(values[0]);
            string rank = values[1];
            string type = values[2];
            float rate = float.Parse(values[3]);
            bool isMultiply = values[4].ToLower() == "M".ToLower();

            SPPointData data = new SPPointData(id, rank, type, rate, isMultiply);
            if (!SPListByType.ContainsKey(data.Type))
            {
                SPListByType[data.Type] = new List<SPPointData>();
            }

            SPListByType[data.Type].Add(data);
        }
        Debug.Log("데이터 로드 완료"+ SPListByType.Count);
    }
}
