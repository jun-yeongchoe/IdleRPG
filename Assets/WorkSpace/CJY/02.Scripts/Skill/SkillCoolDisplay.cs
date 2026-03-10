using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillCoolDisplay : MonoBehaviour
{
    [SerializeField] private Image[] coolImage;
    [SerializeField] private ActiveSkill activeSkill;


    void Update()
    {
        if (activeSkill == null) return;

        UpdateCooltimeUI();
    }

    private void UpdateCooltimeUI()
    {
        
        var equippedSkills = activeSkill.GetEquippedSkills();

        for (int i = 0; i < coolImage.Length; i++)
        {
            
            if (i >= equippedSkills.Count)
            {
                coolImage[i].fillAmount = 0;
                continue;
            }

            SkillDataSo skill = equippedSkills[i];
            float lastUsedTime = activeSkill.GetLastUsedTime(skill.ID);
            float coolDuration = skill.Cooltime;

            
            float elapsedTime = Time.time - lastUsedTime;

            if (elapsedTime < coolDuration)
            {
                
                float remainingRatio = 1.0f - (elapsedTime / coolDuration);
                coolImage[i].fillAmount = remainingRatio;
            }
            else
            {
                coolImage[i].fillAmount = 0;
            }
        }
    }
}
