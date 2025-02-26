public static class TextAtlas
{
    // Forest Floor Tiles Colors (set to desired hex codes)
    public static readonly string ForestFloorLush = "#32CD32";         // LimeGreen for vibrant, lush patches
    public static readonly string ForestFloorDirt = "#A0522D";         // Sienna for earthy dirt
    public static readonly string ForestFloorGrass = "#228B22";        // ForestGreen for standard grass
    public static readonly string ForestFloorMud = "#5C4033";          // A dark, muddy brown
    public static readonly string ForestFloorLeaves = "#6B8E23";       // OliveDrab for leafy areas
    public static readonly string ForestFloorRockyGround = "#708090";  // SlateGray for rocky ground

    // World Tiles Colors
    public static readonly string water = "#1E90FF";       // DodgerBlue
    public static readonly string Town = "#FFD700";        // Gold
    public static readonly string Mine = "#808080";        // Gray
    public static readonly string AbandonMine = "#696969"; // DimGray
    public static readonly string Road = "#2F4F4F";        // DarkSlateGray
    public static readonly string Farm = "#8B4513";        // SaddleBrown

    public static readonly string mountain = "#A9A9A9";    // Gray
    public static readonly string forest = "#228B22";      // Green
    public static readonly string desert = "#FFD700";      // Yellow
    public static readonly string snow = "#FFFFFF";        // White

    // Forest Floor Tiles Characters
    public static readonly string ForestFloorLushChar = ".";
    public static readonly string ForestFloorDirtChar = ".";
    public static readonly string ForestFloorGrassChar = ".";
    public static readonly string ForestFloorMudChar = ".";
    public static readonly string ForestFloorLeavesChar = ".";
    public static readonly string ForestFloorRockyGroundChar = ".";

    // World Tile Characters
    public static readonly char waterChar = '~';
    public static readonly char mountainChar = '^';
    public static readonly char forestChar = '*';
    public static readonly char desertChar = '-';
    public static readonly char snowChar = '+';
    public static readonly char townChar = '¶';
    public static readonly char mineChar = '⌂';
    public static readonly char abandonedMineChar = '⌂';
    public static readonly char roadChar = '=';
    public static readonly char farmChar = '◊';
}
