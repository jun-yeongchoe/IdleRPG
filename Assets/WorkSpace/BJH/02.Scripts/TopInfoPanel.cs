using UnityEngine;
using TMPro;
using System.Numerics;

public class TopInfoPanel : MonoBehaviour
{
    [Header("UI 연결")]
    public TextMeshProUGUI goldText; //골드 텍스트
    public TextMeshProUGUI gemText; //다이아 텍스트

    void Start()
    {
        if (EventManager.Instance != null)
            EventManager.Instance.StartList("CurrencyChange", UpdateCurrencyUI);

        UpdateCurrencyUI();
    }

    private void OnDestroy()
    {
        if (EventManager.Instance != null)
            EventManager.Instance.StopList("CurrencyChange", UpdateCurrencyUI);
    }

    void UpdateCurrencyUI()
    { 
        if(DataManager.Instance == null) return;

        goldText.text = DataManager.Instance.Gold.ToCurren();
        gemText.text = DataManager.Instance.Gem.ToCurren();
    }
}