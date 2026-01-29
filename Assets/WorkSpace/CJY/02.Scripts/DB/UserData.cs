[System.Serializable]
public class UserData
{
    public string userName;
    public int gold;
    public int stageLevel;
    public int attackPower;

    public UserData(string name, int gold, int stage, int atk)
    {
        this.userName = name;
        this.gold = gold;
        this.stageLevel = stage;
        this.attackPower = atk;
    }
}