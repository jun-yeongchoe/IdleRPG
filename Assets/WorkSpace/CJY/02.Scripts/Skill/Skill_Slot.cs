using UnityEngine;
using UnityEngine.UI;

public class SkillSlotUI : MonoBehaviour
{
    [Header("UI References")]
    public Image[] slotIcons; // 6개의 슬롯 Image 컴포넌트 연결
    public Sprite emptySlotSprite; // 빈 슬롯일 때 표시할 기본 이미지

    private string prefabPath = "Items/Skills/"; 


    void OnEnable()
    {
        RefreshSkillSlots();
        EventManager.Instance.StartList("SkillSlotChanged", RefreshSkillSlots);
    }
    void OnDisable()
    {
        EventManager.Instance.StopList("SkillSlotChanged", RefreshSkillSlots);
    }

    public void RefreshSkillSlots()
    {
        int[] skillSlots = DataManager.Instance.SkillSlot;

        for (int i = 0; i < slotIcons.Length; i++)
        {
            int skillID = skillSlots[i];

            
            if (skillID <= 0)
            {
                slotIcons[i].sprite = emptySlotSprite;
                slotIcons[i].color = new Color(1, 1, 1, 0);
                continue;
            }

            
            GameObject skillPrefab = Resources.Load<GameObject>(prefabPath + skillID.ToString());

            if (skillPrefab != null && skillPrefab.TryGetComponent(out SkillDataSo skillData))
            {
                
                slotIcons[i].sprite = GetSkillIcon(skillPrefab);
                slotIcons[i].color = Color.white;
            }
            else
            {
                slotIcons[i].sprite = emptySlotSprite;
                Debug.LogWarning($"슬롯 {i}: {skillID} 프리팹 로드 실패");
            }
        }
    }

    private Sprite GetSkillIcon(GameObject prefab)
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
        if(BaseSkillWindow.pickedSkill != null)
        {
            int skillID = BaseSkillWindow.pickedSkill.ID;

            DataManager.Instance.SkillSlot[index] = skillID;
            EventManager.Instance.TriggerEvent("SkillSlotChanged");

            BaseSkillWindow.pickedSkill = null;
        }
    }
}