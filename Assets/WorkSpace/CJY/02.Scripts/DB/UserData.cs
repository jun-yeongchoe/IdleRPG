[System.Serializable]
public class UserData
{

    //이름, 보유한 코인, 보유한 보석, 공격력, 체력, 체젠, 공속, 치확, 치피
    public string userName;
    public int gold, jewel, atkPower, hp;
    public float atkSpeed, hpGen, criticalChance, criticalDamage;
    public UserData(string name, int gold, int jewel, int atkPower, int hp, float atkSp, float hpGen, float crtChance, float crtDmg)
    {
        this.userName = name;
        this.gold = gold;
        this.jewel = jewel;
        this.atkPower = atkPower;
        this.hp = hp;
        this.atkSpeed = atkSp;
        this.hpGen = hpGen;
        this.criticalChance = crtChance;
        this.criticalDamage = crtDmg;
    }
}