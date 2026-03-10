using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Skill_ToolBar_Slot : MonoBehaviour
{
    [SerializeField] Image[] slot;
    [SerializeField] SkillDataSo[] skillPrefabs;
    SkillDataSo currentSkill;

    private string slotChangeEvent = "SkillSlotChanged";

    void Start()
    {
        EventManager.Instance.StartList(slotChangeEvent, SkillToolBarBinding);
    }
    void OnDestroy()
    {
        EventManager.Instance.StopList(slotChangeEvent, SkillToolBarBinding);
    }

    private void SkillToolBarBinding()
    {
        int[] skillSlots = DataManager.Instance.SkillSlot;

        for(int i = 0; i< slot.Length; i++)
        {
            int currentSkillID = skillSlots[i];
            if(currentSkillID <= 0)
            {
                slot[i].sprite = null;
                slot[i].color = new Color(1,1,1,0);
                continue;
            }
            bool isFound = false;
            foreach(var skill in skillPrefabs)
            {
                if(skill != null && skill.ID == currentSkillID)
                {
                    Transform icon = skill.transform.Find("IconImage");
                    if (icon != null && icon.TryGetComponent(out Image iconImg))
                    {
                        slot[i].sprite = iconImg.sprite;
                        slot[i].color = Color.white;
                        isFound = true;
                    }
                    break;
                }
            }

            if (!isFound)
            {
                slot[i].sprite = null;
                slot[i].color = new Color(1, 1, 1, 0);
            }
        }
    }
}
