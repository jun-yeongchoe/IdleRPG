using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class StatItem : MonoBehaviour
{
    [Header("데이터 구분 아이디")]
    public string statID; // 인스펙터에서 설정

    [Header("연결")]
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI valueText;
    public TextMeshProUGUI costText;
    public Button upgradeButton;

    public void OnUpgradeButtonClicked()
    {
        // 로직 작성
        Debug.Log(statID + " 업그레이드");
    }
}