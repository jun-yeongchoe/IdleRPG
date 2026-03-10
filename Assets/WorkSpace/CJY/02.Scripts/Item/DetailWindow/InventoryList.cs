using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryList : MonoBehaviour
{
    [Header("UI Ref")]
    public Transform content;
    public GameObject textBoxPrefab;
    private int fontSize = 20;
    public EquipmentType currentType;

    public Button mergeAllBtn;

    [Header("Resources Path")]
    private string prefabPath = "Items/EquipItem/";

    public BaseItemWindow detailWindow;


    void Start()
    {
        mergeAllBtn.onClick.AddListener(OnClickAllMerge);
    }

    private void OnClickAllMerge()
    {
        ItemMerge itemMerge = GetComponent<ItemMerge>();
        itemMerge.MergeAll(currentType);
        RefreshInvenUI(currentType);
        BaseItemWindow BIW = GetComponent<BaseItemWindow>();
        BIW.Refresh();

    }

    private void OnEnable()
    {
        RefreshInvenUI(currentType);
    }

    public void RefreshList()
    {
         RefreshInvenUI(currentType);
    }

    public void RefreshInvenUI(EquipmentType type)
    {
        foreach(Transform child in content)
        {
            Destroy(child.gameObject);
        }

        var sortedInven = DataManager.Instance.InventoryDict.OrderBy(x => x.Key);

        foreach(var item in sortedInven)
        {
            int itemID = item.Key;

            GameObject itemPrefab = Resources.Load<GameObject>(prefabPath + itemID.ToString());

            if(itemPrefab != null)
            {
                if(itemPrefab.TryGetComponent(out EquipmentDataSO equipData))
                {
                    if(equipData.equipmentType == type)
                    {
                        
                        int itemQuantity = item.Value.value;
                        int itemLevel = item.Value.level;
                        GameObject slot = Instantiate(itemPrefab, content);

                        CreateTextBoxes(slot.transform, itemQuantity, itemLevel);

                        Button btn = slot.AddComponent<Button>();
                        btn.transition = Selectable.Transition.ColorTint;
                        Navigation nav = new();
                        nav.mode = Navigation.Mode.None;
                        btn.navigation = nav;

                        btn.onClick.AddListener(() =>
                        {
                            detailWindow.UpdateUI(equipData, itemLevel, itemQuantity);
                        });

                        if (slot.TryGetComponent(out Image img))
                        {
                            img.raycastTarget = true;
                        }
                    }
                }
            }
            else
            {
                Debug.LogWarning($"아이템 프리팹을 찾을 수 없음. {itemID}");
            }
        }
    }

    private void CreateTextBoxes(Transform parent, int quantity, int level)
    {
        GameObject lvObj = Instantiate(textBoxPrefab, parent);
        lvObj.name = "Text(TMP)_Level";
        if(lvObj.TryGetComponent(out TextMeshProUGUI lvText))
        {
            lvText.text = $"LV{level}";
            lvText.fontSize = fontSize;
            lvText.alignment = TextAlignmentOptions.TopLeft;

            RectTransform rect = lvText.rectTransform;
            rect.anchorMin = new Vector2(0, 1);
            rect.anchorMax = new Vector2(0, 1);
            rect.pivot = new Vector2(0, 1);
            rect.anchoredPosition = new Vector2(5, -5);
        }
        GameObject qtyObj = Instantiate(textBoxPrefab, parent);
        qtyObj.name = "Text(TMP)_Quantity";
        if(qtyObj.TryGetComponent(out TextMeshProUGUI qtyText))
        {
            qtyText.text = $"x{quantity}";
            qtyText.fontSize = fontSize;
            qtyText.alignment = TextAlignmentOptions.BottomRight;

            RectTransform rect = qtyText.rectTransform;
            rect.anchorMin = new Vector2(1, 0);
            rect.anchorMax = new Vector2(1, 0);
            rect.pivot = new Vector2(1, 0);
            rect.anchoredPosition = new Vector2(-5, 5);
        }
    }
}
