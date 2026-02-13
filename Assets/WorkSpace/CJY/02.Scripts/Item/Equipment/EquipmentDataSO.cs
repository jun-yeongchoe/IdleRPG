using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Equip_", menuName = "SO/Item/Equipment")]
public class EquipmentDataSO : ItemBase
{
    public EquipmentType equipmentType;

    public float Value;
    public string Description;

    public override float GetStatMultiplier(int currentLevel)
    {
        return base.GetStatMultiplier(currentLevel);
    }
}
