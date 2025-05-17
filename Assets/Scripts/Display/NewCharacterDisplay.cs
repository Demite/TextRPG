using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;

public class NewCharacterDisplay : MonoBehaviour
{
    //public GameObject GamePanel;

    [Header("Character Name Field")]
    public TMP_InputField characterName;

    [Header("Drop down menus")]
    public TMP_Dropdown raceDropdown;
    public TMP_Dropdown classDropdown;
    public TMP_Dropdown genderDropdown;

    [Header("Stat Point fields")]
    public GameObject statsPanel;
    public TMP_Text statPointsAvailable;
    public TMP_Text strenth;
    public TMP_Text dexterity;
    public TMP_Text constitution;
    public TMP_Text intelligence;
    public TMP_Text wisdom;
    public TMP_Text willPower;

    [Header("Character Info fields")]
    public GameObject RaceInfoPanel;
    public GameObject ClassInfoPanel;
    public TMP_Text raceInfo;
    public TMP_Text classInfo;

    [Header("Buttons")]
    public Button FinalizePlayer;

    [Header("Stat Point Buttons")]
    public Button addStrength;
    public Button subtractStrength;
    public Button addDexterity;
    public Button subtractDexterity;
    public Button addConstitution;
    public Button subtractConstitution;
    public Button addIntelligence;
    public Button subtractIntelligence;
    public Button addWisdom;
    public Button subtractWisdom;
    public Button addWillPower;
    public Button subtractWillPower;

    bool FirstTimeClickingRaceDropwon = true;
    bool FirstTimeClicklingGenderDropdown = true;
    bool FirstTimeClickingClassDropdown = true;
    bool AllFieldsFilled = false;
    bool StatSelectionAvailable = false;
    bool StatsFinalized = false;

    int[] stats = new int[6];

    EntityGender TempGender { get; set; }
    EntityRace TempRace { get; set; }
    StarterClass TempClass { get; set; }

    int statPoints = 10;

    private void Awake()
    {
        RaceInfoPanel.SetActive(false);
        ClassInfoPanel.SetActive(false);
        statsPanel.SetActive(false);
        FinalizePlayer.gameObject.SetActive(false);
        statPointsAvailable.text = statPoints.ToString();
    }
    private void Start()
    {
        addStrength.onClick.AddListener(AddStrength);
        subtractStrength.onClick.AddListener(SubtractStrength);
        addDexterity.onClick.AddListener(AddDexterity);
        subtractDexterity.onClick.AddListener(SubtractDexterity);
        addConstitution.onClick.AddListener(AddConstitution);
        subtractConstitution.onClick.AddListener(SubtractConstitution);
        addIntelligence.onClick.AddListener(AddIntelligence);
        subtractIntelligence.onClick.AddListener(SubtractIntelligence);
        addWisdom.onClick.AddListener(AddWisdom);
        subtractWisdom.onClick.AddListener(SubtractWisdom);
        addWillPower.onClick.AddListener(AddWillPower);
        subtractWillPower.onClick.AddListener(SubtractWillPower);

        FinalizePlayer.onClick.AddListener(FinalizePlayerCreation);
    }
    private void Update()
    {
        if(OutOfStatPoints() && StatSelectionAvailable == true)
        {
            FinalizePlayer.gameObject.SetActive(true);
            StatsFinalized = true;
        }
        if(!OutOfStatPoints() && StatsFinalized == true)
        {
            FinalizePlayer.gameObject.SetActive(false);
            StatsFinalized = false;
        }
        if (CheckIfAllFieldsAreFilled())
        {
            statsPanel.SetActive(true);
            StatSelectionAvailable = true;
        }
    }
    void FinalizePlayerCreation()
    {
        if(statPoints == 0 && CheckIfAllFieldsAreFilled())
        {
            Debug.Log("Player has been created");
            //Remove this Gameobject
            Destroy(this.gameObject);
            Game_Manager.Instance.CharacterSetup = true;
            //Setup Temp Array to hold stats
            stats[0] = int.Parse(strenth.text);
            stats[1] = int.Parse(dexterity.text);
            stats[2] = int.Parse(constitution.text);
            stats[3] = int.Parse(intelligence.text);
            stats[4] = int.Parse(wisdom.text);
            stats[5] = int.Parse(willPower.text);
            Game_Manager.Instance.player.Strength = stats[0];
            Game_Manager.Instance.player.Dexterity = stats[1];
            Game_Manager.Instance.player.Constitution = stats[2];
            Game_Manager.Instance.player.Intelligence = stats[3];
            Game_Manager.Instance.player.Wisdom = stats[4];
            Game_Manager.Instance.player.Willpower = stats[5];
            Debug.Log($"Player stats are: {Game_Manager.Instance.player.Strength}, {Game_Manager.Instance.player.Dexterity}, {Game_Manager.Instance.player.Constitution}, {Game_Manager.Instance.player.Intelligence}, {Game_Manager.Instance.player.Wisdom}, {Game_Manager.Instance.player.Willpower}");
            Game_Manager.Instance.displayPanels.LoadingScreen.SetActive(true);
            Game_Manager.Instance.GenerateWorld();
            
        }
    }
    bool OutOfStatPoints()
    {
        if (statPoints == 0)
        {
            return true;
        }
        return false;
    }
    bool CheckIfOptionsHaveChanged()
    {
        if (TempGender != Game_Manager.Instance.player.gender || TempRace != Game_Manager.Instance.player.race || TempClass != Game_Manager.Instance.player.starterClass && AllFieldsFilled)
        {
            return true;
        }
        return false;
    }
    bool CheckIfAllFieldsAreFilled()
    {
        if (characterName.text != "" && Game_Manager.Instance.player.gender != EntityGender.None && Game_Manager.Instance.player.race != EntityRace.None && Game_Manager.Instance.player.starterClass != StarterClass.None)
        {
            AllFieldsFilled = true;
            return true;
        }
        return false;
    }
    public void GetGenderInfo()
    {
        if (FirstTimeClicklingGenderDropdown)
        {
            FirstTimeClicklingGenderDropdown = false;
            genderDropdown.options.RemoveAt(0);
            switch (genderDropdown.value)
            {
                case 1:
                    Game_Manager.Instance.player.gender = EntityGender.Male;
                    genderDropdown.value = 0;
                    break;
                case 2:
                    Game_Manager.Instance.player.gender = EntityGender.Female;
                    genderDropdown.value = 1;
                    break;
            }
        }
        else
        {
            switch (genderDropdown.value)
            {
                case 0:
                    Game_Manager.Instance.player.gender = EntityGender.Male;
                    break;
                case 1:
                    Game_Manager.Instance.player.gender = EntityGender.Female;
                    break;
            }
        }
        genderDropdown.RefreshShownValue();
    }
    public void GetRaceInfo()
    {
        RaceDescriptions description = new RaceDescriptions();
        EntityRace newRace = EntityRace.None;
        string newRaceDesc = "";
        bool showPanel = false;

        // Determine the new race selection based on dropdown state.
        if (FirstTimeClickingRaceDropwon)
        {
            FirstTimeClickingRaceDropwon = false;
            raceDropdown.options.RemoveAt(0);
            // After removing the "Please Select" option, the mapping is:
            // 1: Human, 2: Elf, 3: Dwarf, 4: Gnome, 5: Halfling, 6: Fey.
            switch (raceDropdown.value)
            {
                case 1:
                    newRace = EntityRace.Human;
                    newRaceDesc = description.Human;
                    showPanel = true;
                    raceDropdown.value = 0;
                    break;
                case 2:
                    newRace = EntityRace.Elf;
                    newRaceDesc = description.Elf;
                    showPanel = true;
                    raceDropdown.value = 1;
                    break;
                case 3:
                    newRace = EntityRace.Dwarf;
                    newRaceDesc = description.Dwarf;
                    showPanel = true;
                    raceDropdown.value = 2;
                    break;
                case 4:
                    newRace = EntityRace.Gnome;
                    newRaceDesc = description.Gnome;
                    showPanel = true;
                    raceDropdown.value = 3;
                    break;
                case 5:
                    newRace = EntityRace.Halfling;
                    newRaceDesc = description.Halfling;
                    showPanel = true;
                    raceDropdown.value = 4;
                    break;
                case 6:
                    newRace = EntityRace.Fey;
                    newRaceDesc = description.Fey;
                    showPanel = true;
                    raceDropdown.value = 5;
                    break;
                default:
                    newRace = EntityRace.Human;
                    newRaceDesc = description.Human;
                    showPanel = true;
                    raceDropdown.value = 0;
                    break;
            }
        }
        else
        {
            // Normal mapping once the initial option is removed:
            // 0: Human, 1: Elf, 2: Dwarf, 3: Gnome, 4: Halfling, 5: Fey.
            switch (raceDropdown.value)
            {
                case 0:
                    newRace = EntityRace.Human;
                    newRaceDesc = description.Human;
                    showPanel = true;
                    break;
                case 1:
                    newRace = EntityRace.Elf;
                    newRaceDesc = description.Elf;
                    showPanel = true;
                    break;
                case 2:
                    newRace = EntityRace.Dwarf;
                    newRaceDesc = description.Dwarf;
                    showPanel = true;
                    break;
                case 3:
                    newRace = EntityRace.Gnome;
                    newRaceDesc = description.Gnome;
                    showPanel = true;
                    break;
                case 4:
                    newRace = EntityRace.Halfling;
                    newRaceDesc = description.Halfling;
                    showPanel = true;
                    break;
                case 5:
                    newRace = EntityRace.Fey;
                    newRaceDesc = description.Fey;
                    showPanel = true;
                    break;
                default:
                    newRace = EntityRace.Human;
                    newRaceDesc = description.Human;
                    showPanel = true;
                    break;
            }
        }

        // Check if a change has occurred during stat selection and reset stats if needed.
        if (StatSelectionAvailable && newRace != TempRace)
        {
            ResetStatSelections();
        }

        // Now assign the new race to the player.
        Game_Manager.Instance.player.race = newRace;
        raceInfo.text = newRaceDesc;
        RaceInfoPanel.SetActive(showPanel);

        // Update TempRace so future changes can be detected.
        TempRace = newRace;

        raceDropdown.RefreshShownValue();
    }
    public void UpdatePlayerName()
    {
        Game_Manager.Instance.player.entityName = characterName.text;
        Debug.Log($"Player name is {Game_Manager.Instance.player.entityName}");
    }
    public void GetClassInfo()
    {
        EntityClassDescriptions classDescriptions = new EntityClassDescriptions();
        StarterClass newClass = StarterClass.None;
        string newDescription = "";
        bool showPanel = false;

        // Determine the new class based on dropdown value.
        // If it's the first click, adjust the options accordingly.
        if (FirstTimeClickingClassDropdown)
        {
            FirstTimeClickingClassDropdown = false;
            classDropdown.options.RemoveAt(0);
            // Assuming that after removal, indices are:
            // 0: Fighter, 1: SpellCaster, 2: Ranger
            switch (classDropdown.value)
            {
                case 0:
                    classDropdown.value = 0;
                    newClass = StarterClass.Fighter;
                    newDescription = classDescriptions.Fighter;
                    showPanel = true;
                    break;
                case 1:
                    classDropdown.value = 1;
                    newClass = StarterClass.SpellCaster;
                    newDescription = classDescriptions.SpellCaster;
                    showPanel = true;
                    break;
                case 2:
                    classDropdown.value = 2;
                    newClass = StarterClass.Ranger;
                    newDescription = classDescriptions.BlankClass;
                    showPanel = true;
                    break;
            }
        }
        else
        {
            // Normal case without the initial removal
            switch (classDropdown.value)
            {
                case 0:
                    newClass = StarterClass.Fighter;
                    newDescription = classDescriptions.Fighter;
                    showPanel = true;
                    break;
                case 1:
                    newClass = StarterClass.SpellCaster;
                    newDescription = classDescriptions.SpellCaster;
                    showPanel = true;
                    break;
                case 2:
                    newClass = StarterClass.Ranger;
                    newDescription = classDescriptions.BlankClass;
                    showPanel = true;
                    break;
            }
        }

        // Check if the player is in stat selection mode and if the class has changed.
        if (StatSelectionAvailable && newClass != TempClass)
        {
            ResetStatSelections();
        }

        // Now assign the new class to the player.
        Game_Manager.Instance.player.starterClass = newClass;

        // Update the UI accordingly.
        classInfo.text = newDescription;
        ClassInfoPanel.SetActive(showPanel);

        // Update TempClass so future changes can be detected.
        TempClass = newClass;

        // Refresh the dropdown to reflect any changes.
        classDropdown.RefreshShownValue();
    }
    void ResetStatSelections()
    {
        statPoints = 10;
        statPointsAvailable.text = statPoints.ToString();
        strenth.text = "10";
        dexterity.text = "10";
        constitution.text = "10";
        intelligence.text = "10";
        wisdom.text = "10";
        willPower.text = "10";
    }
    void AddStrength()
    {
        Debug.Log("Adding Strength");
        if (statPoints > 0)
        {
            statPoints--;
            statPointsAvailable.text = statPoints.ToString();
            strenth.text = (int.Parse(strenth.text) + 1).ToString();
        }
    }
    void SubtractStrength()
    {
        if (statPoints < 10 && int.Parse(strenth.text) > 0)
        {
            statPoints++;
            statPointsAvailable.text = statPoints.ToString();
            strenth.text = (int.Parse(strenth.text) - 1).ToString();
        }
    }
    void AddDexterity()
    {
        if (statPoints > 0)
        {
            statPoints--;
            statPointsAvailable.text = statPoints.ToString();
            dexterity.text = (int.Parse(dexterity.text) + 1).ToString();
        }
    }
    void SubtractDexterity()
    {
        if (statPoints < 10 && int.Parse(dexterity.text) > 0)
        {
            statPoints++;
            statPointsAvailable.text = statPoints.ToString();
            dexterity.text = (int.Parse(dexterity.text) - 1).ToString();
        }
    }
    void AddConstitution()
    {
        if (statPoints > 0)
        {
            statPoints--;
            statPointsAvailable.text = statPoints.ToString();
            constitution.text = (int.Parse(constitution.text) + 1).ToString();
        }
    }
    void SubtractConstitution()
    {
        if (statPoints < 10 && int.Parse(constitution.text) > 0)
        {
            statPoints++;
            statPointsAvailable.text = statPoints.ToString();
            constitution.text = (int.Parse(constitution.text) - 1).ToString();
        }
    }
    void AddIntelligence()
    {
        if (statPoints > 0)
        {
            statPoints--;
            statPointsAvailable.text = statPoints.ToString();
            intelligence.text = (int.Parse(intelligence.text) + 1).ToString();
        }
    }
    void SubtractIntelligence()
    {
        if (statPoints < 10 && int.Parse(intelligence.text) > 0)
        {
            statPoints++;
            statPointsAvailable.text = statPoints.ToString();
            intelligence.text = (int.Parse(intelligence.text) - 1).ToString();
        }
    }
    void AddWisdom()
    {
        if (statPoints > 0)
        {
            statPoints--;
            statPointsAvailable.text = statPoints.ToString();
            wisdom.text = (int.Parse(wisdom.text) + 1).ToString();
        }
    }
    void SubtractWisdom()
    {
        if (statPoints < 10 && int.Parse(wisdom.text) > 0)
        {
            statPoints++;
            statPointsAvailable.text = statPoints.ToString();
            wisdom.text = (int.Parse(wisdom.text) - 1).ToString();
        }
    }
    void AddWillPower()
    {
        if (statPoints > 0)
        {
            statPoints--;
            statPointsAvailable.text = statPoints.ToString();
            willPower.text = (int.Parse(willPower.text) + 1).ToString();
        }
    }
    void SubtractWillPower()
    {
        if (statPoints < 10 && int.Parse(willPower.text) > 0)
        {
            statPoints++;
            statPointsAvailable.text = statPoints.ToString();
            willPower.text = (int.Parse(willPower.text) - 1).ToString();
        }
    }
}
    public class RaceDescriptions
{
    public readonly string Human = "Humans are a tapestry of ambition and resilience¡ªbuilders of empires and pioneers of uncharted frontiers, their diverse cultures and boundless curiosity drive the ever-changing world.";
    public readonly string Elf = "Elves embody the grace of ancient forests and the mysteries of old magic; ageless and elegant, they serve as living conduits between nature¡¯s eternal beauty and the arcane secrets of the world.";
    public readonly string Dwarf = "Dwarves are the stalwart guardians of the earth, master craftsmen and fearless warriors whose legacy is etched in the stone of mighty mountain halls and the flames of forges long passed.";
    public readonly string Gnome = "Gnomes are the ingenious tinkerers and whimsical tricksters, whose quick wits and inventive minds turn the mundane into marvels, blending arcane secrets with mechanical ingenuity.";
    public readonly string Halfling = "Halflings are the heart of simple joys and quiet heroism; their nimble footsteps and warm spirits reveal a brave soul behind every unassuming smile, forever seeking adventure in the little wonders of life.";
    public readonly string Fey = "Fey are the enigmatic denizens of the wild and mysterious; capricious and bewitching, they straddle the line between reality and dream, leaving trails of wonder and mischief wherever they wander.";

}

public class EntityClassDescriptions
{
    public readonly string Fighter = "Fighters are the embodiment of martial prowess and raw determination¡ªmasters of arms whose rigorous training and indomitable spirit pave the way for a multitude of melee specializations. Whether wielding a sword or a battle axe, their focus on discipline and strength transforms the chaos of battle into a dance of precision.";
    public readonly string SpellCaster = "Spell Casters are the architects of arcane forces, channeling the mysterious energies of the cosmos to both protect and devastate. Their command over magic opens the door to a spectrum of mystical disciplines, where every incantation and elemental surge weaves a new story in the tapestry of power.";
    public readonly string BlankClass = "This class is a realm yet to be charted¡ªa blank canvas brimming with untapped potential. Awaiting the spark of creativity, it promises future adventures and innovative abilities that will redefine the boundaries of traditional roles.";

}

