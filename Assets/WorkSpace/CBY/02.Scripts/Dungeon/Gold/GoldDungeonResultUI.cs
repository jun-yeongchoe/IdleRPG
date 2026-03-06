using UnityEngine;
using TMPro;
using System.Numerics;

public class GoldDungeonResultUI : MonoBehaviour
{
    public static GoldDungeonResultUI Instance;

    [SerializeField] private TextMeshProUGUI rewardText;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void Show(BigInteger reward)
    {
        rewardText.text = reward.ToString("N0"); // 1,000,000 Ç¥½Ã
        gameObject.SetActive(true);
    }
}