using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DwarfUI : MonoBehaviour
{
    [Header("패널 관리")]
    public GameObject entryPanel;   //입장 전 로비 화면
    public GameObject inGamePanel;  //던전 진행 중 화면 (타이머, 체력바)

    [Header("입장 UI")]
    public TextMeshProUGUI ticketText; //"입장권: 2/2"
    public Button enterButton;         //입장 버튼
    public Button closeButton;         //닫기 버튼

    [Header("진행 UI")]
    public TextMeshProUGUI timerText;    //남은 시간
    public TextMeshProUGUI sectionText;  //현재 도달 구간
    public TextMeshProUGUI bossHpText;   //보스 체력 텍스트 (예: 15000 / 20000)
    public Image bossHpFill;             //보스 체력바 게이지 (Image Type: Filled)

    private void Start()
    {
        //버튼 이벤트 연결
        if (enterButton != null) enterButton.onClick.AddListener(OnClickEnter);
        if (closeButton != null) closeButton.onClick.AddListener(OnClickClose);

        //매니저 이벤트 구독
        if (DwarfManager.Instance != null)
        {
            DwarfManager.Instance.OnTimeChanged += UpdateTimerUI;
            DwarfManager.Instance.OnBossHpChanged += UpdateBossHpUI;
        }

        //초기 화면 세팅
        ShowEntryPanel();
    }

    private void OnDestroy()
    {
        //메모리 누수 방지용 구독 해제
        if (DwarfManager.Instance != null)
        {
            DwarfManager.Instance.OnTimeChanged -= UpdateTimerUI;
            DwarfManager.Instance.OnBossHpChanged -= UpdateBossHpUI;
        }
    }

    //던전 UI를 열 때 호출하는 함수(UIManager 등에서 호출)
    public void ShowEntryPanel()
    {
        entryPanel.SetActive(true);
        inGamePanel.SetActive(false);

        // 현재 입장권 개수 표시
        if (DataManager.Instance != null)
        {
            ticketText.text = $"남은 입장권: {DataManager.Instance.DwarfKingTicket}장";
        }
    }

    private void OnClickEnter()
    {
        // 1. 매니저에게 입장 처리 지시
        if (DwarfManager.Instance != null)
        {
            DwarfManager.Instance.EnterDungeon();

            // 2. 입장이 성공적으로 되었다면 (isPlaying == true), 인게임 UI로 전환
            if (DwarfManager.Instance.isPlaying)
            {
                entryPanel.SetActive(false);
                inGamePanel.SetActive(true);
            }
        }
    }

    private void OnClickClose()
    {
        // 던전 창 닫기
        gameObject.SetActive(false);
    }

    private void UpdateTimerUI(float remainTime)
    {
        //소수점 둘째 자리까지 표시 (예: 42.15초)
        timerText.text = $"남은 시간: {remainTime:F2}초";
    }

    private void UpdateBossHpUI(int section, BigInteger currentHp, BigInteger maxHp)
    {
        sectionText.text = $"현재 구간: {section} 단계";

        bossHpText.text = $"{currentHp} / {maxHp}";

        //체력바 게이지 비율(0~1) 갱신
        if (maxHp > 0)
        {
            bossHpFill.fillAmount = (float)((double)currentHp / (double)maxHp);
        }
    }
}
