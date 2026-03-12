using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartnerSlotBinding : MonoBehaviour
{
    [SerializeField] int slotIdx;
    [SerializeField] GameObject[] partnerPrefabs;

    void Start()
    {
        if(DataManager.Instance.CompanionSlot != null) Binding();
        EventManager.Instance.StartList("PartnerSlotChanged",Binding);
    }

    void OnDestroy()
    {
        EventManager.Instance.StopList("PartnerSlotChanged",Binding);
    }

    public void Binding()
    {
        int id = DataManager.Instance.CompanionSlot[slotIdx];
        if(id <= 0) return;
        if(transform.childCount > 0) Destroy(transform.GetChild(0).gameObject);
        foreach(GameObject obj in partnerPrefabs)
        {
            if(obj.name == id.ToString())
            {
                Instantiate(obj, this.transform);
            }
            else continue;
        }
    }
}
