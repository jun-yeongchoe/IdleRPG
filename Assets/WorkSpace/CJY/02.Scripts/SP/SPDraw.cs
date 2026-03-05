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
    [Header("Auto Draw Settings")]
    [SerializeField] private Button autoDrawButton;
    [SerializeField] private TextMeshProUGUI autoDrawButtonText;
    private bool isAutoDrawing = false;
    private Coroutine autoDrawCoroutine;
    [SerializeField] private float autoDelay = 0.2f;

    [Header("Scrap UI")]
    [SerializeField] TextMeshProUGUI scrapText, oneDrawScrap;

    [Header("Slot UI Control")]
    public List<SPSlotUI> spSlots = new List<SPSlotUI>();
    [SerializeField] private Button drawButton;

    [Header("Synergy Display")]
    public string[] synergyNames = { "ghost", "vampire", "hydra", "devil" };
    [SerializeField] private List<SynergyIconData> synergyIcons = new List<SynergyIconData>();
    [SerializeField] private List<SPSynergyLevelUI> synergyLevelUIs = new List<SPSynergyLevelUI>();

    // 데이터 로더 참조
    private SPPointCSVLoader Loader => CSV_LoadManager.Instance.SP_CSV;
    
    private SPDataConnectToDataManager spDataCTD;

    public int CurrentTotalCost
    {
        get
        {
            int lockCount = spSlots.Count(slot => slot.isLocked);
            return (lockCount + 1) * 5; 
        }
    }

    void Start()
    {
        DataManager.Instance.Scrap = 1000;
        if(EventManager.Instance != null) 
            EventManager.Instance.StartList("CurrencyChange", RefreshScrapUI);
        
        RefreshScrapUI();
        UpdateDrawCostUI();

        for(int i = 0; i < synergyLevelUIs.Count; i++)
        {
            synergyLevelUIs[i].Init(synergyNames[i]);
        }
        spDataCTD = GetComponent<SPDataConnectToDataManager>();
    }

    public void OnClickSPChange()
    {
        if (spSlots.All(slot => slot.isLocked)) return;

        int finalCost = CurrentTotalCost;
        if (DataManager.Instance.Scrap < finalCost) return;

        DataManager.Instance.AddScrap(-finalCost);
        ExecuteDrawLogic(); // 실제 뽑기 로직을 별도 함수로 분리 (자동 뽑기와 공유)
        CalcTotalSynergy();
    }

    // 실제 슬롯 데이터를 갱신하는 핵심 로직
    private void ExecuteDrawLogic()
    {
        foreach (var slot in spSlots)
        {
            if (slot.isLocked) continue;

            string randRank = Loader.GetRandomRank();
            SPData result = Loader.GetRandomSPByRank(randRank);

            string randSynergy = synergyNames[UnityEngine.Random.Range(0, synergyNames.Length)];
            SynergyIconData icon = GetSynergyData(randSynergy);

            if (result != null && icon != null)
            {
                Color synergyColor = new Color(icon.color.x/255f, icon.color.y/255f, icon.color.z/255f, 1f);
                slot.UpdateSlotUI(result, randSynergy, icon.synergySprite, synergyColor);
            }
        }

        spDataCTD.SPDataSaveToDataManager(); // DataManager에 특성 저장하는 로직
        spDataCTD.SPDataSaveToPlayer(); // PlayerStat에 반영하는 로직
        Debug.Log("[특성] 특성 포인트 출력 완료");
        foreach(var SPDataValue in DataManager.Instance.TraitSlots)
        {
            Debug.Log($"[특성]{SPDataValue.slotIndex} / {SPDataValue.traitId} / {SPDataValue.isLocked}");
        }
    }

    #region Auto Draw Logic
    public void OnClickAutoDraw()
    {
        if (isAutoDrawing) StopAutoDraw();
        else StartAutoDraw();
    }

    private void StartAutoDraw()
    {
        if (spSlots.All(slot => slot.isLocked)) return;

        isAutoDrawing = true;
        if (autoDrawButtonText != null) autoDrawButtonText.text = "STOP";
        drawButton.interactable = false;
        autoDrawCoroutine = StartCoroutine(AutoDrawRoutine());
    }

    private void StopAutoDraw()
    {
        isAutoDrawing = false;
        if (autoDrawButtonText != null) autoDrawButtonText.text = "AUTO DRAW";
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

            if (DataManager.Instance.Scrap < finalCost)
            {
                StopAutoDraw();
                yield break;
            }

            DataManager.Instance.AddScrap(-finalCost);
            ExecuteDrawLogic();
            CalcTotalSynergy();

            yield return new WaitForSeconds(autoDelay);
        }
    }
    #endregion

    private void CalcTotalSynergy()
    {
        Dictionary<string, int> counts = synergyNames.ToDictionary(name => name.ToLower(), name => 0);

        foreach (var slot in spSlots)
        {
            if (!string.IsNullOrEmpty(slot.currentSynergy))
            {
                string sName = slot.currentSynergy.ToLower();
                if (counts.ContainsKey(sName)) counts[sName]++;
            }
        }

        foreach (var ui in synergyLevelUIs)
        {
            if (counts.ContainsKey(ui.synergyName.ToLower()))
                ui.UpdateLevel(counts[ui.synergyName.ToLower()]);
        }
    }

    public SynergyIconData GetSynergyData(string sName) => synergyIcons.Find(x => x.synergyName.ToLower() == sName.ToLower());
    private void RefreshScrapUI() { if(scrapText != null) scrapText.text = DataManager.Instance.Scrap.ToString(); }
    
    public void UpdateDrawCostUI()
    {
        int lockCount = spSlots.Count(slot => slot.isLocked);
        if (drawButton != null) drawButton.interactable = (lockCount < spSlots.Count);
        oneDrawScrap.text = (lockCount >= spSlots.Count) ? "--" : CurrentTotalCost.ToString();
    }

    private void OnDestroy()
    {
        if (EventManager.Instance != null) EventManager.Instance.StopList("CurrencyChange", RefreshScrapUI);
    }
}
   

 