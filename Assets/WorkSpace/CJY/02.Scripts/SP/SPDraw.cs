using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Linq;

[Serializable]
public class SynergyEffectData
{
    public string synergyName; // ghost, vampire 등
    public int requiredCount;  // 요구 세트 개수 (2, 4 등)
    public float effectValue;  // 증가 수치
    public int refStatID;      // 적용될 Stat ID (101, 102 등)
}

[Serializable]
public class SynergyIconData
{
    public string synergyName; // ghost, vampire, hydra, devil
    public Sprite synergySprite;
    public Vector3 color;
}

[Serializable]
public class SPData // 특성 값 CSV
{
    public int id;
    public string rank;
    public string type;
    public float rate;
}

[Serializable]
public class SPRankData
{
    public string rankName; // 랭크
    public float value; // 등장 확률
}

public class SPDraw : MonoBehaviour
{
    public const string csvURL_Rank = "https://docs.google.com/spreadsheets/d/e/2PACX-1vRyDVW3msTQbFIC8alNBIsTJ54oBNKmlY1-RY5yF-xkKVKORx_-haqwvcTx9jHJfJ9ds5DbWlPsVigp/pub?gid=167356561&single=true&output=csv";
    public const string csvURL_Value = "https://docs.google.com/spreadsheets/d/e/2PACX-1vRyDVW3msTQbFIC8alNBIsTJ54oBNKmlY1-RY5yF-xkKVKORx_-haqwvcTx9jHJfJ9ds5DbWlPsVigp/pub?gid=1869282746&single=true&output=csv";

    public const string csvURL_Synergy = "https://docs.google.com/spreadsheets/d/e/2PACX-1vRyDVW3msTQbFIC8alNBIsTJ54oBNKmlY1-RY5yF-xkKVKORx_-haqwvcTx9jHJfJ9ds5DbWlPsVigp/pub?gid=433299219&single=true&output=csv";


    [Header("SP settings")]
    public List<SPRankData> rankTable = new List<SPRankData>();
    public List<SPData> valueTable = new List<SPData>();
    public int drawCost = 5;
    public int CurrentTotalCost // 잠긴 슬롯 갯수에 따라 비용을 계산하는 속성
    {
        get
        {
            int lockCount = 0;
            foreach (var slot in spSlots)
            {
                if (slot.isLocked) lockCount++;
            }
            // 잠긴 개수 0개면 5, 1개면 10... (잠긴수 + 1) * 5
            return (lockCount + 1) * drawCost;
        }
    }

    [Header("Auto Draw Settings")]
    [SerializeField] private Button autoDrawButton;
    [SerializeField] private TextMeshProUGUI autoDrawButtonText; // "Auto Draw" / "Stop" 텍스트 변경용
    private bool isAutoDrawing = false;
    private Coroutine autoDrawCoroutine;
    [SerializeField] private float autoDelay = 0.2f; // 자동 뽑기 간격


    [Header("Scrap")]
    [SerializeField] TextMeshProUGUI scrapText, oneDrawScrap;

    [Header("Slot UI Contorl")]
    [SerializeField] private List<SPSlotUI> spSlots = new List<SPSlotUI>();
    [SerializeField] private Button drawButton;


    [Header("Synergy Data")]
    public List<SynergyEffectData> synergyTable = new List<SynergyEffectData>();
    public string[] synergyNames = { "ghost", "vampire", "hydra", "devil" };

    [Header("Synergy Display")]
    [SerializeField] private List<SynergyIconData> synergyIcons = new List<SynergyIconData>();
    [Header("Synergy Level UI")]
    [SerializeField] private List<SPSynergyLevelUI> synergyLevelUIs = new List<SPSynergyLevelUI>();
    

    void Start()
    {
        DataManager.Instance.Scrap = 1000;
        if(EventManager.Instance != null) EventManager.Instance.StartList("CurrencyChange", RefreshScrapUI);
        oneDrawScrap.text = drawCost.ToString();
        StartCoroutine(Init());
    }

    IEnumerator Init()
    {
        yield return StartCoroutine(DownloadCSV(csvURL_Rank, ParseRankProbData));
        yield return StartCoroutine(DownloadCSV(csvURL_Value, ParseRankValueData));
        yield return StartCoroutine(DownloadCSV(csvURL_Synergy, ParseSynergyData));

        for(int i = 0; i < synergyLevelUIs.Count; i++)
        {
            string sName = synergyNames[i];
            synergyLevelUIs[i].Init(sName);
        }
    }

    IEnumerator DownloadCSV(string url, Action<string> onComplete)
    {
        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            yield return www.SendWebRequest();
            if (www.result == UnityWebRequest.Result.Success)
                onComplete?.Invoke(www.downloadHandler.text);
            else
                Debug.LogError($"CSV 로드 실패: {www.error}");
        }
    }

    #region Parsing
    private void ParseRankProbData(string csvData)
    {
        rankTable.Clear();
        string[] lines = csvData.Split('\n');
        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;
            string[] cols = lines[i].Split(',');
            rankTable.Add(new SPRankData { rankName = cols[0].Trim(), value = float.Parse(cols[1].Trim()) });
        }
    }

    private void ParseRankValueData(string csvData)
    {
        valueTable.Clear();
        string[] lines = csvData.Split('\n');
        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;
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

    private void ParseSynergyData(string csvData)
    {
        synergyTable.Clear();
        string[] lines = csvData.Split('\n');
        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;
            string[] cols = lines[i].Split(',');
            
            // 시트 구조에 맞게 인덱스 조절 (Name, ReqCount, Value, RefID 순이라 가정)
            synergyTable.Add(new SynergyEffectData {
                synergyName = cols[1].Trim().ToLower(),
                requiredCount = int.Parse(cols[2].Trim()),
                effectValue = float.Parse(cols[4].Trim()),
                refStatID = int.Parse(cols[3].Trim()) // Ref ID 열 번호 확인 필요
            });
        }
    }
    #endregion


    public string GetRandomRank()
    {
        if (rankTable.Count == 0) return "F";

        float total = 0;
        foreach (var res in rankTable) total += res.value;

        float randomValue = UnityEngine.Random.Range(0, total);
        float cumulative = 0;

        foreach (var res in rankTable)
        {
            cumulative += res.value;
            if (randomValue <= cumulative)
            {
                return res.rankName;
            }
        }
        return rankTable[0].rankName;
    }

    public SPData GetFinalSP(string rankName)
    {
        List<SPData> candidates = valueTable.FindAll(x => x.rank == rankName);
        if(candidates.Count == 0) return null;

        int randIdx = UnityEngine.Random.Range(0, candidates.Count);
        return candidates[randIdx];
    }

    public void OnClickSPChange()
    {
        int lockCount = 0;
        foreach (var slot in spSlots) if (slot.isLocked) lockCount++;

        if (lockCount >= spSlots.Count)
        {
            Debug.Log("모든 슬롯이 잠겨있어 변경할 수 없습니다.");
            return;
        }

        int finalCost = CurrentTotalCost;

        if (DataManager.Instance.Scrap < finalCost)
        {
            Debug.Log("Scrap이 부족합니다!");
            return;
        }

        // 재화 차감 (AddScrap 함수 활용)
        DataManager.Instance.AddScrap(-finalCost);

        foreach (var slot in spSlots)
        {
            if (slot.isLocked) continue;

            string randRank = GetRandomRank();
            SPData result = GetFinalSP(randRank);

            string randSynergy = synergyNames[UnityEngine.Random.Range(0, synergyNames.Length)];
            SynergyIconData icon = GetSynergyData(randSynergy);

            if (result != null && icon != null)
        {
            // Vector3(x, y, z)를 Color(r, g, b)로 변환
            // 시트나 인스펙터 값이 0~255라면 255f로 나누어야 하고, 0~1 사이라면 그대로 사용합니다.
            Color synergyColor = new Color(
                                icon.color.x / 255f, 
                                icon.color.y / 255f, 
                                icon.color.z / 255f, 
                                1f
                            );

            slot.UpdateSlotUI(result, randSynergy, icon.synergySprite, synergyColor);
        }
        }

        CalcTotalSynergy();
    }

    private void CalcTotalSynergy()
    {
        // 1. 카운트 초기화
        Dictionary<string, int> counts = new Dictionary<string, int>();
        foreach (var name in synergyNames) counts[name] = 0;

        // 2. 현재 슬롯의 시너지 개수 측정
        foreach (var slot in spSlots)
        {
            if (!string.IsNullOrEmpty(slot.currentSynergy))
            {
                string sName = slot.currentSynergy.ToLower();
                if (counts.ContainsKey(sName)) counts[sName]++;
            }
        }

        // 3. 상단 레벨 UI 업데이트 (LV 0, LV 3 등)
        foreach (var ui in synergyLevelUIs)
        {
            if (counts.ContainsKey(ui.synergyName.ToLower()))
            {
                ui.UpdateLevel(counts[ui.synergyName.ToLower()]);
            }
        }

        // 4. [수정] 최상위 세트 효과만 로그 출력
        Debug.Log("<color=cyan>------ 활성화된 최상위 시너지 효과 ------</color>");
        foreach (var name in counts.Keys)
        {
            int currentCount = counts[name];
            if (currentCount <= 0) continue;

            // 해당 시너지의 효과들 중 (보유 개수 >= 요구 개수)인 것들을 필터링하고, 
            // 그 중 requiredCount가 가장 큰 놈 딱 하나만 가져옵니다.
            var bestEffect = synergyTable
                .Where(fx => fx.synergyName == name && currentCount >= fx.requiredCount)
                .OrderByDescending(fx => fx.requiredCount)
                .FirstOrDefault();

            if (bestEffect != null)
            {
                Debug.Log($"<color=white>[{name}]</color> {bestEffect.requiredCount}세트 효과 적용 중! " +
                        $"(ID: {bestEffect.refStatID}, Value: {bestEffect.effectValue})");
            }
        }
    }

    public SynergyIconData GetSynergyData(string sName)
{
    return synergyIcons.Find(x => x.synergyName.ToLower() == sName.ToLower());
}

    private void RefreshScrapUI()
    {
        if(scrapText != null && DataManager.Instance != null) scrapText.text = DataManager.Instance.Scrap.ToString();
    }

    public void UpdateDrawCostUI()
    {
        int lockCount = 0;
        foreach (var slot in spSlots) if (slot.isLocked) lockCount++;

        if (drawButton != null)
        {
            drawButton.interactable = (lockCount < spSlots.Count);
        }

        // 5개 모두 잠긴 경우 표시 처리
        if (lockCount >= spSlots.Count)
        {
            oneDrawScrap.text = "--"; 
        }
        else
        {
            oneDrawScrap.text = CurrentTotalCost.ToString();
        }
    }

    private void OnDestroy()
    {
        if (EventManager.Instance != null)
        {
            EventManager.Instance.StopList("CurrencyChange", RefreshScrapUI);
        }
    }


    //AutoDraw
    public void OnClickAutoDraw()
    {
        if (isAutoDrawing)
        {
            StopAutoDraw();
        }
        else
        {
            StartAutoDraw();
        }
    }

    private void StartAutoDraw()
    {
        // 잠금 개수 체크 (모두 잠겨있으면 시작 안 함)
        int lockCount = 0;
        foreach (var slot in spSlots) if (slot.isLocked) lockCount++;
        if (lockCount >= spSlots.Count) return;

        isAutoDrawing = true;
        if (autoDrawButtonText != null) autoDrawButtonText.text = "STOP";
        
        // 일반 뽑기 버튼 비활성화 (선택 사항)
        drawButton.interactable = false;

        autoDrawCoroutine = StartCoroutine(AutoDrawRoutine());
    }

    private void StopAutoDraw()
    {
        isAutoDrawing = false;
        if (autoDrawButtonText != null) autoDrawButtonText.text = "AUTO DRAW";
        
        // 일반 뽑기 버튼 복구
        UpdateDrawCostUI();

        if (autoDrawCoroutine != null)
        {
            StopCoroutine(autoDrawCoroutine);
            autoDrawCoroutine = null;
        }
    }

    private IEnumerator AutoDrawRoutine()
    {
        while (isAutoDrawing)
        {
            int finalCost = CurrentTotalCost;

            // 1. 재화 부족 체크
            if (DataManager.Instance.Scrap < finalCost)
            {
                Debug.Log("Scrap 부족으로 자동 뽑기를 중단합니다.");
                StopAutoDraw();
                yield break;
            }

            // 2. 뽑기 실행 (OnClickSPChange의 핵심 로직과 동일)
            DataManager.Instance.AddScrap(-finalCost);

            foreach (var slot in spSlots)
            {
                if (slot.isLocked) continue;

                string randRank = GetRandomRank();
                SPData result = GetFinalSP(randRank);
                string randSynergy = synergyNames[UnityEngine.Random.Range(0, synergyNames.Length)];
                SynergyIconData iconData = GetSynergyData(randSynergy);

                if (result != null && iconData != null)
                {
                    Color synergyColor = new Color(
                        iconData.color.x / 255f, 
                        iconData.color.y / 255f, 
                        iconData.color.z / 255f, 
                        1f
                    );
                    slot.UpdateSlotUI(result, randSynergy, iconData.synergySprite, synergyColor);
                }
            }

            CalcTotalSynergy();

            // 3. 딜레이 대기
            yield return new WaitForSeconds(autoDelay);
        }
    }


    // 추후 DataManger와 연동

    

}
