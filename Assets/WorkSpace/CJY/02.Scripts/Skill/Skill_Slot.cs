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
    void Osable()
    {
        EventManager.Instance.StopList("SkillSlotChanged", RefreshSkillSlots);
    }

    public void RefreshSkillSlots()
    {
        int[] skillSlots = DataManager.Instance.SkillSlot;

        for (int i = 0; i < slotIcons.Length; i++)
        {
            int skillID = skillSlots[i];

            // 슬롯이 비어있거나 -1인 경우
            if (skillID <= 0)
            {
                slotIcons[i].sprite = emptySlotSprite;
                slotIcons[i].color = new Color(1, 1, 1, 0); // 완전히 투명하게 하거나
                continue;
            }

            // 스킬 ID가 있다면 리소스 로드
            // 기존 코드에서 프리팹 안의 SkillDataSo를 참조하므로 동일하게 접근합니다.
            GameObject skillPrefab = Resources.Load<GameObject>(prefabPath + skillID.ToString());

            if (skillPrefab != null && skillPrefab.TryGetComponent(out SkillDataSo skillData))
            {
                // SkillDataSo가 ScriptableObject라면 그 안의 icon을, 
                // 아니라면 프리팹의 Image 컴포넌트를 가져옵니다.
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
        // 프리팹 내부 구조에 따라 아이콘을 가져오는 방식
        // 예: 자식 오브젝트 중 IconImage의 스프라이트 추출
        var iconTransform = prefab.transform.Find("IconImage");
        if (iconTransform != null)
        {
            return iconTransform.GetComponent<Image>().sprite;
        }
        return null;
    }
}