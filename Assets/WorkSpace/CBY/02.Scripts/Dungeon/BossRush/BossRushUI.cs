using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class BossRushUI : MonoBehaviour
{
    public static BossRushUI Instance;

    public TextMeshProUGUI bossCountText;
    public TextMeshProUGUI defeatedText;
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
