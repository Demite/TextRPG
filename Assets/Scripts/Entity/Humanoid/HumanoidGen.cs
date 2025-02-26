using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class HumanoidGen : EntityNPCBase
{
    private System.Random random = new System.Random();

    private List<string> HumanMaleNames = new List<string>();
    private List<string> HumanFemaleNames = new List<string>();
    private List<string> ElfMaleNames = new List<string>();
    private List<string> ElfFemaleNames = new List<string>();

    public HumanoidGen()
    {
        initialize();
        race = MyEntityRace();
        gender = MyEntityGender();
        entityName = GetMyName(race, gender);
        entitySymbolColor = "white";
        EntitySymbol = MyEntitySymbol();
    }
    public HumanoidGen(EntityGender g)
    {
        initialize();
        race = MyEntityRace();
        gender = g;
        entityName = GetMyName(race, gender);
        entitySymbolColor = "white";
        EntitySymbol = MyEntitySymbol();
    }

        private void initialize()
    {
        SetupHumanNames();
        SetupElfNames();
    }

    private string GetMyName(EntityRace r, EntityGender g)
    {
        switch (r)
        {
            case EntityRace.Human:
                if (g == EntityGender.Male)
                {
                    return HumanMaleNames[random.Next(0, HumanMaleNames.Count)];
                }
                else
                {
                    return HumanFemaleNames[random.Next(0, HumanFemaleNames.Count)];
                }
            case EntityRace.Elf:
                if (g == EntityGender.Male)
                {
                    return ElfMaleNames[random.Next(0, ElfMaleNames.Count)];
                }
                else
                {
                    return ElfFemaleNames[random.Next(0, ElfFemaleNames.Count)];
                }
            default:
                return "Unknown";
        }
    }

    private void SetupHumanNames()
    {
        HumanMaleNames.Clear();
        HumanFemaleNames.Clear();
        HumanMaleNames.Add("John");
        HumanMaleNames.Add("James");
        HumanMaleNames.Add("Robert");
        HumanMaleNames.Add("Michael");
        HumanMaleNames.Add("William");
        HumanMaleNames.Add("David");
        HumanMaleNames.Add("Richard");
        HumanMaleNames.Add("Joseph");
        HumanMaleNames.Add("Thomas");
        HumanMaleNames.Add("Charles");
        HumanMaleNames.Add("Daniel");
        HumanMaleNames.Add("Matthew");
        HumanMaleNames.Add("Anthony");
        HumanMaleNames.Add("Mark");
        HumanMaleNames.Add("Paul");
        HumanFemaleNames.Add("Mary");
        HumanFemaleNames.Add("Patricia");
        HumanFemaleNames.Add("Jennifer");
        HumanFemaleNames.Add("Linda");
        HumanFemaleNames.Add("Elizabeth");
        HumanFemaleNames.Add("Barbara");
        HumanFemaleNames.Add("Susan");
        HumanFemaleNames.Add("Jessica");
        HumanFemaleNames.Add("Sarah");
        HumanFemaleNames.Add("Karen");
        HumanFemaleNames.Add("Nancy");
        HumanFemaleNames.Add("Lisa");
        HumanFemaleNames.Add("Betty");
        HumanFemaleNames.Add("Dorothy");
        HumanFemaleNames.Add("Sandra");
    }
    private void SetupElfNames()
    {
        ElfMaleNames.Clear();
        ElfFemaleNames.Clear();
        ElfMaleNames.Add("Legolas");
        ElfMaleNames.Add("Thranduil");
        ElfMaleNames.Add("Elrond");
        ElfMaleNames.Add("Celeborn");
        ElfMaleNames.Add("Haldir");
        ElfMaleNames.Add("Glorfindel");
        ElfMaleNames.Add("Fingon");
        ElfMaleNames.Add("Finrod");
        ElfMaleNames.Add("Fingolfin");
        ElfMaleNames.Add("Turgon");
        ElfMaleNames.Add("Ecthelion");
        ElfMaleNames.Add("Galdor");
        ElfMaleNames.Add("Galion");
        ElfMaleNames.Add("Gildor");
        ElfMaleNames.Add("Glorfindel");
        ElfFemaleNames.Add("Galadriel");
        ElfFemaleNames.Add("Arwen");
        ElfFemaleNames.Add("Luthien");
        ElfFemaleNames.Add("Idril");
        ElfFemaleNames.Add("Aredhel");
        ElfFemaleNames.Add("Finduilas");
        ElfFemaleNames.Add("Nellas");
        ElfFemaleNames.Add("Nimloth");
        ElfFemaleNames.Add("Earwen");
        ElfFemaleNames.Add("Aerin");
        ElfFemaleNames.Add("Amarie");
        ElfFemaleNames.Add("Andreth");
        ElfFemaleNames.Add("Anaire");
        ElfFemaleNames.Add("Anardil");
        ElfFemaleNames.Add("Anariel");
    }

    private EntityRace MyEntityRace()
    {
        var result = random.Next(0, 3);
        switch(result)
        {
            case 0:
                return EntityRace.Human;
            case 1:
                return EntityRace.Elf;
            case 2:
                return EntityRace.Dwarf;
            case 3:
                return EntityRace.Gnome;
            default:
                return EntityRace.None;
        }
    }
    private EntityGender MyEntityGender()
    {
        var result = random.Next(0, 2);
        switch (result)
        {
            case 0:
                return EntityGender.Male;
            case 1:
                return EntityGender.Female;
                default:
                return EntityGender.None;
        }
    }
    private string MyEntitySymbol()
    {
        switch (race)
        {
            case EntityRace.Human:
                return "H";
            case EntityRace.Elf:
                return "E";
            case EntityRace.Dwarf:
                return "D";
            case EntityRace.Gnome:
                return "G";
            default:
                return "?";
        }
    }


    public override void DoTurn()
    {
        
    }
}
