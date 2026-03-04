using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemMerge : MonoBehaviour
{
    public bool TryMerge(int itemID, EquipmentType type)
    {
        if(!DataManager.Instance.InventoryDict.TryGetValue(itemID, out ItemSaveData data)) return false;

        string path = $"Items/EquipItem/{itemID}";
        GameObject prefab = Resources.Load<GameObject>(path);

        if(prefab != null && prefab.TryGetComponent(out ItemBase itemBase))
        {
            int required = itemBase.GetRequiredComposeCount(data.level);

            if(data.value >= required)
            {
                data.value -= required;
                data.level++;
                return true;
            }
        }
        return false;
    }


    public void MergeAll(EquipmentType type)
    {
        var dict = DataManager.Instance.InventoryDict;
        bool anyMerged = false;

        foreach(var pair in dict)
        {
            int id = pair.Key;
            ItemSaveData data = pair.Value;
            string path = $"Items/EquipItem/{id}";
            GameObject prefab = Resources.Load<GameObject>(path);
            if(prefab != null && prefab.TryGetComponent(out EquipmentDataSO equipData))
            {
                if(equipData.equipmentType == type)
                {
                    while (true)
                    {
                        int req = equipData.GetRequiredComposeCount(data.level);
                        if(data.value >= req)
                        {
                            data.value -= req;
                            data.level++;
                            anyMerged = true;
                        }
                        else break;
                    }
                }
            }
        }
        if(anyMerged) Debug.Log("일괄 합성 완료");
    }
}
