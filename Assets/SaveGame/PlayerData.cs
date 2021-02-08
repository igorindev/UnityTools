public struct PlayerData
{
    public int credits;
    public int crystal;
    public int[] playerAttributes; //qual atributo e qual seu valor
    public bool weaponsUnlocked;
    public WeaponData[] allWeapons; //armas - array de 4
    public int[] gearWeapons;
}
public struct WeaponData
{
    public int index;
    public int[] modsIndex;
    public int upgradeValue;
}
public struct PlayerLevel
{
    public int level;
    public int actualExp;
    public int nextLevelExp;
    public int skillPoints;
}
public struct QuestsData
{
    public int[] activeQuests;
    public bool[] Completed;
}
public struct Inventory
{                            //Array dimensions-> 0   1   2   3
    public int[] _backpack;  //ex int[] = ids --> 9 , 10, 23, 2
    public int[] bagQuantity; //quant do item --> 3 , 34, 18, 20

    public int[] _stash;
    public int[] stashQuantity;

    public int[] mods;
}
public struct Unlocks
{
    public bool[] unlockedWeapons;
    public bool[] unlockedCraftingRecipes;
    public bool[] unlockedLevels;
    public bool[] unlockedMonstersInfo;
}
