using UnityEngine;
using UnityEngine.UI;

public class SkillSlot : MonoBehaviour
{
    [Header("슬롯 설정")]
    // 해금 스테이지 설정
    public int unlockStage = 1;

    [Header("장착된 스킬 확인용")]
    public SkillData skillData;

    // 슬롯 해금 여부
    public bool isSlotUnlocked = false;

    [Header("UI 연결")]
    public Image iconImage; // 스킬 아이콘 이미지
    public Image backgroundImage; // 배경 이미지
    public Image durationCover; // 지속시간 관련
    public Image cooldownCover; // 쿨타임 관련
    public GameObject lockObj; // 락상태 관련

    private float currentTimer = 0f; // 현재 남은 시간 (지속시간 or 쿨타임)
    private bool isDurationState = false; // 현재 지속시간중?
    private bool isCooldownState = false; // 현재 쿨타임중?

    public void RefreshSlotState()
    {
        if (SkillManager.Instance != null)
        {
            isSlotUnlocked = SkillManager.Instance.IsSlotUnlocked(unlockStage);
        }
        else
        {
            // 매니저가 없으면 일단 잠금
            isSlotUnlocked = false;
        }

        // 락 UI 키거나 끄기
        if (lockObj != null) lockObj.SetActive(!isSlotUnlocked);

        // 잠김 상태라면 모두 초기화
        if (!isSlotUnlocked)
        {
            if (iconImage != null) iconImage.gameObject.SetActive(false); // 아이콘 숨김
            if (backgroundImage != null) backgroundImage.color = Color.white; // 배경색 복구
            if (durationCover != null) durationCover.gameObject.SetActive(false); // 지속시간 숨김
            if (cooldownCover != null) cooldownCover.fillAmount = 0; // 쿨타임 초기화

            // 변수들도 리셋
            isDurationState = false;
            isCooldownState = false;
            currentTimer = 0;
        }
        else
        {
            // 해금했다면 데이터 상태에 따라 출력
            if (iconImage != null)
            {
                // 스킬 데이터가 있으면 아이콘을 켜고, 없으면 끔
                iconImage.gameObject.SetActive(skillData != null);

                // 이미지가 빠져있을 수 있으니, 다시 넣어줌
                if (skillData != null) iconImage.sprite = skillData.skillIcon;
            }
        }
    }

    // 툴바에서 슬롯에 데이터 넣어주는
    public void SetSkill(SkillData data)
    {
        skillData = data; // 데이터를 내 변수에 저장

        // 데이터가 들어오면 일단 적용
        if (skillData != null && iconImage != null)
        {
            iconImage.sprite = skillData.skillIcon;
        }

        // 잠금이면 숨김, 해금이면 노출
        RefreshSlotState();

        // 해금 상태에서 새로운 스킬 착용시 재시작
        if (isSlotUnlocked && skillData != null)
        {
            currentTimer = 0;
            isDurationState = false;
            isCooldownState = false;
            if (cooldownCover != null) cooldownCover.fillAmount = 0;
            if (durationCover != null) durationCover.gameObject.SetActive(false);
            if (backgroundImage != null) backgroundImage.color = Color.white;
        }
    }

    // 오토기능용 확인
    public bool IsReady()
    {
        // 잠겨있거나, 스킬 데이터가 없으면 사용 불가
        if (!isSlotUnlocked || skillData == null) return false;

        // 지속시간 중이거나, 쿨타임 중이면 사용 불가
        return !isDurationState && !isCooldownState;
    }

    // 스킬 사용 함수
    public void UseSkill()
    {
        if (!IsReady()) return; // 준비 안 됐으면 무시

        // 지속시간 있는 스킬
        if (skillData.durationTime > 0)
        {
            isDurationState = true; // 지속 상태 시작
            currentTimer = skillData.durationTime; // 타이머 설정

            // 일단 코드로 연출. 노란색 
            backgroundImage.color = Color.yellow;
            durationCover.fillAmount = 1.0f;
            durationCover.gameObject.SetActive(true);
        }
        // 즉발 스킬
        else
        {
            StartCooldown(); // 바로 쿨타임 시작
        }
    }

    // 타이머 계산
    void Update()
    {
        // 잠겨있거나 스킬이 없으면 계산할 필요 없음
        if (!isSlotUnlocked || skillData == null) return;

        // 지속시간 중일떄
        if (isDurationState)
        {
            currentTimer -= Time.deltaTime; // 시간 흐름
            durationCover.fillAmount = currentTimer / skillData.durationTime; // UI 연출 갱신

            // 지속시간 종료시
            if (currentTimer <= 0)
            {
                isDurationState = false; // 지속 상태 끝
                StartCooldown(); // 쿨타임 시작
            }
        }
        // 쿨타임 중일때
        else if (isCooldownState)
        {
            currentTimer -= Time.deltaTime; // 시간 흐름
            cooldownCover.fillAmount = currentTimer / skillData.cooldownTime; // UI 연출 갱신

            // 쿨타임 종료시
            if (currentTimer <= 0)
            {
                isCooldownState = false; // 쿨타임 끝
                cooldownCover.fillAmount = 0; // 비우기
            }
        }
    }

    // 쿨타임 시작
    private void StartCooldown()
    {
        isCooldownState = true;
        currentTimer = skillData.cooldownTime;

        // UI 원상복구 및 쿨타임 바 세팅
        backgroundImage.color = Color.white;
        durationCover.gameObject.SetActive(false);
        cooldownCover.fillAmount = 1.0f;
    }
}