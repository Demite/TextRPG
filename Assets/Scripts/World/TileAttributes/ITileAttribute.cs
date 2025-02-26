using UnityEditor.Rendering.Universal.ShaderGraph;
using UnityEngine;

public enum TileAttributeType
{
    Cliff,
    Water
}
public interface ITileAttribute
{
    public string Name { get; set; }
    public MineralType Material { get; set; }
    public string Symbol { get; set; }
    public string SymbolColor { get; set; }
    TileAttributeType Type { get; set; }
}
