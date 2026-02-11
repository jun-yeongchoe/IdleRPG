using UnityEngine;
using TMPro;
using System.Numerics;

public class TopInfoPanel : MonoBehaviour
{
    [Header("UI 연결")]
    public TextMeshProUGUI goldText; // 골드 텍스트
    public TextMeshProUGUI gemText; // 다이아 텍스트

    // 변화 감지용
    private BigInteger lastGold = -1;
    private BigInteger lastGem = -1;

    void Update()
    {
        if (DataManager.Instance == null) return;

        // 재화 가져오기
        BigInteger currentGold = DataManager.Instance.Gold;
        BigInteger currentGem = DataManager.Instance.Gem;

        // 골드 변화
        if (currentGold != lastGold)
        {
            lastGold = currentGold; // 갱신

            goldText.text = FormatBigInt(currentGold);
        }

        // 다이아 변화
        if (currentGem != lastGem)
        {
            lastGem = currentGem;

            gemText.text = FormatBigInt(currentGem);
        }
    }

    private string FormatBigInt(BigInteger value)
    {
        return value.ToString("N0");
    }
}