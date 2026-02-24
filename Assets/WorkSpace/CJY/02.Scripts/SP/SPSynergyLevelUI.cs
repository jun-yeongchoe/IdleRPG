using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SPSynergyLevelUI : MonoBehaviour
{
    public string synergyName;
    [SerializeField] private TextMeshProUGUI levelText;

    public void Init(string sName)
    {
        synergyName = sName.ToLower();
        UpdateLevel(0);
    }

    public void UpdateLevel(int count)
    {
        levelText.text = $"LV {count}";

        CanvasGroup group = GetComponent<CanvasGroup>();
        if (group != null) group.alpha = (count > 0) ? 1.0f : 0.4f;
    }
}
