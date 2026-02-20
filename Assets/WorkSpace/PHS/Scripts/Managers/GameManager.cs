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

            int hours = (int)(totalSeconds / 3600);
            int minutes = (int)((totalSeconds % 3600) / 60);
            string timeStr = hours > 0 ? $"{hours}시간 {minutes}분" : $"{minutes}분";

            if (CommonPopup.Instance != null)
            {
                CommonPopup.Instance.ShowAlert(
                    "휴식 보상",
                    $"오프라인 시간 동안 골드가 모였습니다!\n\n방치 시간: {timeStr}\n획득 골드: {reward}G",
                    "수령"
                );
            }

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
