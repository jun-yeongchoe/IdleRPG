using UnityEngine;
using System.Numerics;

public class CurrencyTester : MonoBehaviour
{
    public int addAmount = 10000;

    void Update()
    {
        // G 키를 누르면 골드 추가
        if (Input.GetKeyDown(KeyCode.G))
        {
            if (DataManager.Instance != null)
            {
                DataManager.Instance.AddGold(addAmount);
            }
        }

        // H 키를 누르면 다이아 추가
        if (Input.GetKeyDown(KeyCode.H))
        {
            if (DataManager.Instance != null)
            {
                DataManager.Instance.AddGem(addAmount);
            }
        }
    }
}