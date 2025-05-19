using UnityEngine;
using TMPro;
using static DisplayPanels;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

public class Player : PlayerBase
{
    public Inventory Inventory;
    public GameObject playerInfoPanel;
    public TMP_Text playerInfoText;
    private WorldData worldData;
    private Game_Manager gameManager;
    private Ticker ticker;
    private WorldDisplay worldDisplay;

    [SerializeField]
    public bool IsInLevel { get; set; } = false;
    [SerializeField]
    public bool IsInWorld { get; set; } = true;

    // Cached positions.
    public int x;
    public int y;

    // Flag: ensures we only process input once per turn.
    private bool turnProcessed = false;

    void Update()
    {
        // Allow immediate biome loading regardless of turn state
        if (Input.GetKeyDown(KeyCode.G))
        {
            if (worldData.WorldTileData.TryGetValue(entityWorldPos, out WorldTile currentTile))
            {
                LevelGen generator = Game_Manager.Instance.levelGen;
                if (generator == null)
                {
                    Debug.LogError("LevelGen not assigned!");
                    return;
                }
                Game_Manager.Instance.PlayerLoadingIntoBiome = true;
                generator.GenerateLevelForTile(currentTile);
            }
            Debug.Log($"Player pressed G to load into a biome at {entityWorldPos.x},{entityWorldPos.y}.");
        }


        if (Game_Manager.Instance.CurrentTurnState == TurnState.PlayerTurn &&
            !turnProcessed && gameManager.CharacterSetup)
        {
            DoTurn();
        }
    }

    void Start()
    {
        // Cache the WorldDisplay from the Game_Manager instance.
        worldDisplay = Game_Manager.Instance.worldDisplay;
        Inventory = new Inventory(20);
        if (worldDisplay == null)
        {
            Debug.LogError("WorldDisplay not found!");
        }
        gameManager = Game_Manager.Instance;
        gameManager.player = this;
        worldData = gameManager.worldData;
        ticker = Ticker.Instance;
        gender = EntityGender.None;
        starterClass = StarterClass.None;
        race = EntityRace.None;
        IsInWorld = true;
        IsInLevel = false;
    }

    public void SetupPlayer()
    {
        EntitySymbol = "@";
        entitySymbolColor = "#FFA500"; // Orange color.
        isPlayer = true;

        // Choose the spawn position AFTER generating the world.
        this.entityWorldPos = Game_Manager.Instance.SuitablePlayerSpawnLocation();
        Debug.Log($"Player spawn location: {entityWorldPos.x},{entityWorldPos.y}");
        worldData.SetEntityOnTile(this, entityWorldPos);

        Game_Manager.Instance.spawnentity();

        x = entityWorldPos.x;
        y = entityWorldPos.y;

        // Center the display on the player.
        CenterDisplayOnPlayerInWorldView();

        // Add the player to the list of all entities.
        gameManager.EntitiesInGame.Add(this);

        // Start the game; set the player's turn and update the display.
        gameManager.SetupGameStart();
        CenterDisplayOnPlayerInWorldView();
        ResetTurn();  // Ready for the next turn.
    }

    /// <summary>
    /// Helper method that clamps a world tile position to within the generated bounds.
    /// </summary>
    private WorldTilePos ClampWorldTilePos(WorldTilePos pos)
    {
        int clampedX = Mathf.Clamp(pos.x, 0, gameManager.worldGen.worldWidth - 1);
        int clampedY = Mathf.Clamp(pos.y, 0, gameManager.worldGen.worldHeight - 1);
        return new WorldTilePos(clampedX, clampedY);
    }

    public override void DoTurn()
    {
        // Only process input if it's the player's turn and we haven't already processed input.
        if (Game_Manager.Instance.CurrentTurnState != TurnState.PlayerTurn || turnProcessed)
        {
            Debug.Log("Exiting DoTurn early because it's not the player's turn or input already processed.");
            return;
        }

        Vector2Int move = Vector2Int.zero;

        // Process movement keys.
        if (Input.GetKeyDown(KeyCode.W))
        {
            move.y = -1;
            if (IsInLevel)
                Debug.Log("W key pressed");
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            move.y = 1;
            if (IsInLevel)
                Debug.Log("S key pressed");
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            move.x = -1;
            if (IsInLevel)
                Debug.Log("A key pressed");
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            move.x = 1;
            if (IsInLevel)
                Debug.Log("D key pressed");
        }


        // Teleport to Region 0 center tile.
        if (Input.GetKeyDown(KeyCode.I))
        {
            Debug.Log("I key pressed");
            WorldTilePos centerTile = Game_Manager.Instance.WorldRegions[0].RegionStartTile;
            if (centerTile == null)
            {
                Debug.LogWarning("Center tile is null.");
                return;
            }

            WorldTilePos oldPos = entityWorldPos;
            entityWorldPos = centerTile;
            x = entityWorldPos.x;
            y = entityWorldPos.y;

            worldData.RemoveEntityOnTile(this, oldPos);
            worldData.SetEntityOnTile(this, centerTile);

            if (worldDisplay != null)
            {
                worldDisplay.RemoveTileOverride(oldPos);
                CenterDisplayOnPlayerInWorldView();
            }
            ChatLog.Instance.AddGeneralLog($"{this.entityName} teleported to Region 0 center tile.");

            EndPlayerTurn();
            return;
        }
        if(Input.GetKeyDown(KeyCode.F1))
        {
            if(IsInLevel)
            {
                // We will use this to leave the level and return to the World view.
                IsInLevel = false;
                IsInWorld = true;
                CenterDisplayOnPlayerInWorldView();
                Game_Manager.Instance.worldData.InactiveLevelData = Game_Manager.Instance.worldData.ActiveLevelData;
                Game_Manager.Instance.worldData.ActiveLevelData.Clear(); // Clear Level data
                Game_Manager.Instance.displayPanels.UpdateViewMode("World");
            }
        }
        // Teleport to a random town.
        if (Input.GetKeyDown(KeyCode.T))
        {
            WorldRegions region = gameManager.WorldRegions[0];
            List<TownBase> MyTowns = new List<TownBase>();
            if (region != null)
            {
                foreach (var town in gameManager.TownsInGame)
                {
                    if (town.myKingdom.KingdomRegion == region)
                    {
                        MyTowns.Add(town);
                    }
                }
                if (MyTowns.Count > 0)
                {
                    TownBase mychoosentown = MyTowns[Random.Range(0, MyTowns.Count)];
                    WorldTilePos newPos = new WorldTilePos(mychoosentown.TownLocation.x - 1, mychoosentown.TownLocation.y - 1);
                    // Update world tile position.
                    worldData.RemoveEntityOnTile(this, entityWorldPos);
                    worldData.SetEntityOnTile(this, newPos);
                    entityWorldPos = newPos;
                    x = newPos.x;
                    y = newPos.y;
                    if (worldDisplay != null)
                    {
                        worldDisplay.RemoveTileOverride(newPos);
                        CenterDisplayOnPlayerInWorldView();
                    }
                    ChatLog.Instance.AddGeneralLog($"{this.entityName} teleported to {mychoosentown.TownName}.");
                    EndPlayerTurn();
                    return;
                }
            }
        }

        // Handle movement on the world map.
        if (move != Vector2Int.zero && IsInWorld)
        {
            // Calculate new position and clamp it to world bounds.
            WorldTilePos newPos = new WorldTilePos(entityWorldPos.x + move.x, entityWorldPos.y + move.y);
            newPos = ClampWorldTilePos(newPos);

            if (worldData.WorldTileData.TryGetValue(newPos, out WorldTile tile))
            {
                if (!tile.IsTileTransversable)
                {
                    Debug.Log($"Tile at ({newPos.x},{newPos.y}) is not traversable.");
                    return;
                }
            }
            else
            {
                Debug.LogWarning($"Tile at ({newPos.x},{newPos.y}) not found.");
                return;
            }

            turnProcessed = true;

            WorldTilePos oldPos = entityWorldPos;
            entityWorldPos = newPos;
            x = newPos.x;
            y = newPos.y;

            worldData.RemoveEntityOnTile(this, oldPos);
            worldData.SetEntityOnTile(this, newPos);

            if (worldDisplay != null)
            {
                worldDisplay.RemoveTileOverride(oldPos);
                CenterDisplayOnPlayerInWorldView();
            }
            EndPlayerTurn();
            return;
        }

        // Handle movement in a level.
        if (move != Vector2Int.zero && IsInLevel)
        {
            LevelPOS newPos = new LevelPOS(entityLevelPos.x + move.x, entityLevelPos.y + move.y);
            if (worldData.ActiveLevelData.TryGetValue(newPos, out LevelTile tile))
            {
                if (!tile.IsTransversable)
                {
                    if (tile.IsOccupiedByFoliage && tile.foliage != null)
                    {
                        return;
                    }
                    else
                        return;
                }
            }
            else
            {
                Debug.LogWarning($"Tile at ({newPos.x},{newPos.y}) not found.");
                return;
            }

            turnProcessed = true;

            LevelPOS oldPos = entityLevelPos;
            entityLevelPos = newPos;
            // Optionally update x,y if level and world positions are correlated.
            worldData.RemoveEntityOnLevelTile(this, oldPos);
            worldData.SetEntityOnLevelTile(this, newPos);

            if (gameManager.levelDisplay != null)
            {
                gameManager.levelDisplay.RemoveTileOverride(oldPos);
                CenterDisplayOnPlayerInLevelView();
            }
            EndPlayerTurn();
            return;
        }
    }

    /// <summary>
    /// Ends the player's turn by processing NPC turns.
    /// </summary>
    private void EndPlayerTurn()
    {
        Game_Manager.Instance.CurrentTurnState = TurnState.NPCTurn;
        Game_Manager.Instance.turnManager.ProcessNPCTurns();
    }

    // Resets the flag so the player can act again on their next turn.
    public void ResetTurn()
    {
        turnProcessed = false;
    }

    public string PlayerInformationText() =>
        $"{entityName} the {gender} {race} {starterClass}";

    /// <summary>
    /// Centers the WorldDisplay on the player's current world position.
    /// </summary>
    private void CenterDisplayOnPlayerInWorldView()
    {
        if (worldDisplay != null && entityWorldPos != null)
        {
            int idealViewX = entityWorldPos.x - worldDisplay.Columns / 2;
            int idealViewY = entityWorldPos.y - worldDisplay.Rows / 2;

            int viewX = Mathf.Clamp(idealViewX, 0, worldDisplay.WorldDataWidth - worldDisplay.Columns);
            int viewY = Mathf.Clamp(idealViewY, 0, worldDisplay.WorldDataHeight - worldDisplay.Rows);

            worldDisplay.currentViewPos = new Vector2Int(viewX, viewY);
            Debug.Log($"Centering display on player at {entityWorldPos.x},{entityWorldPos.y}");
            worldDisplay.BuildWorldDisplay();
        }
    }

    /// <summary>
    /// Centers the LevelDisplay on the player's current level position.
    /// </summary>
    public void CenterDisplayOnPlayerInLevelView()
    {
        LevelDisplay levelDisplay = Game_Manager.Instance.levelDisplay;
        if (levelDisplay == null || entityLevelPos == null)
        {
            Debug.LogWarning("LevelDisplay or entityLevelPos is null.");
            return;
        }

        // Calculate ideal view using the player's level coordinates.
        int idealViewX = entityLevelPos.x - levelDisplay.Columns / 2;
        int idealViewY = entityLevelPos.y - levelDisplay.Rows / 2;

        // Now clamp using level bounds.
        int clampedX = (levelDisplay.WorldDataWidth >= levelDisplay.Columns) ?
                          Mathf.Clamp(idealViewX, 0, levelDisplay.WorldDataWidth - levelDisplay.Columns) : 0;
        int clampedY = (levelDisplay.WorldDataHeight >= levelDisplay.Rows) ?
                          Mathf.Clamp(idealViewY, 0, levelDisplay.WorldDataHeight - levelDisplay.Rows) : 0;

        levelDisplay.currentViewPos = new Vector2Int(clampedX, clampedY);
        Debug.Log($"Centering level display on player at {entityLevelPos.x},{entityLevelPos.y}. Calculated view pos: {clampedX},{clampedY}");
        Debug.Log($"LevelDisplay: Rows={levelDisplay.Rows}, Columns={levelDisplay.Columns}, WorldDataWidth={levelDisplay.WorldDataWidth}, WorldDataHeight={levelDisplay.WorldDataHeight}");
        levelDisplay.BuildWorldDisplay();
    }
}


