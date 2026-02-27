using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
public class ActiveSkill : MonoBehaviour
{
    string loadPath = $"Items/Skills/";

    [Header("Settings")]
    public Transform firePoint;
    public Transform testTarget;

    private List<SkillDataSo> _equippedSkills = new List<SkillDataSo>();
    private Dictionary<int, SkillDataSo> _skillDatabase = new Dictionary<int, SkillDataSo>();
    GameObject[] allPrefabs;

    [SerializeField] private int _currentSkillIndex = 0;

    public bool isAutoMode = false;
    private Dictionary<int, float> _skillCoolTimers = new Dictionary<int, float>();


    

    void Start()
    {
        allPrefabs = Resources.LoadAll<GameObject>(loadPath);

        foreach(var prefab in allPrefabs)
        {
            SkillDataSo data = prefab.GetComponent<SkillDataSo>();
            if(data != null)
            {
                if (!_skillDatabase.ContainsKey(data.ID))
                {
                    _skillDatabase.Add(data.ID, data);
                }
            }
        }

        EventManager.Instance.StartList("SkillSlotChanged", RefreshEquippedSKill);
        RefreshEquippedSKill();
    }

    void OnDestroy()
    {
        EventManager.Instance.StopList("SkillSlotChanged", RefreshEquippedSKill);
    }

    private void RefreshEquippedSKill()
    {
        int[] availableIds = { 
        10001, 10002, 10003, 10004, 
        11001, 11002, 11003, 11004, 
        12001, 12002, 12003, 12004, 
        13001, 13002, 13003, 13004, 
        14001, 14002, 14003, 14004, 
        15001, 15002, 15003, 15004, 
        16001, 16002, 16003, 16004 
        };

        if (DataManager.Instance != null)
        {
            // DataManager의 SkillSlot(크기 6)에 랜덤 ID 할당
            for (int i = 0; i < DataManager.Instance.SkillSlot.Length; i++)
            {
                int randomIndex = UnityEngine.Random.Range(0, availableIds.Length);
                DataManager.Instance.SkillSlot[i] = availableIds[randomIndex];
            }
        }
        
        _equippedSkills.Clear();
        if(DataManager.Instance == null) return;

        foreach(int skillId in DataManager.Instance.SkillSlot)
        {
            if(skillId != -1 && _skillDatabase.TryGetValue(skillId, out SkillDataSo skillData))
            {
                _equippedSkills.Add(skillData);
                if(!_skillCoolTimers.ContainsKey(skillId)) _skillCoolTimers.Add(skillId, -999f);
            }
        }

        _currentSkillIndex = 0;
        if(_equippedSkills.Count >0) Debug.Log($"스킬 슬롯 갱신 완료. 현재 장착 스킬 수 : {_equippedSkills.Count}");
    }

    void Update()
    {
        if(isAutoMode) HandleAutoSkill();

        if (Input.GetKeyDown(KeyCode.T))
        {
            RefreshEquippedSKill();
        }
    }

    public void ToggleAutoMode()
    {
        isAutoMode = !isAutoMode;
    }

    public void OnClickSkillSlot(int index)
    {
        if(_equippedSkills.Count == 0)
        {
            Debug.LogWarning("장착된 스킬이 없습니다.");
            return;
        }

        if (index>=0 && index < _equippedSkills.Count)
        {
            if (UseSkill(index))
            {
                Debug.Log($"{index}번 슬롯의 스킬({_equippedSkills[index].Name_KR}) 수동 발사 성공");
            }
            else
            {
                Debug.Log($"{_equippedSkills[index].Name_KR} 스킬은 현재 쿨타임 중입니다.");
            }
        }
        else
        {
            Debug.LogWarning($"{index}번 슬롯에 장착된 스킬 데이터가 없습니다.");
        }
    }

    public bool UseSkill(int skillIdx)
    {
        if(skillIdx <0 || skillIdx >= _equippedSkills.Count) return false;
        SkillDataSo skill = _equippedSkills[skillIdx];

        if(Time.time >= _skillCoolTimers[skill.ID] + skill.Cooltime)
        {
            ExecuteSkill(skill);
            _skillCoolTimers[skill.ID] = Time.time;
            return true;
        }
        return false;
    }

    private void ExecuteSkill(SkillDataSo skill)
    {
        // 데미지 계산: FinalAtk * DamageCoef
        // BigInteger와 float 연산을 위해 100을 곱하고 나누는 방식 사용
        BigInteger finalAtk = PlayerStatCalculator.instance.FinalAtk;
        BigInteger damagePerStrike = (finalAtk * (long)(skill.Damage_Coef * 100)) / 100;

        // StrikeCount만큼 이펙트 반복 실행
        // 연타형 스킬일 경우간격을 두기 위해 코루틴을 사용할 수도 있지만, 
        // 우선은 요구하신 대로 StrikeCount 횟수만큼 즉시/반복 호출합니다.
        StartCoroutine(ProcessStrikes(skill, firePoint, testTarget, damagePerStrike));
    }

    // 자동 사용
    private void HandleAutoSkill()
    {
        for(int i = 0; i < _equippedSkills.Count; i++)
        {
            UseSkill(i);
        }
    }


    // public void Active(SkillDataSo skillData, Transform firePoint, Transform target)
    // {
    //     if (skillData == null) return;

    //     // 데미지 계산: FinalAtk * DamageCoef
    //     // BigInteger와 float 연산을 위해 100을 곱하고 나누는 방식 사용
    //     BigInteger finalAtk = PlayerStatCalculator.instance.FinalAtk;
    //     BigInteger damagePerStrike = (finalAtk * (long)(skillData.Damage_Coef * 100)) / 100;

    //     // StrikeCount만큼 이펙트 반복 실행
    //     // 연타형 스킬일 경우간격을 두기 위해 코루틴을 사용할 수도 있지만, 
    //     // 우선은 요구하신 대로 StrikeCount 횟수만큼 즉시/반복 호출합니다.
    //     StartCoroutine(ProcessStrikes(skillData, firePoint, target, damagePerStrike));
    // }

    private IEnumerator ProcessStrikes(SkillDataSo skillData, Transform firePoint, Transform target, BigInteger damage)
    {
        for (int i = 0; i < skillData.StrikeCount; i++)
        {
            SpawnEffect(skillData, firePoint, target);
            
            // 여기서 실제로 타겟에게 데미지를 전달하는 로직
            Debug.Log($"{skillData.Name_KR} 적중! 데미지: {damage} (타격: {i + 1}/{skillData.StrikeCount})");

            yield return new WaitForSeconds(0.1f);
        }
        // yield break;
    }

    private void SpawnEffect(SkillDataSo skillData, Transform firePoint, Transform target)
    {
        UnityEngine.Vector3 spawnPos = UnityEngine.Vector3.zero;

        switch (skillData.spawnPoint)
        {
            case SkillSpawnPoint.Player:
                spawnPos = firePoint.position;
                break;
            case SkillSpawnPoint.Target:
                spawnPos = target.position-UnityEngine.Vector3.up * 0.5f;
                break;
            case SkillSpawnPoint.Up:
                spawnPos = target.position + UnityEngine.Vector3.up * 4f;
                break;
        }

        EffectManager.Instance.PlayEffect(skillData.EffectPrefabName, spawnPos);
    }
}
