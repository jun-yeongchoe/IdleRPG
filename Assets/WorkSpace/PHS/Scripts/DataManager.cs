using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Numerics;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set; }

    public enum BackGroundType
        {
            //타입 이름 정하면 그 때 수정 예정
            ABCDEFG=0,
            HIJKLMNOP=1,
            QRSTUV=2
        }

    //실제 스테이지 번호
    public int currentStageNum = 1;

    //챕터당 스테이지 개수
    public int StageCount = 10;

    //배경 개수
    public int backgroundCount = 3;

    //계산 로직(배경 순환: 10스테이지=1챕터, 매 챕터 클리어시 배경 전환을 위한 함수)
    public BackGroundType BackgroundIndex()
    { 
        int chapterNum = (currentStageNum-1)/StageCount;    //1~10는 0, 11~20은 1챕터 이런식
        int index= chapterNum%backgroundCount;                  //11, 111, 10001 등등 커져도 문제x

        return (BackGroundType)index;
    }

    //경제 변수 이름 바뀌면 다시 수정할것
    [Header("경제")]
    public BigInteger Gold = 0;
    public BigInteger Scrap = 0;
    public BigInteger Gem = 0;

    //스텟도 변수명 바뀌면 다시 수정할것
    //JSON으로 저장할지, playerprefs로 저장할지는 팀장이랑 상의 후 정할것
    [Header("스텟")]
    public int AtkLv = 1;
    public int HpLv = 1;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this; DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    public BigInteger GetGoldperSec()
    {
        BigInteger baseGold = 10;
        BigInteger stageBonus = currentStageNum * 5;

        return baseGold + stageBonus;
    }

    public void AddGold(BigInteger amount)
    {
        Gold += amount;
        if (EventManager.Instance != null) EventManager.Instance.TriggerEvent("CurrencyChange");
    }
    public void AddScrap(BigInteger amount)
    {
        Scrap += amount;
        if (EventManager.Instance != null) EventManager.Instance.TriggerEvent("CurrencyChange");
    }

    public void AddGem(BigInteger amount)
    {
        Gem += amount;
        if (EventManager.Instance != null) EventManager.Instance.TriggerEvent("CurrencyChange");
    }
}
