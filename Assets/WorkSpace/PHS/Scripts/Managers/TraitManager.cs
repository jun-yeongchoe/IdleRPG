using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class TraitInfo
{
    public int id;                 //예: 70001
    public string rank;            //예: "F", "SS"
    public string type;            //예: "Attack_Damage"
    public float rate;             //예: 0.1, 3.0
    public string applicationType; //예:"M"(곱연산), "P"(합연산)
}

public class TraitManager : MonoBehaviour
{
    public static TraitManager Instance;

    [Header("특성 DB (팀원이 파싱해서 채워줄 곳)")]
    public Dictionary<int, TraitInfo> traitDB = new Dictionary<int, TraitInfo>();

    [Header("특성 재설정 비용")]
    public int baseRerollCost = 100; //1회 돌릴 때 기본 스크랩 비용

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (DataManager.Instance != null && DataManager.Instance.TraitSlots[0] == null)
        {
            for (int i = 0; i < 5; i++)
            {
                DataManager.Instance.TraitSlots[i] = new TraitSaveData { slotIndex = i, traitId = 0, isLocked = false };
            }
        }
    }

    public void RollTraits()
    {
        if (DataManager.Instance == null || traitDB.Count == 0)
        {
            Debug.LogWarning("데이터매니저가 없거나 특성 DB가 비어있습니다.");
            return;
        }

        //잠긴 슬롯 개수에 따라 비용 증가 (잠긴 개수 + 1 배)
        int lockedCount = DataManager.Instance.TraitSlots.Count(slot => slot.isLocked);
        int totalCost = baseRerollCost * (lockedCount + 1);

        //재화(Scrap) 부족 체크(BigInteger 비교)
        if (DataManager.Instance.Scrap < totalCost)
        {
            Debug.Log("스크랩이 부족합니다!");
            if (CommonPopup.Instance != null)
                CommonPopup.Instance.ShowAlert("알림", "스크랩이 부족합니다.", "확인");
            return;
        }

        //재화 차감(DataManager의 AddScrap 함수를 이용해 마이너스 처리 -> UI 자동 갱신됨!)
        DataManager.Instance.AddScrap(-totalCost);

        //DB에 있는 모든 특성 ID 가져오기
        List<int> allTraitIds = traitDB.Keys.ToList();

        //잠기지 않은 슬롯만 랜덤 뽑기
        foreach (var slot in DataManager.Instance.TraitSlots)
        {
            if (!slot.isLocked)
            {
                int randomIdx = Random.Range(0, allTraitIds.Count);
                slot.traitId = allTraitIds[randomIdx];
            }
        }

        Debug.Log($"특성 재설정 완료! (소모 스크랩: {totalCost})");

        //나중에 UI 만들면 연결
    }

    //캐릭터 스탯 적용 시 호출해서 쓸 총합 계산기
    public float GetTotalStatBonus(string statType, string appType)
    {
        float total = 0f;
        foreach (var slot in DataManager.Instance.TraitSlots)
        {
            if (slot.traitId != 0 && traitDB.ContainsKey(slot.traitId))
            {
                TraitInfo info = traitDB[slot.traitId];
                if (info.type == statType && info.applicationType == appType)
                {
                    total += info.rate;
                }
            }
        }
        return total;
    }
}
