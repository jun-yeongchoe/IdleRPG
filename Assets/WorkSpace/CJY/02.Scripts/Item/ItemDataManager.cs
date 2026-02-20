using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemDataManager : MonoBehaviour
{
    public static ItemDataManager Instance { get; private set; }

    // ID로 검색하기 위한 사전
    private Dictionary<int, EquipmentDataSO> equipmentDict = new Dictionary<int, EquipmentDataSO>();

    void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); }

        LoadAllEquipments();
    }

    void LoadAllEquipments()
    {
        // Resources 폴더나 특정 경로에서 모든 EquipmentDataSO를 로드
        // (UniversalSheetLoader가 저장하는 경로가 Resources 하위라면 아래처럼 가능)
        EquipmentDataSO[] assets = Resources.LoadAll<EquipmentDataSO>("Equipments");
        
        foreach (var asset in assets)
        {
            if (!equipmentDict.ContainsKey(asset.ID))
                equipmentDict.Add(asset.ID, asset);
        }
        Debug.Log($"{equipmentDict.Count}개의 장비 데이터를 로드했습니다.");

        if (equipmentDict.Count > 0)
        {
            // 딕셔너리의 Value들 중 하나를 무작위로 선택
            List<EquipmentDataSO> tempList = equipmentDict.Values.ToList();
            int randomIndex = Random.Range(0, tempList.Count);
            EquipmentDataSO randomItem = tempList[randomIndex];

            Debug.Log($"<color=yellow>[테스트]</color> 무작위 로드 확인 - ID: {randomItem.ID}, 이름: {randomItem.Name_KR}, 기본치: {randomItem.Value}");
        }
        else
        {
            Debug.LogWarning("로드된 장비 데이터가 없습니다. Resources/Equipments 경로를 확인하세요.");
        }
    }

    public EquipmentDataSO GetEquipment(int id)
    {
        equipmentDict.TryGetValue(id, out var data);
        return data;
    }
}