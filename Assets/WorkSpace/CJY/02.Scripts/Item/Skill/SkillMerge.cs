using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SkillMerge : MonoBehaviour
{
    public bool TryMerge(int itemID)
    {
        if(!DataManager.Instance.SkillDict.TryGetValue(itemID, out ItemSaveData data)) return false;

        string path = $"Items/Skills/{itemID}";
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

public void MergeAllSkill()
{
    var dict = DataManager.Instance.SkillDict;
    bool movedAtLeastOnce;
    do 
    {
        movedAtLeastOnce = false;
        foreach (var id in dict.Keys.ToList())
        {
            ItemSaveData save = dict[id];
            GameObject prefab = Resources.Load<GameObject>($"Items/Skills/{id}");
            
            if (prefab != null && prefab.TryGetComponent(out SkillDataSo data))
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
