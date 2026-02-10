using UnityEngine;

public enum DungeonType
{
    Gold,
    BossRush,
    DwarfKing
}

[CreateAssetMenu(
    fileName = "DungeonRuleData",
    menuName = "Dungeon/Dungeon Rule"
)]
public class DungeonRuleData : ScriptableObject
{
    [Header("Dungeon Info")]
    public DungeonType dungeonType;

    [Header("Fail Conditions")]
    public bool failOnPlayerDeath = true;
    public bool hasTimeLimit = false;
    public float timeLimit = 60f;
}
