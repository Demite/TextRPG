using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections;
using System.Text;
using Unity.VisualScripting;

public class WorldTileHoverInfo : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IPointerMoveHandler
{
    [Header("References")]
    public TMP_Text DisplayText;       // The TMP text that displays your ASCII world.
    public WorldDisplay worldDisplay;       // Reference to the WorldDisplay component.
    public LevelDisplay levelDisplay;       // Reference to the LevelDisplay component.
    public WorldData worldData;             // Reference to your WorldData container.

    [Header("Tooltip UI")]
    public GameObject tooltipPanel;         // The UI Panel that displays tile information.
    public TMP_Text tooltipText;            // The TMP text within the tooltip panel.

    [Header("Tooltip Options")]
    public bool autoHide = true;            // If true, auto-hide after delay.
    public float tooltipDelay = 1.5f;         // Delay before auto-hiding.

    private Camera uiCamera = null;         // Not used for Screen Space ¨C Overlay.
    private int clickedCharIndex = -1;      // Track which tile was clicked.
    private bool tooltipLocked = false;     // Lock the tooltip on click.
    private Coroutine tooltipHideCoroutine = null;

    void Start()
    {
        levelDisplay = Game_Manager.Instance.levelDisplay;
        if (tooltipPanel != null)
            tooltipPanel.SetActive(false);

        uiCamera = null; // For Screen Space ¨C Overlay.

        if (DisplayText == null)
            Debug.LogError("DisplayText not assigned!");
        if (worldDisplay == null)
            Debug.LogError("worldDisplay not assigned!");
        if (worldData == null)
            Debug.LogError("worldData not assigned!");
        if (levelDisplay == null)
            Debug.LogError("levelDisplay not assigned!");
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // No immediate action on pointer enter.
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!tooltipLocked)
            HideTooltip();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (Game_Manager.Instance.player.IsInWorld)
        {
            // Prevent interaction until the world generation is complete.
            if (Game_Manager.Instance == null || !Game_Manager.Instance.GameGenerated)
            {
                Debug.LogWarning("World generation not complete yet! Click ignored.");
                return;
            }

            // Only process right-clicks.
            if (eventData.button != PointerEventData.InputButton.Right)
                return;

            Debug.Log("Right click on text.");
            DisplayText.ForceMeshUpdate();

            int charIndex = TMP_TextUtilities.FindIntersectingCharacter(DisplayText, eventData.position, null, true);
            Debug.Log("Clicked char index: " + charIndex);

            if (charIndex != -1 && charIndex < DisplayText.textInfo.characterCount)
            {
                clickedCharIndex = charIndex;

                // Get character info to calculate row and column.
                TMP_CharacterInfo charInfo = DisplayText.textInfo.characterInfo[charIndex];
                int lineIndex = charInfo.lineNumber;
                int firstCharIndex = DisplayText.textInfo.lineInfo[lineIndex].firstCharacterIndex;
                int column = charIndex - firstCharIndex;
                int row = lineIndex;  // Each line corresponds to one row.

                // Map the display coordinate to full-world coordinates.
                Vector2Int worldCoord = new Vector2Int(
                    worldDisplay.currentViewPos.x + column,
                    worldDisplay.currentViewPos.y + row);

                // Create a key for lookup.
                WorldTilePos posKey = new WorldTilePos(worldCoord.x, worldCoord.y);
                StringBuilder infoBuilder = new StringBuilder();

                // Look up the tile data directly.
                if (worldData.WorldTileData.TryGetValue(posKey, out WorldTile tile))
                {
                    infoBuilder.AppendLine($"Tile: ({worldCoord.x}, {worldCoord.y})");
                    infoBuilder.AppendLine($"Ground: {tile.TileType}");
                    infoBuilder.AppendLine($"Discovered: {tile.HasBeenDiscovered}");
                    if (tile.Region != null && tile.Region.Kingdom != null)
                    {
                        // Optionally, display tile.Region.RegionID instead.
                        infoBuilder.AppendLine($"Region: {tile.BelongsToRegion}");
                        infoBuilder.AppendLine($"Kingdom: {tile.Region.Kingdom.KingdomName}");
                        infoBuilder.AppendLine($"King: {tile.Region.Kingdom.KingdomKing.entityName}\nQueen: {tile.Region.Kingdom.KingdomQueen.entityName}");
                    }
                    else if (tile.BelongsToRegion)
                    {
                        Debug.Log("Tile has no region data, but is claimed by a region.");
                    }
                    else
                    {
                        infoBuilder.AppendLine("Region: None");
                        Debug.LogWarning("Tile has no region data.");
                    }

                    if (tile.Features != null && tile.Features.Count > 0)
                    {
                        foreach (string feature in tile.Features)
                        {
                            infoBuilder.AppendLine($"Feature: {feature}");
                        }
                    }

                    if (tile.EntityOnTile != null)
                    {
                        infoBuilder.AppendLine($"NPC: {tile.EntityOnTile.entityName}");
                    }

                    tooltipText.text = infoBuilder.ToString();
                }
                else
                {
                    tooltipText.text = "No tile data.";
                }

                //// Debug logging to confirm dictionary lookup.
                //foreach (var key in worldData.WorldTileData.Keys)
                //{
                //    if (key.x == worldCoord.x && key.y == worldCoord.y)
                //    {
                //        Debug.Log($"Found key in dictionary: ({key.x},{key.y})");
                //        if (worldData.WorldTileData.TryGetValue(key, out WorldTile RegionTile))
                //        {
                //            string regionID = RegionTile.Region != null ? RegionTile.Region.RegionID.ToString() : "null";
                //            Debug.Log($"Tile type: {RegionTile.TileType}, RegionID: {regionID}, BelongsToRegion: {RegionTile.BelongsToRegion} Found After Cords!!");
                //        }
                //        break;
                //    }
                //    if (key.x == posKey.x && key.y == posKey.y)
                //    {
                //        Debug.Log($"Found key in dictionary: ({key.x},{key.y})");
                //        if (worldData.WorldTileData.TryGetValue(key, out WorldTile RegionTile))
                //        {
                //            string regionID = RegionTile.Region != null ? RegionTile.Region.RegionID.ToString() : "null";
                //            Debug.Log($"Tile type: {RegionTile.TileType}, RegionID: {regionID}, BelongsToRegion: {RegionTile.BelongsToRegion} Found After PosKEY!!");
                //        }
                //        break;
                //    }
                //}

                ShowTooltip(eventData.position);
                tooltipLocked = true;

                if (autoHide)
                {
                    if (tooltipHideCoroutine != null)
                        StopCoroutine(tooltipHideCoroutine);
                    tooltipHideCoroutine = StartCoroutine(LockTooltipForDelay(tooltipDelay));
                }
                else
                {
                    HideTooltip();
                }
            }
        }
        else if (Game_Manager.Instance.player.IsInLevel)
        {
            // Only process right-clicks.
            if (eventData.button != PointerEventData.InputButton.Right)
                return;

            DisplayText.ForceMeshUpdate();

            int charIndex = TMP_TextUtilities.FindIntersectingCharacter(DisplayText, eventData.position, null, true);
            Debug.Log("Clicked char index: " + charIndex);

            if (charIndex != -1 && charIndex < DisplayText.textInfo.characterCount)
            {
                clickedCharIndex = charIndex;

                // Get character info to calculate row and column.
                TMP_CharacterInfo charInfo = DisplayText.textInfo.characterInfo[charIndex];
                int lineIndex = charInfo.lineNumber;
                int firstCharIndex = DisplayText.textInfo.lineInfo[lineIndex].firstCharacterIndex;
                int column = charIndex - firstCharIndex;
                int row = lineIndex;  // Each line corresponds to one row.

                // Map the display coordinate to full-world coordinates.
                Vector2Int worldCoord = new Vector2Int(
                    levelDisplay.currentViewPos.x + column,
                    levelDisplay.currentViewPos.y + row);

                // Create a key for lookup.
                LevelPOS posKey = new LevelPOS(worldCoord.x, worldCoord.y);
                StringBuilder infoBuilder = new StringBuilder();
                // Look up the tile data directly.
                if (worldData.ActiveLevelData.TryGetValue(posKey, out LevelTile tile))
                {
                    //infoBuilder.AppendLine($"");
                    if (tile.IsOccupiedByEnitiy && tile.entity == (IEntity)Game_Manager.Instance.player)
                    {
                        Player p = Game_Manager.Instance.player;
                        infoBuilder.AppendLine($"{p.entityName}, {p.race.ToString()}, {p.gender.ToString()}");
                        infoBuilder.AppendLine($"{p.starterClass.ToString()}");
                    }
                    else if (tile.IsOccupiedByFoliage)
                    {
                        if (tile.foliage is TreeBase)
                        {
                            TreeBase tree = (TreeBase)tile.foliage;
                            infoBuilder.AppendLine($"Tree: {tree.Name}");
                            infoBuilder.AppendLine($"Appears to be {tree.FormatTreeAge()}");
                        }
                    }
                    else
                    {
                        infoBuilder.AppendLine($"Tile: ({worldCoord.x}, {worldCoord.y})");
                        infoBuilder.AppendLine($"Ground: {tile.ForestTileType}");
                        infoBuilder.AppendLine($"Transversable: {tile.IsTransversable}");
                    }
                    tooltipText.text = infoBuilder.ToString();
                }
                else
                {
                    return;
                }
                ShowTooltip(eventData.position);
                tooltipLocked = true;

                if (autoHide)
                {
                    if (tooltipHideCoroutine != null)
                        StopCoroutine(tooltipHideCoroutine);
                    tooltipHideCoroutine = StartCoroutine(LockTooltipForDelay(tooltipDelay));
                }
                else
                {
                    HideTooltip();
                }
            }
        }    
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        if (clickedCharIndex != -1)
        {
            int currentCharIndex = TMP_TextUtilities.FindIntersectingCharacter(DisplayText, eventData.position, null, true);
            if (currentCharIndex != clickedCharIndex)
            {
                HideTooltip();
            }
        }
    }

    private void ShowTooltip(Vector3 position)
    {
        if (tooltipPanel != null)
        {
            tooltipPanel.SetActive(true);
            tooltipPanel.transform.position = position;
        }
    }

    private IEnumerator LockTooltipForDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        tooltipLocked = false;
        HideTooltip();
    }

    private void HideTooltip()
    {
        if (tooltipPanel != null)
            tooltipPanel.SetActive(false);
        clickedCharIndex = -1;
        tooltipLocked = false;
        if (tooltipHideCoroutine != null)
        {
            StopCoroutine(tooltipHideCoroutine);
            tooltipHideCoroutine = null;
        }
    }
}
