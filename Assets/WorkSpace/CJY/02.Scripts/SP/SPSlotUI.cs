using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SPSlotUI : MonoBehaviour
{
    [Header("UI Ref")]
    [SerializeField] private TextMeshProUGUI rankText, valueText;

    [Header("Lock Status")]
    public GameObject lockImage, UnlockImage;
    public bool isLocked = false;

    [SerializeField] public Image synergyIcon;
    public string currentSynergy;
    public SPData spDataStorage;
    

    public void UpdateSlotUI(SPData data, string synergyName = null, Sprite icon = null, Color synergyColor = default)
    {
        if (data == null) return;
        spDataStorage = data;
        currentSynergy = synergyName;

        rankText.text = data.rank;

        float displayRate;
        string suffix;

        // "Attack_Speed" 타입인지 확인
        if (data.type == "Attack_Speed")
        {
            // 공속이면 100을 곱하지 않고 수치 그대로 사용
            displayRate = data.rate;
            suffix = "";
        }
        else
        {
            // 그 외 타입은 100을 곱하고 %를 붙임
            displayRate = data.rate * 100f;
            suffix = "%";
        }

        // N2는 소수점 둘째자리까지 표시 (공속의 미세한 변화를 보여주기 위함)
        valueText.text = $"{data.type} +{displayRate:N2}{suffix}";

        // if(synergyIcon != null && icon != null)
        // {
        //     synergyIcon.sprite = icon;
        //     // synergyIcon.gameObject.SetActive(true);
        // }

        if (synergyIcon != null)
        {
            if(icon != null)
            {
                synergyIcon.gameObject.SetActive(true);
                synergyIcon.sprite = icon;
                synergyIcon.color = synergyColor;
            }
        }
        
        SetRankColor(data.rank);
    }

    private void SetRankColor(string rank)
    {
        switch (rank)
        {
            case "SS" : case "SSS" : rankText.color = Color.red; break;
            case "S" : rankText.color = new Color(1f, 0.5f, 0f); break;
            case "A" : rankText.color = Color.magenta; break;
            case "B" : rankText.color = Color.blue; break;
            case "C" : rankText.color = Color.green; break;
            case "D" : rankText.color = Color.yellow; break;
            case "E" : rankText.color = Color.white; break;
            default : rankText.color = Color.gray; break;
        }
    }

    public void RefreshLockUI()
    {
        if (lockImage != null) lockImage.SetActive(isLocked);
        if (UnlockImage != null) UnlockImage.SetActive(!isLocked);
    }

    public void ToggleLock()
    {
        isLocked = !isLocked;
        RefreshLockUI();

        if (GetComponentInParent<SPDraw>() != null) GetComponentInParent<SPDraw>().UpdateDrawCostUI();
    }

}
