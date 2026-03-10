using System.Numerics;

[System.Serializable]
public class UserData
{

    //이름, 보유한 코인, 보유한 보석, 공격력, 체력, 체젠, 공속, 치확, 치피
    //저장 필요한 데이터 : 이름, 보유 골드, 보유 보석, 기본공격력, 기본 체력, 기본공속, 기본 피젠, 기본 치확, 기본 치피
    //저장 필요 없는 데이터 : 증가된 공격력, 증가된 체력, 증가된 공속, 증가된 피젠, 증가된 치확, 증가된 치피

    //계산 해서 출력 해줘야하는데이터 : 현재 공격력, 현재 체력, 현재 공속, 현재 피젠, 현재 치확, 현재 치피
    //계산에 필요한 데이터 : 공격력/체력/공속/피젠/치확/치피의 레벨, 장비(무기/방어구)의 공격력/체력 증가치
    //추가 저장 데이터 : 현재 찍은 각 스탯별 레벨, 인벤토리(무기/방어구/동료/스킬), 현재 진행중이던 스테이지/챕터

    //수정해야할 포인트 : UserData, DGBManager, PlayerSO, DB
    public string userName;
    //골드, 보석, 스탯 레벨
    
    public BigInteger gold, gem;
    public int atkPower, hp, hpGen, atkSpeed, criticalChance, criticalDamage;

    public float invenShopExp, skillShopExp, partnerShopExp;

    public UserData(string name, BigInteger gold, BigInteger gem, int atkPower, int hp, int hpGen, int atkSp, int crtChance, int crtDmg, float invenExp, float skillExp, float partnerExp)
    {
        this.userName = name;
        this.gold = gold;
        this.gem = gem;
        this.atkPower = atkPower;
        this.hp = hp;
        this.hpGen = hpGen;
        this.atkSpeed = atkSp;
        this.criticalChance = crtChance;
        this.criticalDamage = crtDmg;
        this.invenShopExp = invenExp;
        this.skillShopExp = skillExp;
        this.partnerShopExp = partnerExp;
    }
}