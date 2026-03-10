using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillInfoPopup : MonoBehaviour
{
    public static SkillInfoPopup Instance;

    [Header("UI 연결")]
    public Image iconImage;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descText; // 설명
    // 경험치 관련 UI도 연결 필요

    [Header("버튼 연결")]
    public GameObject btnEquip;   // 장착 버튼 오브젝트
    public GameObject btnUnequip; // 해제 버튼 오브젝트
    public Button closeButton;

    private SkillData currentData; // 지금 보고 있는 스킬

    void Awake()
    {
        Instance = this;
        // 버튼 리스너 연결 등...
        if (closeButton != null) closeButton.onClick.AddListener(ClosePopup);
    }
    void Start()
    {
        gameObject.SetActive(false);
    }

    public void Open(SkillData data, bool isEquippedSlot)
    {
        currentData = data;
        gameObject.SetActive(true); // 팝업 켜기

        // 정보
        iconImage.sprite = data.skillIcon;
        nameText.text = data.skillName;
        descText.text = data.skillDesc; // 추가한 설명 표시

        if (isEquippedSlot)
        {
            // 해제 버튼 노출
            btnEquip.SetActive(false);
            btnUnequip.SetActive(true);
        }
        else
        {
            // 장착 버튼 노출
            btnEquip.SetActive(true);
            btnUnequip.SetActive(false);
        }
    }

    public void ClosePopup()
    {
        gameObject.SetActive(false);
    }

    // 장착 버튼
    public void OnClickEquip()
    {
        ClosePopup();
    }

    // 해제 버튼
    public void OnClickUnequip()
    {
        ClosePopup();
    }
}