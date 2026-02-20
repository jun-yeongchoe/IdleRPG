using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PartnerDataLoader : MonoBehaviour
{
    public static PartnerDataLoader Instance;

    private const string csvURL = "https://docs.google.com/spreadsheets/d/e/2PACX-1vRyDVW3msTQbFIC8alNBIsTJ54oBNKmlY1-RY5yF-xkKVKORx_-haqwvcTx9jHJfJ9ds5DbWlPsVigp/pub?gid=1209793167&single=true&output=csv";

    private Dictionary<int, PartnerData> partnerDict = new Dictionary<int, PartnerData>();
    public bool isDataLoaded = false;


    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    private IEnumerator Start()
    {
        yield return StartCoroutine(DownloadCSV());
    }

    private IEnumerator DownloadCSV()
    {
        using (UnityWebRequest www = UnityWebRequest.Get(csvURL))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"CSV 다운로드 실패: {www.error}");
            }
            else
            {
                ParseCSV(www.downloadHandler.text);
                isDataLoaded = true;
                Debug.Log("구글 시트로부터 동료 데이터 로드 완료!");
            }
        }
    }

    private void ParseCSV(string csvText)
    {
        string[] lines = csvText.Split(new[] { '\n', '\r' }, System.StringSplitOptions.RemoveEmptyEntries);

        // 첫 번째 줄(헤더) 제외
        for (int i = 1; i < lines.Length; i++)
        {
            string[] row = lines[i].Split(',');

            if (row.Length < 10) continue; // 데이터 열 개수가 부족하면 스킵

            PartnerData data = new PartnerData
            {
                ID = int.Parse(row[0]),
                Name_KR = row[1],
                Name_EN = row[2],
                Rank = row[3],
                Atk_Damage = float.Parse(row[4]),
                Atk_Speed = float.Parse(row[5]),
                Base_ComposeStat = float.Parse(row[6]),
                Stat_Per_Lv = float.Parse(row[7]),
                Prefab_Path = row[8],
                Description_KR = row[9]
            };

            if (!partnerDict.ContainsKey(data.ID))
                partnerDict.Add(data.ID, data);
        }
    }

    public PartnerData GetPartnerData(int id)
    {
        if (partnerDict.TryGetValue(id, out PartnerData data))
            return data;
        return null;
    }
}
