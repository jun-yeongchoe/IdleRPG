using System.Collections.Generic;
using System.Linq;
using UnityEditor.SceneManagement;
using UnityEngine;

public class ItemDataManager : MonoBehaviour
{
    public static ItemDataManager Instance { get; private set; }

    public EquipmentDataSO[] assets;
    public GameObject[] prefabs;
    // ID로 검색하기 위한 사전
    public Dictionary<int, EquipmentDataSO> equipmentDict = new Dictionary<int, EquipmentDataSO>();
    public Dictionary<int, GameObject> prefabDict = new Dictionary<int, GameObject>();

    void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); }

    }
    void Start()
    {
        assets = Resources.LoadAll<EquipmentDataSO>("Items/EquipItem");
        prefabs = Resources.LoadAll<GameObject>("Items/EquipItem");
        LoadAllEquipments();
        LoadAllPrefabs();
        
    }

    void LoadAllEquipments()
    {
        foreach (var asset in assets)
        {
            if (!equipmentDict.ContainsKey(asset.ID))
                equipmentDict.Add(asset.ID, asset);
        }
        Debug.Log($"{equipmentDict.Count}개의 장비 데이터를 로드했습니다.");

        // {
        //     // 딕셔너리의 Value들 중 하나를 무작위로 선택
        //     List<EquipmentDataSO> tempList = equipmentDict.Values.ToList();
        //     int randomIndex = Random.Range(0, tempList.Count);
        //     EquipmentDataSO randomItem = tempList[randomIndex];

        //     Debug.Log($"<color=yellow>[테스트]</color> 무작위 로드 확인 - ID: {randomItem.ID}, 이름: {randomItem.Name_KR}, 기본치: {randomItem.Value}");
        // }
        // else
        // {
        //     Debug.LogWarning("로드된 장비 데이터가 없습니다. Resources/Equipments 경로를 확인하세요.");
        // }
    }

    void LoadAllPrefabs()
    {
        foreach (var p in prefabs)
        {
            
            if (int.TryParse(p.name, out int id))
            {
                if (!prefabDict.ContainsKey(id))
                    prefabDict.Add(id, p);
            }
        }
        Debug.Log($"{prefabDict.Count}개의 장비 프리팹을 로드했습니다.");
    }

    public EquipmentDataSO GetEquipment(int id)
    {
        equipmentDict.TryGetValue(id, out var data);
        return data;
    }

    public GameObject GetEquipmentPrefab(int id)
    {
        prefabDict.TryGetValue(id, out var prefab);
        return prefab;
    }
}