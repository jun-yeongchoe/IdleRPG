using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScrapDisplay : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI scrapTxt;
    string eventName = "CurrencyChange";

    void Start()
    {
        UpdateUI();
        EventManager.Instance.StartList(eventName, UpdateUI);
    }

    void Oestroy()
    {
        EventManager.Instance.StopList(eventName, UpdateUI);
    }

    private void UpdateUI()
    {
        scrapTxt.text = DataManager.Instance.Scrap.ToString();
    }
}
