using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class UIItem : MonoBehaviour
{
    [Header("UI 연결")]
    public TextMeshProUGUI idText;
    public TextMeshProUGUI countText;
    public GameObject equipMark;
    public Button slotButton;

    private int myItemID;
    private System.Action<int> onClickCallback;

    public void Setup(int itemID, int count, bool isEquipped, System.Action<int> onClickAction)
    {
        myItemID = itemID;

        //텍스트 세팅
        idText.text = $"ID: {itemID}";
        countText.text = $"x{count}";

        //장착 중이면 장착 마크(E) 켜기
        if (equipMark != null) equipMark.SetActive(isEquipped);

        onClickCallback = onClickAction;
        slotButton.onClick.RemoveAllListeners();
        slotButton.onClick.AddListener(() => onClickCallback?.Invoke(myItemID));
    }
}
