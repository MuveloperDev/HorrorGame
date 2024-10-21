using System.Collections.Generic;
using Enum;
[System.Serializable]
public class CameraShakeData
{
    public int Id;
    public bool ScaledTime;
    public bool Camera;
    public bool Canvases;
    public bool Objects;
    public bool UnlimitedDuration;
    public float TotalDuration;
    public float FadeInDuration;
    public float FadeOutDuration;
    public float Magnitude;
    public float MagnitudeNoise;
    public string MagnitudeCurve;
    public float Roughness;
    public float RoughnessNoise;
    public string RoughnessCurve;
    public float[] PositionalInfluence;
    public int[] PositionalInverts;
    public float[] RotationalInfluence;
    public int[] RotationalInverts;
    public bool RandomSeed;
    public static Dictionary<int, CameraShakeData> table = new Dictionary<int, CameraShakeData> ();   
}