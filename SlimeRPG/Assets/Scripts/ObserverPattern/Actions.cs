/// <summary>
/// Slime Actions
/// (upgrades first)
/// </summary>
public enum Actions
{
    Damage,
    AttackSpeed,
    Distance,
    Coins,
    Health,
    Jumping,
    Hitted,
    Attacking,
    Slyak,
    DamageText,
    GameOver
}

public struct UpgradeValues
{
    public int level;
    public float currValue;
    public float addValue;
    public int price;

    public UpgradeValues(int level, float currValue, float addValue, int price)
    {
        this.level = level;
        this.currValue = currValue;
        this.addValue = addValue;
        this.price = price;
    }

}


public struct Data
{
    public Actions action;
    public float value;
    public UpgradeValues values;

    public Data(Actions action, float value)
    {
        this.action = action;
        this.value = value;
        values = new UpgradeValues();
    }

    public Data(Actions action, UpgradeValues values)
    {
        this.action = action;
        value = 0;
        this.values = values;
    }

}
