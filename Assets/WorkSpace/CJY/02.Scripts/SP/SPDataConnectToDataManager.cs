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

    void Awake()
    {
        spDraw = GetComponent<SPDraw>();
    }
    void Start()
    {
        
        EventManager.Instance.StartList("CSV_DataLoaded", RestoreSPUIFromDataManager);
    }
    void OnEnable()
    {
        RestoreSPUIFromDataManager();
    }
    void OnDestroy()
    {
        EventManager.Instance.StopList("CSV_DataLoaded", RestoreSPUIFromDataManager);
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
        PlayerStat.instance.hasSPData.Clear();
        foreach(SPSlotUI slot in spDraw.spSlots)
        {
            SPData saveData = slot.spDataStorage;
            if (saveData != null) 
            {
                SPPointData hasSP = new SPPointData(saveData);
                PlayerStat.instance.hasSPData.Add(hasSP);
            }
            PlayerStat.instance.UpdateFinalStats();
        }
    }

    public void RestoreSPUIFromDataManager()
    {
        for(int i=0; i < DataManager.Instance.SynergyName.Count; i++) Debug.Log($"데이터매니저 저장값 [{i}] : {DataManager.Instance.SynergyName[i]}");
        if (!CSV_LoadManager.Instance.allLoaded)
        {
            Debug.LogWarning("CSV 로드가 완료되지 않아 특성 UI를 복구할 수 없습니다.");
            return;
        }
        for (int i = 0; i < DataManager.Instance.TraitSlots.Length; i++)
        {
            TraitSaveData savedData = DataManager.Instance.TraitSlots[i];

            if (i < spDraw.spSlots.Count)
            {
                SPSlotUI slot = spDraw.spSlots[i];

                if (savedData != null && savedData.traitId > 0)
                {
                    SPData actualData = CSV_LoadManager.Instance.SP_CSV.valueTable.Find(x => x.id == savedData.traitId);

                    if (actualData != null)
                    {
                        string savedSynergyName = "";
                        if (i < DataManager.Instance.SynergyName.Count) savedSynergyName = DataManager.Instance.SynergyName[i];
                        SynergyIconData synIconData = spDraw.GetSynergyData(savedSynergyName);
                        Sprite synSprite = null;
                        Color synColor = Color.white;

                        if (synIconData != null)
                        {
                            synSprite = synIconData.synergySprite;
                            synColor = new Color(synIconData.color.x / 255f, synIconData.color.y / 255f, synIconData.color.z / 255f, 1f);
                        }
                        slot.UpdateSlotUI(actualData, savedSynergyName, synSprite, synColor);

                        // slot.spDataStorage = actualData;
                        slot.isLocked = savedData.isLocked;
                        
                        slot.RefreshLockUI();
                        Debug.Log($"슬롯 {i} 복구중 - 시너지이름: {savedSynergyName} / 데이터찾음: {synIconData != null} / 아이콘유무 : {synSprite is null}");
                    }
                }
                else
                {
                    slot.spDataStorage = null;
                    slot.currentSynergy = null;
                    slot.isLocked = false;
                    slot.UpdateSlotUI(null);
                    slot.RefreshLockUI();
                }
            }
        }
        
        SPDataSaveToPlayer();
        Debug.Log("게임을 끄기 전 장착했던 특성 UI 복구 완료!");
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
