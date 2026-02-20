using UnityEngine;

[CreateAssetMenu(menuName = "BossRush/BossData")]
public class BossData : ScriptableObject
{
    public GameObject bossPrefab;
    public string bossName;
    public int difficultyLevel;
}
