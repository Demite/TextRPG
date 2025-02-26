public class BaseLog
{
    protected LogType logType;
    public string message;
    public System.DateTime timestamp;

    public BaseLog()
    {
        timestamp = System.DateTime.Now;
    }
}

public class GeneralLog : BaseLog
{

    public GeneralLog()
    {
        logType = LogType.General;
    }
}
public class CombatLog : BaseLog
{
    public IEntity attacker;
    public IEntity defender;
    public int damage;

    public void CreateMessage()
    {
        message = attacker.entityName + " attacks " + defender.entityName + " for " + damage + " damage!";
    }
    public CombatLog()
    {
        logType = LogType.Combat;
    }
}
public enum LogType
{
    General,
    Combat
}