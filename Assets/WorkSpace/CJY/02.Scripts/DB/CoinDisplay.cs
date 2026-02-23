using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using TMPro;
using UnityEngine;

public class CoinDisplay : MonoBehaviour
{
    [SerializeField] PlayerStatus playerStatus;
    [SerializeField] TextMeshProUGUI goldText, gemText;

    private ReactiveProperty<BigInteger> gold = new ReactiveProperty<BigInteger>(1), 
    gem = new ReactiveProperty<BigInteger>(1);    
    
    void Start()
    {
        // 1. 액션 먼저 등록 (Value가 바뀔 때 바로 UI가 그려지도록)
        gold.AddAction(OnChangedGold);
        gem.AddAction(OnChangedGem);

        // 2. DataManager에서 현재 보유 중인 값을 가져와서 할당
        // 이 순간 Value가 바뀌면서 위에서 등록한 OnChanged 액션들이 즉시 실행됩니다.
        if (DataManager.Instance != null)
        {
            gold.Value = DataManager.Instance.Gold;
            gem.Value = DataManager.Instance.Gem;
        }
        else
        {
            // DataManager가 없을 경우를 대비해 초기화
            UpdateCoinDisplay();
        }
    }

    private void OnChangedGold(BigInteger newValue)
    {
        // ToString("N0")를 사용하면 1,000,000 처럼 쉼표가 붙어 더 읽기 편합니다.
        goldText.text = newValue.ToString();
    }

    private void OnChangedGem(BigInteger newValue)
    {
        gemText.text = newValue.ToString();
    }

    // 수동으로 갱신해야 할 때 호출
    public void UpdateCoinDisplay()
    {
        if (DataManager.Instance == null) return;

        gold.Value = DataManager.Instance.Gold;
        gem.Value = DataManager.Instance.Gem;
    }

    private void OnDestroy()
    {
        gold.RemoveAction(OnChangedGold);
        gem.RemoveAction(OnChangedGem);
    }
}
