using UnityEngine;

public class TreeBase : IFoliage
{
    public enum TreeLifeStage
    {
        Seed,
        Sapling,
        Mature,
        Old,
        Dead
    }
    public enum TreeType
    {
        Oak,
        Pine,
        Birch,
        Maple,
        Willow,
        Cherry,
        Apple,
        Pear,
        Peach,
        Plum,
        Orange,
        Lemon,
        Lime,
        Grapefruit,
        Banana,
        Mango,
        Papaya,
        Guava,
        Kiwi,
        Avocado,
        Coconut,
        Palm,
        Cedar,
        Redwood,
        Sequoia,
        Cypress,
        Fir,
        Spruce,
        Hemlock,
        Yew,
        Juniper,
        Mahogany,
        Ebony,
        Teak,
        Rosewood,
        Walnut,
        Chestnut,
        Hickory
    }

    public string Name { get; set; }
    public FoliageType Type { get; set; }
    public string Symbol { get; set; } = "0";
    public string SymbolColor { get; set; }
    public TreeType Tree { get; set; }
    public TreeLifeStage LifeStage { get; set; }
    public float Age { get; set; }
    public bool FruitBearing { get; set; }
    public ItemBase Resource { get; set; }

    public TreeBase(TreeType tree, string symbolColor)
    {
        Type = FoliageType.Tree;
        Tree = tree;
        LifeStage = SetLifeStage();
        SetLifeStageInfomation();
        SymbolColor = symbolColor;
    }
    private TreeLifeStage SetLifeStage()
    {
        int ran = UnityEngine.Random.Range(0, 5);
        switch(ran)
        {
            case 0:
                return TreeLifeStage.Seed;
            case 1:
                return TreeLifeStage.Sapling;
            case 2:
                return TreeLifeStage.Mature;
            case 3:
                return TreeLifeStage.Old;
            case 4:
                return TreeLifeStage.Dead;
                default:
                return TreeLifeStage.Seed;
        }
    }

    private void SetLifeStageInfomation()
    {
        switch (LifeStage) {
            case TreeLifeStage.Seed:
                Age = UnityEngine.Random.Range(0.1f, 1.5f);
                Name = GetTreeName() + "Seedling";
                break;
            case TreeLifeStage.Sapling:
                Age = UnityEngine.Random.Range(1.6f, 3.5f);
                Name = GetTreeName() + "Sapling";
                break;
            case TreeLifeStage.Mature:
                Age = UnityEngine.Random.Range(3.6f, 20.1f);
                Name = GetTreeName();
                break;
            case TreeLifeStage.Old:
                Age = UnityEngine.Random.Range(20.2f, 100.0f);
                Name = GetTreeName();
                break;
            case TreeLifeStage.Dead:
                Name = GetTreeName() + " (Dead)";
                break;
        }
    }
    public string FormatTreeAge()
    {
        if (Age < 1.0f)
        {
            // For ages below 1 year, split the output based on the half‐year threshold.
            if (Age < 0.5f)
            {
                // Ages from 0.1 up to 0.49
                return $"{Age:0.0} years old (just sprouted)";
            }
            else
            {
                // Ages from 0.5 to 0.99
                return $"{Age:0.0} years old (more established)";
            }
        }
        else
        {
            return $"{Age:0} years old";
        }
    }

    private string GetTreeName()
    {
        switch (Tree) {
            case TreeType.Apple:
                return "Apple Tree";
            case TreeType.Lime:
                return "Lime Tree";
            case TreeType.Lemon:
                return "Lemon Tree";
            case TreeType.Orange:
                return "Orange Tree";
            case TreeType.Pear:
                return "Pear Tree";
            case TreeType.Peach:
                return "Peach Tree";
            case TreeType.Plum:
                return "Plum Tree";
            case TreeType.Pine:
                return "Pine Tree";
            case TreeType.Oak:
                return "Oak Tree";
            case TreeType.Birch:
                return "Birch Tree";
            case TreeType.Maple:
                return "Maple Tree";
            case TreeType.Willow:
                return "Willow Tree";
            case TreeType.Cherry:
                return "Cherry Tree";
            case TreeType.Grapefruit:
                return "Grapefruit Tree";
            case TreeType.Banana:
                return "Banana Tree";
            case TreeType.Mango:
                return "Mango Tree";
            case TreeType.Papaya:
                return "Papaya Tree";
            case TreeType.Guava:
                return "Guava Tree";
            case TreeType.Kiwi:
                return "Kiwi Tree";
            case TreeType.Avocado:
                return "Avocado Tree";
            case TreeType.Coconut:
                return "Coconut Tree";
            case TreeType.Palm:
                return "Palm Tree";
            case TreeType.Cedar:
                return "Cedar Tree";
            case TreeType.Redwood:
                return "Redwood Tree";
            case TreeType.Sequoia:
                return "Sequoia Tree";
            case TreeType.Cypress:
                return "Cypress Tree";
            case TreeType.Fir:
                return "Fir Tree";
            case TreeType.Spruce:
                return "Spruce Tree";
            case TreeType.Hemlock:
                return "Hemlock Tree";
            case TreeType.Yew:
                return "Yew Tree";
            case TreeType.Juniper:
                return "Juniper Tree";
            case TreeType.Mahogany:
                return "Mahogany Tree";
            case TreeType.Ebony:
                return "Ebony Tree";
            case TreeType.Teak:
                return "Teak Tree";
            case TreeType.Rosewood:
                return "Rosewood Tree";
            case TreeType.Walnut:
                return "Walnut Tree";
            default:
                return "Tree";
        }
    }
}
