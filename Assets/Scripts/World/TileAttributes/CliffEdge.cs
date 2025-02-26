using UnityEditor.Rendering.Universal.ShaderGraph;
using UnityEngine;

public enum MineralType
{
    Clay,
    Sand,
    Silt,
    Loam
    //Peat,
    //Chalk,
    //Limestone,
    //Marl,
    //Marlstone,
    //Shale,
    //Slate,
    //Schist,
    //Gneiss,
    //Quartzite,
    //Granite,
    //Basalt,
    //Obsidian,
    //Pumice,
    //Tuff,
    //Rhyolite,
    //Andesite,
    //Diorite,
    //Gabbro,
    //Peridotite,
    //Serpentinite,
    //Soapstone,
    //Pyroxenite,
    //Dunite,
    //Anorthosite,
    //Norite,
    //Troctolite,
    //Pegmatite,
    //Gossan,
    //Laterite,
    //Bauxite,
    //Hematite,
    //Magnetite,
    //Limonite,
    //Siderite,
    //Goethite,
    //Pyrite,
    //Chalcopyrite,
    //Galena,
    //Sphalerite,
    //Cinnabar,
    //Baryte,
    //Fluorite,
    //Calcite,
    //Gypsum,
    //Halite,
    //Sylvite,
    //Borax,
    //Boronite

}

public class CliffEdge : ITileAttribute
{
    public string Name { get; set; }
    public MineralType Material { get; set; }
    public TileAttributeType Type { get; set; } = TileAttributeType.Cliff;
    public bool IsSolid { get; set; }
    public bool IsClimbable { get; set; }
    public int CliffEdgeHeight { get; set; }
    public string Symbol { get; set; } = "^";
    public string SymbolColor { get; set; }
    /// <summary>
    /// The Z index of the Z layer the CliffEdge is on.
    /// </summary>
    public LevelPOS ZindexLower { get; set; }
    /// <summary>
    /// The Z Index of the Z layer the Cliffedge is reaching up to.
    /// </summary>
    public LevelPOS ZindexUpper { get; set; }

    /// <summary>
    /// This constructor is used for cliff edges that are on a level with a Z index.
    /// </summary>
    /// <param name="material">The material the cliff edge is made off</param>
    /// <param name="lower">the location the CliffEdge is located at on the ActiveLevelData Dictionary Map</param>
    /// <param name="upper">The location the Entity will end up at if they climbed up the Cliff edge to the next Z level</param>
    public CliffEdge(MineralType material, LevelPOS lower, LevelPOS upper)
    {
        Material = material;
        GetCliffEdgeInformation();
    }
    /// <summary>
    /// This Constructor is used to Cliff edges on a level without a Z index.
    /// Cliff Edges should be non-Climbable and solid.
    /// </summary>
    /// <param name="material">The material the cliff edge is made off</param>
    public CliffEdge(MineralType material)
    {
        Material = material;
        GetCliffEdgeInformation();
        IsSolid = true;
        IsClimbable = false;
        ZindexLower = null;
        ZindexUpper = null;
    }


    private void GetCliffEdgeInformation()
    {
        switch (Material)
        {
            case MineralType.Clay:
                Name = "Clay";
                IsSolid = true;
                IsClimbable = false;
                // A reddish-brown shade for clay
                SymbolColor = "#B66A50";
                break;
            case MineralType.Loam:
                Name = "Loam";
                IsSolid = true;
                IsClimbable = false;
                // A dark, earthy brown for loam
                SymbolColor = "#8B4513";
                break;
            case MineralType.Silt:
                Name = "Silt";
                IsSolid = true;
                IsClimbable = false;
                // A light gray to represent fine silt
                SymbolColor = "#D3D3D3";
                break;
            case MineralType.Sand:
                Name = "Sand";
                IsSolid = false;
                IsClimbable = false;
                // A soft, sandy beige for sand
                SymbolColor = "#EDC9AF";
                break;
        }
    }
}
