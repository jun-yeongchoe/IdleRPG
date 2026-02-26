using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DwarfManager : MonoBehaviour
{
    public static DwarfManager Instance;

    [Header("던전 기획 설정")]
    public float timeLimit = 45f;          // 제한 시간
    public float baseHp = 20000f;          // 0구간 기본 체력
    public float hpMultiplier = 1.8f;      // 구간별 체력 증가 배율
    public int maxSection = 15;            // 최대 구간

    [Header("현재 진행 상태 (UI 표시용)")]
    public bool isPlaying = false;
    public float currentTime = 0f;
    public int currentSection = 0;
    public float currentBossHp = 0f;
    public float currentBossMaxHp = 0f;

    // UI 갱신용 이벤트 (옵저버 패턴 활용)
    public Action<float> OnTimeChanged;
    public Action<int, float, float> OnBossHpChanged; // (현재구간, 남은체력, 최대체력)

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // 1. 던전 입장 로직 (UI 버튼에서 호출)
    public void EnterDungeon()
    {
        // 입장 조건 체크 (챕터 3 클리어 & 입장권 확인)
        if (DataManager.Instance.currentStageNum < 31) // 예: 3챕터 클리어 조건 (31스테이지 진입)
        {
            CommonPopup.Instance.ShowAlert("입장 불가", "챕터 3을 클리어해야 해금됩니다.");
            return;
        }
        if (DataManager.Instance.DwarfKingTicket <= 0)
        {
            CommonPopup.Instance.ShowAlert("입장 불가", "입장권이 부족합니다.");
            return;
        }

        // 입장권 차감 및 변수 초기화
        DataManager.Instance.DwarfKingTicket--;
        currentTime = timeLimit;
        currentSection = 0;

        CalculateNextSectionHp(); // 0구간 체력 세팅
        isPlaying = true;

        // TODO: 보스 소환 (EnemySpawner 재활용) 및 인게임 던전 UI 켜기
        Debug.Log("드워프 킹 던전 입장!");
    }

    // 2. 제한 시간 관리 (Update)
    private void Update()
    {
        if (!isPlaying) return;

        currentTime -= Time.deltaTime;
        OnTimeChanged?.Invoke(currentTime); // 타이머 UI 갱신

        if (currentTime <= 0)
        {
            EndDungeon(); // 45초 끝!
        }
    }

    // 3. 보스 피격 및 구간(Section) 처리 (몬스터 스크립트에서 호출)
    public void OnBossTakeDamage(float damage)
    {
        if (!isPlaying) return;

        currentBossHp -= damage;

        // HP가 0 이하가 되면 다음 구간으로 넘어감 (핵심 로직)
        while (currentBossHp <= 0 && currentSection < maxSection)
        {
            float overflowDamage = Mathf.Abs(currentBossHp); // 초과해서 때린 데미지 보존

            currentSection++;
            CalculateNextSectionHp();

            currentBossHp -= overflowDamage; // 다음 페이즈 체력에서 초과 데미지 깎기
        }

        // 최대 구간 달성 시 체력을 0으로 고정하거나 던전 즉시 종료 처리 가능
        if (currentSection >= maxSection && currentBossHp <= 0)
        {
            currentBossHp = 0;
            EndDungeon();
        }

        OnBossHpChanged?.Invoke(currentSection, currentBossHp, currentBossMaxHp);
    }

    // 다음 구간의 체력 계산 로직 (기획서 공식 적용)
    private void CalculateNextSectionHp()
    {
        // 공식: 20000 * (1.8 ^ currentSection)
        currentBossMaxHp = baseHp * Mathf.Pow(hpMultiplier, currentSection);
        currentBossHp = currentBossMaxHp;
    }

    // 4. 던전 종료 및 보상 지급
    private void EndDungeon()
    {
        isPlaying = false;

        // 보상 계산: 구간 * 2 (기획서 공식)
        int rewardScrap = currentSection * 2;

        // 데이터 매니저에 재화 추가 (아까 만든 AddScrap 등 활용)
        DataManager.Instance.AddScrap(rewardScrap);

        // 결과 팝업 연동
        if (CommonPopup.Instance != null)
        {
            CommonPopup.Instance.ShowAlert(
                "던전 종료",
                $"드워프 킹 던전 탐벌 완료!\n\n도달 구간: {currentSection} 단계\n획득 스크랩: {rewardScrap}개",
                "확인",
                () => {
                    // TODO: 원래 스테이지 로비로 복귀하는 로직
                    Debug.Log("로비 복귀");
                }
            );
        }
    }
}
