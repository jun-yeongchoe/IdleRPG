using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class GachaResultPopup : MonoBehaviour
{
    public static GachaResultPopup Instance;

    [Header("UI 연결")]
    public GameObject popupPanel;     //팝업창 전체 배경 (평소엔 꺼둠)
    public Transform contentTarget;   //슬롯들이 생성될 부모 객체 (Grid Layout Group이 달린 곳)
    public GameObject slotPrefab;     //아까 만든 GachaResultSlot 프리팹
    public Button closeButton;        //닫기 버튼

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        //시작할 때 팝업 숨기기 및 버튼 연결
        popupPanel.SetActive(false);
        if (closeButton != null) closeButton.onClick.AddListener(ClosePopup);
    }

    //ShopManager가 뽑기 끝난 후 이 함수를 호출
    public void ShowResult(Dictionary<int, int> results)
    {
        foreach (Transform child in contentTarget)
        {
            Destroy(child.gameObject);
        }

        //새로운 결과 슬롯 생성
        foreach (var pair in results)
        {
            //슬롯 하나 복사해서 부모(contentTarget) 밑에 붙임
            GameObject slotObj = Instantiate(slotPrefab, contentTarget);

            GachaResult slot = slotObj.GetComponent<GachaResult>();
            if (slot != null)
            {
                slot.Setup(pair.Key, pair.Value); //Key = ID, Value = 획득 개수
            }
        }

        popupPanel.SetActive(true);
    }

    private void ClosePopup()
    {
        popupPanel.SetActive(false);
    }
}