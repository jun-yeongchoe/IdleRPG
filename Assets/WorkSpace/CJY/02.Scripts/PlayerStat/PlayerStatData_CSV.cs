using System;

[Serializable]
public class PlayerStatData_CSV
{
    public int ID;
    public string StatName;
    public float BaseValue;
    public float GrowthPerLevel;
    public float StartCost;
    public float CostGrowthRate;
    public int LimitLevel;
    public int UnlockCondition;
    public int UnlockLevel;
}
