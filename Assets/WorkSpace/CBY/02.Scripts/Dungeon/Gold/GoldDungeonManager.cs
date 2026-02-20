using UnityEngine;

// 골드 던전의 진행,골드 집계,결과 처리를 담당
public class GoldDungeonManager : MonoBehaviour
{
    public static GoldDungeonManager Instance;

    [Header("Dungeon")]
    public float dungeonTime = 60f;

    [Header("Gold")]
    public int totalGold;

    private float timer;
    private bool isEnded;

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        timer = dungeonTime;
        totalGold = 0;
        isEnded = false;
        Time.timeScale = 1f;
    }

    private void Update()
    {
        if (isEnded) return;

        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            EndDungeon();
        }
    }

    // 골드 던전 전용 골드 추가
    public void AddGold(int amount)
    {
        if (isEnded) return;

        totalGold += amount;
    }

    void EndDungeon()
    {
        isEnded = true;

        Debug.Log($"골드 던전 종료 / 획득 골드: {totalGold}");

        Time.timeScale = 0f; // 결과 UI 표시용
    }
}
