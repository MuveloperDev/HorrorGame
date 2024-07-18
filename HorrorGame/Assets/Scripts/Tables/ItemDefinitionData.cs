using System.Collections.Generic;
using Enum;
[System.Serializable]
public class ItemDefinitionData
{
    public int Id;
    public int Description;
    public string ResourcePath;
    public int Ref_InventoryItemInfo;
    public static Dictionary<int, ItemDefinitionData> table = new Dictionary<int, ItemDefinitionData> ();   
}