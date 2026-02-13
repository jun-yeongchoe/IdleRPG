using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
public class CommonPopup : MonoBehaviour
{
    public static CommonPopup Instance;

    [Header("UI 연결")]
    [SerializeField] private GameObject contentObj; // 팝업 전체 부모 (껏다킬거)
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI bodyText;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button cancelButton;
    [SerializeField] private TextMeshProUGUI confirmText; // 버튼 글씨 변경용 (확인/네 등)

    private Action onConfirmCallback; // 확인 눌렀을 때 실행할 함수 저장소

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬 넘어가도 살아있게
        }
        else Destroy(gameObject);

        // 버튼 리스너
        confirmButton.onClick.AddListener(OnConfirm);
        if (cancelButton != null) cancelButton.onClick.AddListener(OnCancel);

        contentObj.SetActive(false); // 시작할 땐 꺼둠
    }

    // 1. 확인 버튼만 있는 알림창 (예: 돈 부족함!)
    public void ShowAlert(string title, string body, string btnText = "확인")
    {
        titleText.text = title;
        bodyText.text = body;
        confirmText.text = btnText;

        onConfirmCallback = null; // 실행할 거 없음

        if (cancelButton != null) cancelButton.gameObject.SetActive(false); // 취소 버튼 숨김
        contentObj.SetActive(true);
    }

    // 2. 예/아니오 선택창 (예: 구매하시겠습니까?)
    public void ShowConfirm(string title, string body, Action onConfirm, string btnText = "네")
    {
        titleText.text = title;
        bodyText.text = body;
        confirmText.text = btnText;

        onConfirmCallback = onConfirm; // 실행할 함수 저장

        if (cancelButton != null) cancelButton.gameObject.SetActive(true); // 취소 버튼 보임
        contentObj.SetActive(true);
    }

    private void OnConfirm()
    {
        onConfirmCallback?.Invoke(); // 저장된 함수 실행
        Close();
    }

    private void OnCancel()
    {
        Close();
    }

    private void Close()
    {
        contentObj.SetActive(false);
    }
}
