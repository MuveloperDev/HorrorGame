using System.Collections.Generic;
using Enum;
[System.Serializable]
public class InventoryItemInfoData
{
    public int Id;
    public string Description;
    public string RolloverResourcePath;
    public static Dictionary<int, InventoryItemInfoData> table = new Dictionary<int, InventoryItemInfoData> ();   
}