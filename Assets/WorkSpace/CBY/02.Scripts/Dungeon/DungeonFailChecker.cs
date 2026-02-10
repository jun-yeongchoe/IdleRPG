using UnityEngine;
using System;

public class DungeonFailChecker : MonoBehaviour
{
    public DungeonRuleData ruleData;
    public float elapsedTime;

    public Action OnDungeonFail;

    private bool isFailed = false;

    private void Update()
    {
        if (isFailed) return;

        CheckTimeLimit();
        CheckPlayerDeath();
    }

    void CheckTimeLimit()
    {
        if (!ruleData.hasTimeLimit) return;

        elapsedTime += Time.deltaTime;

        if (elapsedTime >= ruleData.timeLimit)
        {
            FailDungeon("Time Over");
        }
    }

    void CheckPlayerDeath()
    {
        if (!ruleData.failOnPlayerDeath) return;

        if (IsPlayerDead())
        {
            FailDungeon("Player Dead");
        }
    }

    void FailDungeon(string reason)
    {
        isFailed = true;
        Debug.Log($"Dungeon Failed: {reason}");
        OnDungeonFail?.Invoke();
    }

    bool IsPlayerDead()
    {
        // 나중에 PlayerManager / PlayerHealth로 교체
        return false;
    }
}