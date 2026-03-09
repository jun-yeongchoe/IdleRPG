using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Partner_Slot : MonoBehaviour
{
    [SerializeField] private RectTransform container;
    [SerializeField] private int slotIndex;

    void OnEnable()
    {
        RefreshUI();

        EventManager.Instance.StartList("ServerDataChange", RefreshUI);
    }

    void OnDisable()
    {
        EventManager.Instance.StopList("ServerDataChange", RefreshUI);
    }

    private void RefreshUI()
    {
        if(slotIndex < 0 ||slotIndex >=DataManager.Instance.EquipSlot.Length) return; 

        int currentID = DataManager.Instance.EquipSlot[slotIndex]; // Partnerslot 추가하면 만들기
        UpdateSlot(currentID);
    }

    private void UpdateSlot(int id)
    {
        foreach (Transform child in container) Destroy(child.gameObject);

        
        GameObject prefab = ItemDataManager.Instance.GetEquipmentPrefab(id);

        if (prefab != null)
        {
            GameObject instance = Instantiate(prefab, container);
            RectTransform rect = instance.GetComponent<RectTransform>();

            if (rect != null)
            {
                // Stretch 설정
                rect.anchorMin = Vector2.zero;
                rect.anchorMax = Vector2.one;
                rect.offsetMin = Vector2.zero;
                rect.offsetMax = Vector2.zero;
                rect.localScale = Vector3.one;
            }
        }
    }
}
