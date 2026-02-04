using UnityEngine;

[CreateAssetMenu(
    fileName = "StageData",
    menuName = "Game/Stage Data"
)]
public class StageData : ScriptableObject
{
    [Header("Stage Info")]
    public string stageName;

    [Header("Waves")]
    public WaveData[] waves;
}
