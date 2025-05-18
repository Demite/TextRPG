using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TurnState { PlayerTurn, NPCTurn }

public class Game_Manager : MonoBehaviour
{
    [Header("Game Data")]
    public List<IEntity> FullSimulatedTurns;
    public List<IEntity> EntitiesInGame;
    public List<KingdomBase> KingdomInGame;
    public List<WorldRegions> WorldRegions;
    public List<TownBase> TownsInGame;
    public List<IEntity> EntitiesInLevel;
    public LevelPOS PlayerLevelSpawnLocation { get; set; }

    public Player player;
    public WorldDisplay worldDisplay;
    public LevelDisplay levelDisplay;
    public DisplayPanels displayPanels;
    public WorldData worldData;
    public WorldGen worldGen;
    public LevelGen levelGen;
    public SpellTargetting spellTargetting;
    public ChatLog chatLog;
    public bool GameGenerated = false;
    public bool WorldGenStarted = false;
    private bool GameSetup = false;
    public bool IsWorldViewActive = true;


    public bool PlayerLoadingIntoTown = false;
    public bool PlayerLoadingIntoBiome = false;

    [Header("Managers")]
    public Turn_Manager turnManager; // Ensure this is assigned in Inspector or via code

    public bool CharacterSetup = false;

    // Turn state property.
    public TurnState CurrentTurnState { get; set; } = TurnState.PlayerTurn;

    // Singleton instance.
    public static Game_Manager Instance { get; private set; }

    private void Awake()
    {
        WorldGenStarted = false;
        GameGenerated = false;
        KingdomInGame = new List<KingdomBase>();
        WorldRegions = new List<WorldRegions>();
        TownsInGame = new List<TownBase>();
        EntitiesInLevel = new List<IEntity>();
        chatLog = ChatLog.Instance;

        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Automatically find the Turn_Manager if it's not assigned.
        if (turnManager == null)
        {
            turnManager = FindObjectOfType<Turn_Manager>();
            if (turnManager == null)
            {
                Debug.LogError("Turn_Manager not found in the scene!");
            }
        }

        FullSimulatedTurns = new List<IEntity>();
        EntitiesInGame = new List<IEntity>();
        worldGen = GetComponent<WorldGen>();
    }

    private void Update()
    {
        if (WorldGenStarted && GameGenerated && !GameSetup)
        {
            Debug.Log("Game Setup.");
            GameSetup = true;
            StartCoroutine(ScreenTimerCoroutine());
        }
    }

    public void SetupGameStart()
    {
        // Start with player's turn.
        CurrentTurnState = TurnState.PlayerTurn;
    }

    public void GenerateWorld()
    {
        if (!GameGenerated)
        {
            StartCoroutine(worldGen.GenerateWorldCoroutine());
            Debug.Log("World Generated.");
            WorldGenStarted = true;
        }
        else
        {
            Debug.LogError("World already generated, this shouldn't be happening.");
        }
    }

    private void StartGame()
    {
        displayPanels.LoadingScreen.SetActive(false);

        // Activate all necessary game panels.
        displayPanels.GameDisplay.SetActive(true);
        displayPanels.GameView.SetActive(true);
        worldDisplay.BuildWorldDisplay();
    }
    private IEnumerator ScreenTimerCoroutine()
    {
        if (GameGenerated)
        {
            yield return new WaitForSeconds(4f);
            player.SetupPlayer();
            StartGame();
        }
    }

    public void spawnentity()
    {
        worldGen.SpawnEntities();
    }

    // Player setup methods.
    public WorldTilePos SuitablePlayerSpawnLocation()
    {
        List<WorldTilePos> tilesList = new List<WorldTilePos>();
        if (worldData == null)
        {
            Debug.LogError("WorldData reference is missing.");
            return new WorldTilePos(0, 0);
        }
        foreach (var tile in worldData.WorldTileData)
        {
            if (tile.Value.IsTileTransversable)
            {
                tilesList.Add(new WorldTilePos(tile.Key.x, tile.Key.y));
            }
        }
        if (tilesList.Count == 0)
        {
            Debug.LogError("No suitable tiles found for player spawn.");
            return new WorldTilePos(0, 0);
        }
        int randomTile = Random.Range(0, tilesList.Count);
        return tilesList[randomTile];
    }

    public void CheckAllTilesForAnyRegionOwnership()
    {
        foreach (var kvp in worldData.WorldTileData)
        {
            if (kvp.Value.BelongsToRegion)
            {
                Debug.Log($"Tile at ({kvp.Key.x}, {kvp.Key.y}) [{System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(kvp.Value)}] belongs to Region {kvp.Value.Region.RegionID}.");
            }
        }
    }

    public void LoadLevel()
    {
        Debug.Log("Getting Player Spawn Location...");
        if (PlayerLoadingIntoBiome)
        {
            if (worldData.FindSuitableTileForPlayerToSpawn())
            {
                player.IsInWorld = false;
                player.entityLevelPos = PlayerLevelSpawnLocation;
                Debug.Log($"Player spawned at {PlayerLevelSpawnLocation.x}, {PlayerLevelSpawnLocation.y}.");
                player.IsInLevel = true;
                worldData.SetEntityOnLevelTile(player, PlayerLevelSpawnLocation);

                // **Ensure level size is set before building/centering display!**

                levelDisplay.BuildWorldDisplay();
                Debug.Log($"Player Location {player.entityLevelPos.x}, {player.entityLevelPos.y}, Spawned location {PlayerLevelSpawnLocation.x}, {PlayerLevelSpawnLocation.y}");
                player.CenterDisplayOnPlayerInLevelView();
                PlayerLoadingIntoBiome = false;
            }
            else
            {
                Debug.LogError("No suitable tile found for player to spawn.");
            }
        }
        if( PlayerLoadingIntoTown)
        {
            Debug.Log("Loading Player into Town.");
            if (worldData.FindSuitableTileForPlayerSpawnInTown())
            {
                Debug.Log("Player Spawn Location Found.");
                player.IsInWorld = false;
                player.entityLevelPos = PlayerLevelSpawnLocation;
                Debug.Log($"Player spawned at {PlayerLevelSpawnLocation.x}, {PlayerLevelSpawnLocation.y}.");
                player.IsInLevel = true;
                worldData.SetEntityOnLevelTile(player, PlayerLevelSpawnLocation);

                // **Ensure level size is set before building/centering display!**

                levelDisplay.BuildWorldDisplay();
                Debug.Log($"Player Location {player.entityLevelPos.x}, {player.entityLevelPos.y}, Spawned location {PlayerLevelSpawnLocation.x}, {PlayerLevelSpawnLocation.y}");
                player.CenterDisplayOnPlayerInLevelView();
                PlayerLoadingIntoTown = false;
            }
            else
            {
                Debug.LogError("No suitable tile found for player to spawn.");
            }


        }
    }

}
