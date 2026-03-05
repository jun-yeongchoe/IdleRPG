using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Interactions;


public class SPDataAndLocked 
{
    public SPData spdata;
    public bool isLocked;
}

public class SPDataConnectToDataManager : MonoBehaviour
{
    private SPDraw spDraw;
    
    
    public Dictionary<int, SPDataAndLocked> eachSlotData = new Dictionary<int, SPDataAndLocked>();

    void Start()
    {
        spDraw = GetComponent<SPDraw>();
    }

    public void SPDataSaveToDataManager()
    {
        SPDataSaveToDict();

        foreach(var slotdata in eachSlotData)
        {
            TraitSaveData data = new TraitSaveData
            {
                slotIndex = slotdata.Key,
                traitId = slotdata.Value.spdata.id,
                isLocked = slotdata.Value.isLocked
            };
            DataManager.Instance.TraitSlots[data.slotIndex] = data;
        }

    }

    public void SPDataSaveToDict()
    {
        foreach(SPSlotUI slot in spDraw.spSlots)
        {
            SPData saveData = slot.spDataStorage;
            SPDataAndLocked slotData = new SPDataAndLocked
            {
                spdata = saveData,
                isLocked = slot.isLocked
            };
            int index = spDraw.spSlots.IndexOf(slot);
            eachSlotData[index] = slotData;
        }
    }

    public void SPDataSaveToPlayer()
    {
        foreach(SPSlotUI slot in spDraw.spSlots)
        {
            SPData saveData = slot.spDataStorage;
            SPPointData hasSP = new SPPointData(saveData);
            PlayerStat.instance.hasSPData.Add(hasSP);
        }
    }

    /********************************************
        public class TraitSaveData
        {
            public int slotIndex;   //슬롯 번호
            public int traitId;     //부여된 특성 ID(0이면 빈 칸)
            public bool isLocked;   //자물쇠 잠금 여부
        }
        public TraitSaveData[] TraitSlots = new TraitSaveData[5];
    *******************************************/
}
