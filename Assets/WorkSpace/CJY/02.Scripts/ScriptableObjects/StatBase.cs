using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "StatBase", menuName = "SO/StatData")]
public class StatBase : ScriptableObject
{
    // 베이스 스탯 값
    public float baseValue, valueIncre;
    // 베이스 코스트
    public float baseCost, costIncre;

    // 레벨 제한
    public int limitLevel = -1;

    public float GetValue(int level) => baseValue + (level -1) * valueIncre;
    public double GetCost(int level) => baseCost + (double)(level -1) * costIncre;
}
