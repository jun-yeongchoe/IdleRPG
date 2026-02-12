using UnityEngine;

[CreateAssetMenu(fileName = "New Skill", menuName = "Skill Data")]
public class SkillData : ScriptableObject
{
    public string skillName; // 스킬 이름
    public Sprite skillIcon; // 아이콘 이미지
    public float cooldownTime; // 쿨타임
    public float durationTime; // 지속시간 (0이면 즉발)
    public bool isUnlocked; // 해금 여부 (테스트용)

    [TextArea]
    public string skillDesc;
}