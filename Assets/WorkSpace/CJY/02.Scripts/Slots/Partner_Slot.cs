using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Partner_Slot : MonoBehaviour
{
    [Header("UI References")]
    public Image[] slotIcons; // 3개의 슬롯 Image 컴포넌트 연결
    public Sprite emptySlotSprite; // 빈 슬롯일 때 표시할 기본 이미지

    private string prefabPath = "Items/Partners/"; 

    void OnEnable()
    {
        RefreshSkillSlots();
        EventManager.Instance.StartList("PartnerSlotChanged", RefreshSkillSlots);
    }

    void OnDisable()
    {
        EventManager.Instance.StopList("PartnerSlotChanged", RefreshSkillSlots);
    }

    public void RefreshSkillSlots()
    {
        int[] partnerSlots = DataManager.Instance.CompanionSlot;

        for (int i = 0; i < slotIcons.Length; i++)
        {
            int partnerID = partnerSlots[i];

            
            if (partnerID <= 0)
            {
                slotIcons[i].sprite = emptySlotSprite;
                slotIcons[i].color = new Color(1, 1, 1, 1);
                continue;
            }

            
            GameObject partnerPrefab = Resources.Load<GameObject>(prefabPath + partnerID.ToString());

            if (partnerPrefab != null && partnerPrefab.TryGetComponent(out PartnerDataSO partnerData))
            {
                
                slotIcons[i].sprite = GetPartnerIcon(partnerPrefab);
                slotIcons[i].color = Color.white;
            }
            else
            {
                slotIcons[i].sprite = emptySlotSprite;
                Debug.LogWarning($"슬롯 {i}: {partnerID} 프리팹 로드 실패");
            }
        }
    }

    private Sprite GetPartnerIcon(GameObject prefab)
    {
        var iconTransform = prefab.transform.Find("IconImage");
        if (iconTransform != null)
        {
            return iconTransform.GetComponent<Image>().sprite;
        }
        return null;
    }

    public void OnClickMySlot(int index)
    {
        Debug.Log("온클릭슬롯(인덱스 : "+index + ")");
        if(BasePartnerWindow.pickedPartner != null)
        {
            int partnerID = BasePartnerWindow.pickedPartner.ID;

            DataManager.Instance.CompanionSlot[index] = partnerID;
            EventManager.Instance.TriggerEvent("PartnerSlotChanged");

            BasePartnerWindow.pickedPartner = null;
        }
    }
}
