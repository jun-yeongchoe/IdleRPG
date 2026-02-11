using UnityEngine;
using UnityEngine.UI;

public class BossRushUI : MonoBehaviour
{
    public static BossRushUI Instance;

    public Text bossCountText;
    public Text defeatedText;
    public GameObject clearPanel;

    private void Awake()
    {
        Instance = this;
        clearPanel.SetActive(false);
    }

    public void UpdateBossInfo(int current, int total)
    {
        bossCountText.text = $"보스 {current} / {total}";
    }

    public void UpdateDefeated(int count)
    {
        defeatedText.text = $"처치 수 : {count}";
    }

    public void ShowClear()
    {
        clearPanel.SetActive(true);
    }
}
