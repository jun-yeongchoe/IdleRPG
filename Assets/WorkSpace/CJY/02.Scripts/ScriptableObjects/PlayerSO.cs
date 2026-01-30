using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStatus", menuName = "ScriptableObjects/PlayerStatus")]
public class PlayerStatus : ScriptableObject
{
    public string userName;
    public int gold;
    public int stageLevel;
    public int attackPower;

    // 서버에 보낼 때 사용할 JSON용 클래스로 변환하는 함수
    public UserData ToUserData()
    {
        return new UserData(userName, gold, stageLevel, attackPower);
    }
}