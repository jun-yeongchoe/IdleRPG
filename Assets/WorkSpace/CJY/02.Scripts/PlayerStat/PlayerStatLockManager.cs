using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatLockManager : MonoBehaviour
{
    [Serializable]
    public struct LockMapping
    {
        public int statID;
        public GameObject lockPanel;
    }

    [Header("Lock Settings")]
    [SerializeField] private List<LockMapping> lockMappings;

    private PlayerStatLoaderFromGoogleSheets loader;

    void Start()
    {
        loader = CSV_LoadManager.Instance.playerStats_CSV;

        StartCoroutine(WaitForDataAndInit());

        if(EventManager.Instance != null) EventManager.Instance.StartList("StatChange", RefreshAllLocks);
    }

    private void OnDestroy()
    {
        if (EventManager.Instance != null)EventManager.Instance.StopList("StatChange", RefreshAllLocks);
    }
    
    IEnumerator WaitForDataAndInit()
    {
        if(loader == null) yield break;

        while(!loader.isLoaded) yield return null;
        RefreshAllLocks();
    }

    public void RefreshAllLocks()
    {
        if(loader ==null || !loader.isLoaded) return;
        foreach(var mapping in lockMappings)
        {
            var data = loader.playerStatDataList.Find(x => x.ID == mapping.statID);
            if(data == null) continue;

            bool isUnlocked = CheckUnlock(data);

            if(mapping.lockPanel != null) mapping.lockPanel.SetActive(!isUnlocked);
        }
    }

    private bool CheckUnlock(PlayerStatData_CSV data)
    {
        if(data.UnlockCondition <= 0 ) return true;
        int currentLevel = GetLevelByID(data.UnlockCondition);
        return currentLevel >= data.UnlockLevel;
    }

    private int GetLevelByID(int id)
    {
        if (DataManager.Instance == null) return 0;

        // ID별로 DataManager의 레벨 변수 매핑
        return id switch
        {
            101 => DataManager.Instance.AtkLv,
            102 => DataManager.Instance.HpLv,
            103 => DataManager.Instance.RecoverLv,
            104 => DataManager.Instance.AtSpeedLv,
            105 => DataManager.Instance.CritPerLv,
            106 => DataManager.Instance.CritDmgLv,
            _ => 0
        };
    }
}
