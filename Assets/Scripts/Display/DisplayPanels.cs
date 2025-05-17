using System.Net;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DisplayPanels : MonoBehaviour
{
    [Header("Game Display")]
    public GameObject GameDisplay;
    public GameObject GameView;

    //TileDisplay
    [Header("Tile Display")]
    public GameObject tileDisplay;
    public TMPro.TMP_Text TileDisplayText;
    public WorldData WorldData;
    private Ticker ticker;
    private Game_Manager Game_Manager;

    // Buttons
    [Header("Buttons")]
    public Button ExitGame;
    public Button HostilePeaceButton;

    [Header("Chat Log Buttons")]
    public Button GeneralLog;
    public Button CombatLog;

    // Chat Log Display
    [Header("Chat Log")]
    public ChatLog ChatLog;
    public ScrollRect chatLogRect;
    public TMPro.TMP_Text chatLogText;

    [Header("Loading Screen")]
    public GameObject LoadingScreen;
    public TMP_Text loadingtext;

    void OnEnable()
    {
        ChatLog.OnLogAdded += UpdateChatLog;
    }

    void OnDisable()
    {
        ChatLog.OnLogAdded -= UpdateChatLog;
    }
    private void UpdateChatLog()
    {
        // Call the ChatLog instance's UpdateChatLog method
        ChatLog.UpdateChatLog();
    }
    void Start()
    {
        Game_Manager = Game_Manager.Instance;
        Game_Manager.displayPanels = this;
        ticker = Ticker.Instance;

        // Event listener for buttons
        ExitGame.onClick.AddListener(ExitGameClick);
        GeneralLog.onClick.AddListener(() => {
            ChatLog.activeLog = ChatLog.ActiveLog.General;
            ChatLog.UpdateChatLog();
            // Set the clicked button's normal color to red and the other to white.
            GeneralLog.image.color = Color.green;
            CombatLog.image.color = Color.white;
        });
        CombatLog.onClick.AddListener(() => {
            ChatLog.activeLog = ChatLog.ActiveLog.Combat;
            ChatLog.UpdateChatLog();
            // Set the clicked button's normal color to red and the other to white.
            GeneralLog.image.color = Color.white;
            CombatLog.image.color = Color.green;
        });
        GeneralLog.image.color = Color.green;
        HostilePeaceButton.onClick.AddListener(() =>
        {
            if(HostilePeaceButton.image.color == Color.red)
            {
                HostilePeaceButton.image.color = Color.green;
                ChatLog.Instance.AddGeneralLog("You are now peaceful.");

                //Game_Manager.player.isHostile = false;
            }
            else
            {
                ChatLog.Instance.AddGeneralLog("You are now hostile.");
                HostilePeaceButton.image.color = Color.red;
                //Game_Manager.player.isHostile = true;
            }
        });
        HostilePeaceButton.image.color = Color.green;
    }

    public void UpdateLoadingText(string text)
    {
        loadingtext.text = text;
    }
    private void Update()
    {
        // If either shift key is held and Tab is pressed this frame
        if ((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            && Input.GetKeyDown(KeyCode.Tab))
        {
            HostilePeaceButton.onClick.Invoke();
        }
        // Shift F1 Interacts with GeneralLog
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.F1))
        {
            GeneralLog.onClick.Invoke();
        }
        // Shift F2 Interacts with CombatLog
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.F2))
        {
            CombatLog.onClick.Invoke();
        }
    }

    public void UpdatePlayerTileInfo()
    {
        if (Game_Manager.player == null)
        {
            Debug.LogWarning("Player not found.");
            return;
        }

        // Check if player's entity position is valid
        if (Game_Manager.player.entityWorldPos == null)
        {
            Debug.LogWarning("Player entity position is null.");
            return;
        }

        WorldTilePos pos = Game_Manager.player.entityWorldPos;
        if (WorldData.WorldTileData.TryGetValue(pos, out WorldTile tile))
        {
            TileDisplayText.text = $"Tile: {tile.TileType.ToString()} \n";
        }
        else
        {
            Debug.LogWarning("Tile not found at position: " + pos.x + ", " + pos.y);
        }
    }

    private void ExitGameClick()
    {
        Application.Quit();
    }
}

