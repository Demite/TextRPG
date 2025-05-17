[System.Serializable]
public class ItemAttributes
{
    // Primary stats
    public int Strength;
    public int Dexterity;
    public int Constitution;
    public int Intelligence;
    public int Wisdom;
    public int Willpower;
    public int Charisma;

    // Resource pools
    public int HitPoints;
    public int StaminaPoints;
    public int ManaPoints;

    // Extra damage
    public bool AdditionalDamageOnHit;
    public int AdditionalDamage;
}
