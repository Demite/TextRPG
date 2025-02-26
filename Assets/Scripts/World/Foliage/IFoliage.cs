public enum FoliageType
{
    Tree,
    Bush,
    Grass,
    Flower,
    Weed,
    Vine,
    Shrub,
    Fern,
    Moss,
    Algae,
    Lichen,
    Fungus,
    Mushroom,
    Cactus
}


public interface IFoliage
{
    public string Name { get; set; }
    public FoliageType Type { get; set; }
    public string Symbol { get; set; }
    public string SymbolColor { get; set; }
    public ItemBase Resource { get; set; }

}
