using UnityEngine;
using UnityEngine.SceneManagement;
using System.Numerics;

public class GoldDungeonManager : MonoBehaviour
{
    public static GoldDungeonManager Instance;

    [Header("던전 설정")]
    public BigInteger baseGoldReward = 1000;
    public BigInteger goldPerKill = 100;

    [Tooltip("클리어 보너스 퍼센트 (예: 150 = 1.5배)")]
    [SerializeField] private int clearBonusPercent = 150;

    private int killedEnemyCount;
    private bool isCleared;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        StartDungeon();
    }

    void StartDungeon()
    {
        killedEnemyCount = 0;
        isCleared = false;
    }

    // GoldDungeonEnemySpawner에서 호출
    public void OnEnemyKilled()
    {
        if (isCleared) return;
        killedEnemyCount++;
    }

    // 모든 적 처치 시 호출
    public void ClearDungeon()
    {
        if (isCleared) return;
        isCleared = true;

        BigInteger reward = CalculateReward();

        if (DataManager.Instance != null)
            DataManager.Instance.AddGold(reward);

        if (GoldDungeonResultUI.Instance != null)
            GoldDungeonResultUI.Instance.Show(reward);
    }

    BigInteger CalculateReward()
    {
        BigInteger reward =
            baseGoldReward +
            (goldPerKill * killedEnemyCount);

        // 정수 기반 배율 계산
        reward = reward * clearBonusPercent / 100;

        return reward;
    }

    public void ExitDungeon()
    {
        SceneManager.LoadScene("Lobby");
    }
}