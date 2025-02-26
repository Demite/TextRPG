using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Text;

public class SpellTargetting : MonoBehaviour
{
    [Header("View Window")]
    // The UI object that contains the ASCII text.
    public GameObject ViewWindow; // Must have tag "ViewWindow"

    // We'll locate the ASCII text from the ViewWindow.
    private TMP_Text asciiText;

    [Header("Tile Info Panel")]
    // Panel that will display info about the clicked tile.
    public GameObject tileInfoPanel;
    // The text component inside the tile info panel.
    public TMP_Text tileInfoText;

    [Header("Targetting Settings")]
    // For multi-cell targeting if needed.
    public int targettingWidth = 1;
    public int targettingHeight = 1;

    // Reference to the camera (for Screen Space ¨C Camera or World Space canvases).
    public Camera uiCamera;

    // Cached references to world systems.
    private WorldDisplay worldDisplay;
    private WorldData worldData;

    void Start()
    {
        // Attempt to locate the ASCII text from the ViewWindow.
        if (ViewWindow != null)
        {
            asciiText = ViewWindow.GetComponentInChildren<TMP_Text>();
            if (asciiText == null)
            {
                Debug.LogError("No TMP_Text component found under ViewWindow.");
            }
        }
        else
        {
            Debug.LogError("ViewWindow is not assigned!");
        }

        // Cache world systems.
        worldDisplay = Game_Manager.Instance.worldDisplay;
        worldData = Game_Manager.Instance.worldData;

        if (uiCamera == null)
            uiCamera = Camera.main;

        if (tileInfoPanel != null)
            tileInfoPanel.SetActive(false);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Check if the click hit an object tagged "ViewWindow".
            if (!IsClickOnViewWindow())
            {
                Debug.Log("Click not on ViewWindow; ignoring.");
                return;
            }

            //ProcessClick(Input.mousePosition);
        }
    }

    /// <summary>
    /// Returns true if the click at the current mouse position hits an object with tag "ViewWindow".
    /// </summary>
    private bool IsClickOnViewWindow()
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        foreach (RaycastResult result in results)
        {
            if (result.gameObject.CompareTag("ViewWindow"))
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Processes a click at the given screen position by determining the clicked tile and positioning the tile info panel.
    /// </summary>
    /// <param name="clickPosition">The screen space position of the click.</param>
    private void ProcessClick(Vector2 clickPosition)
    {
        // Convert the click to a local point in the asciiText's RectTransform.
        RectTransform asciiRect = asciiText.rectTransform;
        Vector2 localPoint;
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(asciiRect, clickPosition, uiCamera, out localPoint))
        {
            Debug.LogWarning("Could not convert click to local point.");
            return;
        }

        asciiText.ForceMeshUpdate();
        TMP_TextInfo textInfo = asciiText.textInfo;

        // Find the character closest to the click.
        int clickedCharIndex = -1;
        float minDistance = float.MaxValue;
        for (int i = 0; i < textInfo.characterCount; i++)
        {
            TMP_CharacterInfo charInfo = textInfo.characterInfo[i];
            Vector3 charCenter = (charInfo.bottomLeft + charInfo.topRight) / 2f;
            float distance = Vector2.Distance(localPoint, charCenter);
            if (distance < minDistance)
            {
                minDistance = distance;
                clickedCharIndex = i;
            }
        }

        if (clickedCharIndex == -1)
        {
            Debug.LogWarning("No character found at click position.");
            return;
        }

        TMP_CharacterInfo clickedCharInfo = textInfo.characterInfo[clickedCharIndex];

        // Determine which tile was clicked.
        // Assuming the ASCII display is a fixed grid with known columns.
        int col = clickedCharIndex % worldDisplay.Rows;
        int row = clickedCharIndex / worldDisplay.Columns;
        int tileX = worldDisplay.currentViewPos.x + col;
        int tileY = worldDisplay.currentViewPos.y + row;
        WorldTilePos posKey = new WorldTilePos(tileX, tileY);
        Debug.Log($"Clicked tile at: {tileX}, {tileY}");

        // Check if there is an entity on that tile.
        if (worldData.WorldTileData.TryGetValue(posKey, out WorldTile tile))
        {
            if (tile.EntityOnTile != null)
            {
                Debug.Log($"Tile has entity: {tile.EntityOnTile.entityName}");
            }
            else
            {
                Debug.Log("Tile is empty.");
            }

            // Position the tile info panel over the clicked tile.
            Vector3 bottomLeft = clickedCharInfo.bottomLeft;
            Vector3 topRight = clickedCharInfo.topRight;
            Vector3 charCenterLocal = (bottomLeft + topRight) / 2f;
            Vector3 worldPos = asciiRect.TransformPoint(charCenterLocal);
            Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(uiCamera, worldPos);

            RectTransform panelRect = tileInfoPanel.GetComponent<RectTransform>();
            panelRect.position = screenPos;
            tileInfoPanel.SetActive(true);

            // Update the panel's text with tile details.
            if (tileInfoText != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine($"Tile: ({tileX}, {tileY})");
                sb.AppendLine($"Ground: {tile.TileType}");
                if (tile.EntityOnTile != null)
                    sb.AppendLine($"Entity: {tile.EntityOnTile.entityName}");
                tileInfoText.text = sb.ToString();
            }
        }
        else
        {
            Debug.Log("Clicked tile not found in world data.");
        }
    }
}
