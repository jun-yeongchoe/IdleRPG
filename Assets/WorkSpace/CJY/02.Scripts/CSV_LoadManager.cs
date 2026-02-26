using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSV_LoadManager : MonoBehaviour
{
    public static CSV_LoadManager Instance{get; private set;}

    public PlayerStatLoaderFromGoogleSheets playerStats_CSV;
    public SPPointCSVLoader SP_CSV;

    public bool allLoaded = false;


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        EventManager.Instance.StartList("CSV_DataLoaded", CheckLoaded);
    }

    private void OnDestroy()
    {
        if (EventManager.Instance != null)
        {
            EventManager.Instance.StopList("CSV_DataLoaded", CheckLoaded);
        }
    }

    private void CheckLoaded()
    {
        allLoaded = true;
    }


    IEnumerator CheckLoadRoutine()
    {
        while (true)
        {
            // 모든 로더의 isLoaded가 true라면
            if (playerStats_CSV.isLoaded && SP_CSV.isLoaded)
            {
                EventManager.Instance.TriggerEvent("CSV_DataLoaded");
                Debug.Log("CSVLoader_Event 등록 완료");
                yield break; // 루틴 종료
            }
            yield return null;
        }
    }

    public bool AllCSVDataLoaded()
    {
        Debug.Log($"All Loaded : {allLoaded}");
        return allLoaded;
    }

}
