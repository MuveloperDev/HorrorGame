using System.Collections.Generic;
using Enum;
[System.Serializable]
public class StringData
{
    public int Id;
    public string En;
    public string Kr;
    public static Dictionary<int, StringData> table = new Dictionary<int, StringData> ();   
}