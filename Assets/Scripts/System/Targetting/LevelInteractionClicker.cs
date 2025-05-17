using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class LevelInteractionClicker : MonoBehaviour, IPointerClickHandler
{
    public TMPro.TMP_Text LevelViewText;
    public int ClickedDistance = 1;

    LevelDisplay levelDisplay;
    WorldData worldData;
    ChatLog chatLog;
    private int clickedCharIndex = -1;      // Track which tile was clicked.

    private void Start()
    {
        levelDisplay = Game_Manager.Instance.levelDisplay;
        worldData = Game_Manager.Instance.worldData;
        chatLog = Game_Manager.Instance.chatLog;

        if (worldData == null)
            Debug.LogError("worldData not assigned!");
        if (levelDisplay == null)
            Debug.LogError("levelDisplay not assigned!");
        if (chatLog == null)
            Debug.LogError("chatLog not assigned!");
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (Game_Manager.Instance.player.IsInLevel)
        {
            // Only process right-clicks.
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            LevelViewText.ForceMeshUpdate();

            int charIndex = TMP_TextUtilities.FindIntersectingCharacter(LevelViewText, eventData.position, null, true);
            if (charIndex != -1 && charIndex < LevelViewText.textInfo.characterCount)
            {
                clickedCharIndex = charIndex;

                // Get character info to calculate row and column.
                TMP_CharacterInfo charInfo = LevelViewText.textInfo.characterInfo[charIndex];
                int lineIndex = charInfo.lineNumber;
                int firstCharIndex = LevelViewText.textInfo.lineInfo[lineIndex].firstCharacterIndex;
                int column = charIndex - firstCharIndex;
                int row = lineIndex;  // Each line corresponds to one row.

                // Map the display coordinate to full-world coordinates.
                Vector2Int worldCoord = new Vector2Int(
                    levelDisplay.currentViewPos.x + column,
                    levelDisplay.currentViewPos.y + row);

                // Create a key for lookup.
                LevelPOS posKey = new LevelPOS(worldCoord.x, worldCoord.y);
                LevelPOS PlayerPos = Game_Manager.Instance.player.entityLevelPos;
                StringBuilder infoBuilder = new StringBuilder();
                // check Player Distance from Clicked tile
                int distance = Pathfindier.GetDistanceBetweenLevelPositions(PlayerPos, posKey);

                // Check if player is within 1 tile of the clicked tile
                if (distance <= ClickedDistance)
                {
                    Debug.Log($"Player is {distance} tiles away from the clicked tile.");

                    // Look up the tile data directly.
                    if (worldData.ActiveLevelData.TryGetValue(posKey, out LevelTile tile))
                    {
                        Debug.Log($"Clicked tile at {posKey.x}, {posKey.y}.");

                        if (tile.IsOccupiedByFoliage && tile.foliage != null)
                        {
                            Debug.Log($"Foliage at {posKey.x}, {posKey.y}.");
                            TreeBase t = tile.foliage as TreeBase;
                            if (t != null)
                            {
                                Debug.Log($"You hit a {t.Name} tree.");
                                chatLog.AddGeneralLog($"You hit a {t.Name} tree.");
                            }
                        }
                    }
                    else
                    {
                        Debug.LogWarning($"No tile data found at {posKey.x}, {posKey.y}.");
                        return;
                    }
                }
                else
                {
                    Debug.Log($"Player is {distance} tiles away from the clicked tile.");
                    return;
                }
            }
        }
    }
}
        
    

