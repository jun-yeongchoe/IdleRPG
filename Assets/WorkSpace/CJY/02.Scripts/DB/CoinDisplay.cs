using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CoinDisplay : MonoBehaviour
{
    [SerializeField] PlayerStatus playerStatus;
    [SerializeField] TextMeshProUGUI goldText, gemText;

    private ReactiveProperty<int> gold = new ReactiveProperty<int>(1), 
    gem = new ReactiveProperty<int>(1);    
    
    void Start()
    {
        gold.Value = playerStatus.gold;
        gem.Value = playerStatus.gem;
        
        gold.AddAction(OnChangedValue1);
        gem.AddAction(OnChangedValue2);

        UpdateCoinDisplay();
    }
    private void OnChangedValue1(int obj)
    {
        playerStatus.gold = gold.Value;
        goldText.text = playerStatus.gold.ToString();
    }
    private void OnChangedValue2(int obj)
    {
        playerStatus.gem = gem.Value;
        gemText.text = playerStatus.gem.ToString();
    }
    public void UpdateCoinDisplay()
    {
        goldText.text = playerStatus.gold.ToString();
        gemText.text = playerStatus.gem.ToString();
    }
}
