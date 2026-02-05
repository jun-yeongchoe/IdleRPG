using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStatus", menuName = "ScriptableObjects/PlayerStatus")]
public class PlayerStatus : ScriptableObject
{
    public string userName;
    public int gold, gem, atkPower, hp, hpGen, atkSpeed, criticalChance, criticalDamage;

    public float invenShopExp, skillShopExp, partnerShopExp;

    // 서버에 보낼 때 사용할 JSON용 클래스로 변환하는 함수
    public UserData ToUserData()
    {
        return new UserData(userName,gold, gem, atkPower, hp,atkSpeed, hpGen, criticalChance, criticalDamage, invenShopExp, skillShopExp, partnerShopExp);
    }
}