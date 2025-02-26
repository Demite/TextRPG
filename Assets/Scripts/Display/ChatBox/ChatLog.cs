using System.Collections.Generic;
using UnityEngine;

public class ChatLog : MonoBehaviour
{
    public static ChatLog Instance;

    public enum ActiveLog
    {
        General,
        Combat
    }
    private Game_Manager Game_Manager;
    private DisplayPanels DisplayPanels;

    public List<GeneralLog> GeneralLogList;
    public List<CombatLog> CombatLogList;
    public ActiveLog activeLog;
    public int maxLogCount = 100;

    public delegate void UpdateLog();
    public static event UpdateLog OnLogAdded;

    void Awake()
    {
        // Set up the singleton instance.
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        GeneralLogList = new List<GeneralLog>();
        CombatLogList = new List<CombatLog>();
    }

    void Start()
    {
        Game_Manager = Game_Manager.Instance;
        DisplayPanels = Game_Manager.displayPanels;
        activeLog = ActiveLog.General;
        AddGeneralLog("Welcome to the game!");
        UpdateChatLog();
    }

    public void AddGeneralLog(string message)
    {
        GeneralLog generalLog = new GeneralLog();
        generalLog.message = message;
        GeneralLogList.Add(generalLog);
        // Invoke the event to signal that a log was added.
        OnLogAdded?.Invoke();
    }

    public void AddCombatLog(IEntity attacker, IEntity defender, int damage)
    {
        CombatLog combatLog = new CombatLog();
        combatLog.attacker = attacker;
        combatLog.defender = defender;
        combatLog.damage = damage;
        combatLog.CreateMessage();
        CombatLogList.Add(combatLog);
        UpdateChatLog();
    }

    public void UpdateChatLog()
    {
        if (activeLog == ActiveLog.General)
        {
            SortLogs(GeneralLogList);
            if (GeneralLogList.Count > 6)
            {
                DisplayPanels.chatLogRect.verticalNormalizedPosition = 0;
            }
        }
        else if (activeLog == ActiveLog.Combat)
        {
            SortLogs(CombatLogList);
            if (CombatLogList.Count > 6)
            {
                DisplayPanels.chatLogRect.verticalNormalizedPosition = 0;
            }
        }

        // Optionally, invoke the event here only if it's safe.
        // OnLogAdded?.Invoke(); // Consider removing this if it causes recursion.

        void SortLogs<T>(List<T> list) where T : BaseLog
        {
            if (list.Count == 0)
            {
                return;
            }
            list.Sort((x, y) => x.timestamp.CompareTo(y.timestamp));

            // Clear current text and update UI
            DisplayPanels.chatLogText.text = "";
            foreach (BaseLog log in list)
            {
                if (!string.IsNullOrEmpty(log.message))
                {
                    DisplayPanels.chatLogText.text += log.message + "\n";
                }
            }
        }
    }
}