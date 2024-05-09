using System.Collections.Generic;
using Enum;
[System.Serializable]
public class TestData
{
    public int Id;
    public string Description;
    public string Name;
    public string Layer;
    public int UnlockLevel;
    public int[] UnlockLevel3;
    public int[][] UnlockLevel4;
    public int[][][] UnlockLevel5;
    public object BoolData;
    public float FloatData;
    public float[] UnlockLevel6;
    public static Dictionary<int, TestData> table = new Dictionary<int, TestData> ();   
}