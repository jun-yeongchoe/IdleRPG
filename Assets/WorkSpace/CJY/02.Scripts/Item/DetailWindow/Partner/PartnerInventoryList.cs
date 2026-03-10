using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PartnerInventoryList : MonoBehaviour
{
    [Header("UI Ref")]
    public Transform content;
    public GameObject textBoxPrefab;
    private int fontSize = 20;

    public Button mergeAllBtn;

    [Header("Resources Path")]
    private string prefabPath = "Items/Partners/";

    public BasePartnerWindow detailWindow;


    void Start()
    {
        mergeAllBtn.onClick.AddListener(OnClickAllMerge);
    }

    private void OnClickAllMerge()
    {
        PartnerMerge Merge = GetComponent<PartnerMerge>();
        Merge.MergeAll();
        RefreshInvenUI();
    }

    private void OnEnable()
    {
        RefreshInvenUI();
    }

    public void RefreshList()
    {
         RefreshInvenUI();
    }

    public void RefreshInvenUI()
    {
        foreach(Transform child in content)
        {
            Destroy(child.gameObject);
        }

        var sortedInven = DataManager.Instance.CompanionDict.OrderBy(x => x.Key);

        foreach(var item in sortedInven)
        {
            int itemID = item.Key;

            GameObject itemPrefab = Resources.Load<GameObject>(prefabPath + itemID.ToString());

            if(itemPrefab != null)
            {
                if(itemPrefab.TryGetComponent(out PartnerDataSO partnerData))
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
                        detailWindow.gameObject.SetActive(true);
                        detailWindow.UpdateUI(partnerData, itemLevel, itemQuantity);
                    });

                    if (slot.TryGetComponent(out Image img))
                    {
                        img.raycastTarget = true;
                    }
                }
            }
            else
            {
                Debug.LogWarning($"스킬 프리팹을 찾을 수 없음. {itemID}");
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
