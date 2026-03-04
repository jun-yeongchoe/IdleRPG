using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PartnerMerge : MonoBehaviour
{
    public bool TryMerge(int itemID)
    {
        if(!DataManager.Instance.CompanionDict.TryGetValue(itemID, out ItemSaveData data)) return false;

        string path = $"Items/Partners/{itemID}";
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

public void MergeAll()
{
    var dict = DataManager.Instance.CompanionDict;
    bool movedAtLeastOnce;
    do 
    {
        movedAtLeastOnce = false;
        foreach (var id in dict.Keys.ToList())
        {
            ItemSaveData save = dict[id];
            GameObject prefab = Resources.Load<GameObject>($"Items/Partners/{id}");
            
            if (prefab != null && prefab.TryGetComponent(out PartnerDataSO data))
            {
                int req = data.GetRequiredComposeCount(save.level);
                if (save.value >= req)
                {
                    save.value -= req;
                    save.level++;
                    movedAtLeastOnce = true;
                }
            }
        }
    } while (movedAtLeastOnce);
}
}
