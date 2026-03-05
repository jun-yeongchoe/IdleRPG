using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;

public class DwarfManager : MonoBehaviour
{
    public static DwarfManager Instance;

    [Header("던전 기획 설정")]
    public float timeLimit = 45f;          //제한 시간
    public float baseHp = 20000f;          //0구간 기본 체력
    public float hpMultiplier = 1.8f;      //구간별 체력 증가 배율
    public int maxSection = 15;            //최대 구간

    [Header("현재 진행 상태 (UI 표시용)")]
    public bool isPlaying = false;
    public float currentTime = 0f;
    public int currentSection = 0;
    public BigInteger currentBossHp = 0;
    public BigInteger currentBossMaxHp = 0;

    [Header("스폰 설정")]
    public GameObject bossPrefab;      //드워프 킹 프리팹
    public Transform spawnPoint;       //소환될 위치
    private GameObject currentBoss;    //현재 소환된 보스 추적용

    //UI 갱신용 이벤트
    public Action<float> OnTimeChanged;
    public Action<int, BigInteger, BigInteger> OnBossHpChanged; //(현재구간, 남은체력, 최대체력)

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    //던전 입장 로직
    public void EnterDungeon()
    {
        //입장 조건 체크
        if (DataManager.Instance.currentStageNum < 31) //예: 3챕터 클리어 조건 (31스테이지 진입)
        {
            CommonPopup.Instance.ShowAlert("입장 불가", "챕터 3을 클리어해야 해금됩니다.");
            return;
        }
        if (DataManager.Instance.DwarfKingTicket <= 0)
        {
            CommonPopup.Instance.ShowAlert("입장 불가", "입장권이 부족합니다.");
            return;
        }

        DataManager.Instance.DwarfKingTicket--;
        currentTime = timeLimit;
        currentSection = 0;

        CalculateNextSectionHp(); //0구간 체력 세팅

        if (currentBoss == null && bossPrefab != null && spawnPoint != null)
        {
            currentBoss = Instantiate(bossPrefab, spawnPoint.position, Quaternion.identity);
        }
        else if (currentBoss != null)
        {
            //이미 생성되어 있다면 위치만 초기화하고 다시 켜기 (최적화)
            currentBoss.transform.position = spawnPoint.position;
            currentBoss.SetActive(true);
        }

        isPlaying = true;

        Debug.Log("드워프 킹 던전 입장!");
    }

    //제한 시간 관리
    private void Update()
    {
        if (!isPlaying) return;

        currentTime -= Time.deltaTime;
        OnTimeChanged?.Invoke(currentTime); //타이머 UI 갱신

        if (currentTime <= 0)
        {
            EndDungeon(); //45초 끝
        }
    }

    //보스 피격 및 구간(Section) 처리(몬스터 스크립트에서 호출)
    public void OnBossTakeDamage(BigInteger damage)
    {
        if (!isPlaying) return;

        currentBossHp -= damage;

        //HP가 0 이하가 되면 다음 구간으로 넘어감
        while (currentBossHp <= 0 && currentSection < maxSection)
        {
            BigInteger overflowDamage = BigInteger.Abs(currentBossHp); //초과로 때린 데미지 보존

            currentSection++;
            CalculateNextSectionHp();

            currentBossHp -= overflowDamage; //다음 페이즈 체력에서 초과 데미지 깎기
        }

        //최대 구간 달성 시 체력을 0으로 고정하거나 던전 즉시 종료 처리 가능
        if (currentSection >= maxSection && currentBossHp <= 0)
        {
            currentBossHp = 0;
            EndDungeon();
        }

        OnBossHpChanged?.Invoke(currentSection, currentBossHp, currentBossMaxHp);
    }

    //다음 구간의 체력 계산 로직 (기획서 공식 적용)
    private void CalculateNextSectionHp()
    {
        double multiplier=System.Math.Pow(hpMultiplier,currentSection);
        //공식: 20000 * (1.8 ^ currentSection)
        currentBossMaxHp = (BigInteger)(baseHp * multiplier);
        currentBossHp = currentBossMaxHp;
    }

    //던전 종료 및 보상 지급
    private void EndDungeon()
    {
        isPlaying = false;

        if (currentBoss != null)
        {
            currentBoss.SetActive(false);
        }

        //보상 계산: 구간 * 2 (기획서 공식)
        int rewardScrap = currentSection * 2;

        //데이터 매니저에 재화 추가
        DataManager.Instance.AddScrap(rewardScrap);

        //결과 팝업 연동
        if (CommonPopup.Instance != null)
        {
            CommonPopup.Instance.ShowAlert(
                "던전 종료",
                $"드워프 킹 던전 탐벌 완료!\n\n도달 구간: {currentSection} 단계\n획득 스크랩: {rewardScrap}개",
                "확인",
                () => {
                    //원래 스테이지 로비로 복귀하는 로직 추가해주기
                    Debug.Log("로비 복귀");
                }
            );
        }
    }
}
