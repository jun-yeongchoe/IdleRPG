using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    private void Start()
    {
        //오프라인 보상 계산
        CalculateOfflineReward();
    }

    private void OnApplicationQuit() => SaveExitTime();

    private void OnApplicationPause(bool pause)
    {
        if (pause) SaveExitTime();
        else CalculateOfflineReward();
    }

    private void SaveExitTime()
    {
        string timeStr = DateTime.Now.ToBinary().ToString();
        PlayerPrefs.SetString("LastExitTime", timeStr);
        PlayerPrefs.Save();
        Debug.Log($"[GameManager] 종료 시간 저장됨: {DateTime.Now}");
    }

    private void CalculateOfflineReward()
    {
        if (!PlayerPrefs.HasKey("LastExitTime")) return;

        long temp = Convert.ToInt64(PlayerPrefs.GetString("LastExitTime"));
        DateTime lastExitTime = DateTime.FromBinary(temp);
        TimeSpan difference = DateTime.Now - lastExitTime;
        double totalSeconds = difference.TotalSeconds;

        if (totalSeconds < 10) return;
        if (totalSeconds > 86400) totalSeconds = 86400;

        // 보상 지급
        if (DataManager.Instance != null)
        {
            BigInteger goldPerSec = DataManager.Instance.GetGoldperSec();
            BigInteger reward=goldPerSec*(int)totalSeconds;

            DataManager.Instance.AddGold(reward);

            //나중에 UI 팝업 띄우는 이벤트도 여기서 쏘면 됨
            //EventManager.Instance.TriggerEvent("ShowOfflineRewardPopup");

            Debug.Log($"[GameManager] {totalSeconds:F0}초 방치 보상: {reward.ToCurren()} 획득");
        }
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
    }
}
