using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class WorldInteractionCliker : MonoBehaviour, IPointerClickHandler
{
    public TMPro.TMP_Text WorldViewText;
    public int ClickedDistance = 1;

    WorldDisplay WorldDisplay;
    WorldData worldData;
    ChatLog chatLog;
    private int clickedCharIndex = -1;      // Track which tile was clicked.

    private void Start()
    {
        WorldDisplay = Game_Manager.Instance.worldDisplay;
        worldData = Game_Manager.Instance.worldData;
        chatLog = Game_Manager.Instance.chatLog;

        if (worldData == null)
            Debug.LogError("worldData not assigned!");
        if (WorldDisplay == null)
            Debug.LogError("levelDisplay not assigned!");
        if (chatLog == null)
            Debug.LogError("chatLog not assigned!");
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // World Overview clicking for entering POIS // Currently Only Towns supported.
        if (eventData.button == PointerEventData.InputButton.Left && Input.GetKey(KeyCode.LeftControl))
        {

            WorldViewText.ForceMeshUpdate();

            int charIndex = TMP_TextUtilities.FindIntersectingCharacter(WorldViewText, eventData.position, null, true);
            if (charIndex != -1 && charIndex < WorldViewText.textInfo.characterCount)
            {
                clickedCharIndex = charIndex;

                // Get character info to calculate row and column.
                TMP_CharacterInfo charInfo = WorldViewText.textInfo.characterInfo[charIndex];
                int lineIndex = charInfo.lineNumber;
                int firstCharIndex = WorldViewText.textInfo.lineInfo[lineIndex].firstCharacterIndex;
                int column = charIndex - firstCharIndex;
                int row = lineIndex;  // Each line corresponds to one row.

                // Map the display coordinate to full-world coordinates.
                Vector2Int worldCoord = new Vector2Int(
                    WorldDisplay.currentViewPos.x + column,
                    WorldDisplay.currentViewPos.y + row);

                // Create a key for lookup.
                WorldTilePos posKey = new WorldTilePos(worldCoord.x, worldCoord.y);
                WorldTilePos PlayerPos = Game_Manager.Instance.player.entityWorldPos;
                StringBuilder infoBuilder = new StringBuilder();
                // check Player Distance from Clicked tile
                int distance = Pathfindier.GetDistanceBetweenLevelPositions(PlayerPos, posKey);
                // Check if player is within 1 tile of the clicked tile
                if (distance <= ClickedDistance)
                {
                    // Look up the tile data directly.
                    if (worldData.WorldTileData.TryGetValue(posKey, out WorldTile tile))
                    {
                        LevelGen generator = Game_Manager.Instance.levelGen;
                        if (generator == null)
                        {
                            Debug.LogError("LevelGen not assigned!");
                            return;
                        }
                        if (tile.IsTown)
                        {
                            Debug.Log($"Clicked Town at {posKey.x}, {posKey.y}.");
                            if (tile.POI == WorldTile.POIType.Town)
                            {
                                Game_Manager.Instance.PlayerLoadingIntoTown = true;
                                generator.GenerateHumanTownLevel(tile);
                            }
                        }
                    }

                }
            }
        }
    }
}
