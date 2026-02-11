using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_SkillItem : MonoBehaviour
{
    [Header("UI 연결")]
    public Image iconImage; //스킬 아이콘
    public TextMeshProUGUI levelText; //LV
    public GameObject equippedMark; //장착중 표시

    // 스킬 데이터
    private SkillData mySkillData;

    public void Init(SkillData data, int level, bool isEquipped)
    {
        mySkillData = data;

        if (mySkillData != null)
        {
            iconImage.sprite = mySkillData.skillIcon;
        }
        UpdateUI(level, isEquipped);
    }

    // UI 갱신
    public void UpdateUI(int level, bool isEquipped)
    {
        levelText.text = $"LV.{level}";

        if (equippedMark != null)
            equippedMark.SetActive(isEquipped);
    }

    //버튼 클릭 시 실행할 함수
    public void OnClickItem()
    {
        //팝업 관리자 호출
        if(SkillInfoPopup.Instance!=null && mySkillData!=null)
        { 
            //일단 false로 두고 스킬 매니저 연결시 수정예정
            SkillInfoPopup.Instance.Open(mySkillData,false);
        }
    }
}