using Cysharp.Threading.Tasks;
using FirstGearGames.SmoothCameraShaker;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShakeManager
{
    private static readonly object _lock = new object();
    private static CameraShakeManager _instance;
    private bool isExistence = false;
    private CameraShakeManager()
    {
        Initialize();
    }

    public static CameraShakeManager Instance
    {
        get
        {
            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = new CameraShakeManager();
                }

                return _instance;
            }
        }

    }

    private CurveSerializer curveSerializer = new CurveSerializer();

    private Dictionary<int, ShakeData> _shakeDataDic = new();
    private Dictionary<int, ShakeDataStruct> _shakeDataStructDic = new();
    private ShakeDataStruct _shakeData = new();
    private bool _isMyCamera = false;
    private bool _checkCameraOwnership = false;
    private bool _isIgnored = false;
    public void Initialize()
    {
        if (true == isExistence)
            return;
#if UNITY_EDITOR
        DataIntegrityValidation();
#endif
        foreach (var data in CameraShakeData.table.Values)
        {
            ShakeData shakeData = new ShakeData();

            AnimationCurve maginutdeCurve = curveSerializer.DeserializeCurve(curveSerializer.stringToByteArray(data.MagnitudeCurve));
            AnimationCurve roughnessCurve = curveSerializer.DeserializeCurve(curveSerializer.stringToByteArray(data.RoughnessCurve));

            shakeData.SetInstancedWithProperties(
                data.ScaledTime,
                data.Camera,
                data.Canvases,
                data.Objects,
                data.UnlimitedDuration,
                data.Magnitude,
                data.MagnitudeNoise,
                maginutdeCurve,
                data.Roughness,
                data.RoughnessNoise,
                roughnessCurve,
                data.TotalDuration,
                data.FadeInDuration,
                data.FadeOutDuration,
                new Vector3(data.PositionalInfluence[0], data.PositionalInfluence[1], data.PositionalInfluence[2]),
                GenerateInvertibleAxes(data.PositionalInverts),
                new Vector3(data.RotationalInfluence[0], data.RotationalInfluence[1], data.RotationalInfluence[2]),
                GenerateInvertibleAxes(data.RotationalInverts),
                data.RandomSeed
                );
            _shakeDataStructDic.Add(data.Id, new ShakeDataStruct(shakeData));
            _shakeDataDic.Add(data.Id, shakeData);
        }
        curveSerializer = null;
        isExistence = true;
    }

    public async void PlayCameraShake(int id)
    {
        await UniTask.WaitUntil(() => true == isExistence);

#if UNITY_EDITOR
        ReLoadTableData();
#endif

        ShakeData data = GetShakeData(id);
        if (null == data)
            return;

        CameraShakerHandler.Stop();
        _shakeData.InitData(data);
        CameraShakerHandler.Shake(data);

        //_isMyCamera = false;
        void ReLoadTableData()
        {
            _shakeDataStructDic.Clear();
            _shakeDataDic.Clear();
            curveSerializer = new();
            foreach (var data in CameraShakeData.table.Values)
            {
                ShakeData shakeData = new ShakeData();
                AnimationCurve maginutdeCurve = curveSerializer.DeserializeCurve(curveSerializer.stringToByteArray(data.MagnitudeCurve));
                AnimationCurve roughnessCurve = curveSerializer.DeserializeCurve(curveSerializer.stringToByteArray(data.RoughnessCurve));

                shakeData.SetInstancedWithProperties(
                    data.ScaledTime,
                    data.Camera,
                    data.Canvases,
                    data.Objects,
                    data.UnlimitedDuration,
                    data.Magnitude,
                    data.MagnitudeNoise,
                    maginutdeCurve,
                    data.Roughness,
                    data.RoughnessNoise,
                    roughnessCurve,
                    data.TotalDuration,
                    data.FadeInDuration,
                    data.FadeOutDuration,
                    new Vector3(data.PositionalInfluence[0], data.PositionalInfluence[1], data.PositionalInfluence[2]),
                    GenerateInvertibleAxes(data.PositionalInverts),
                    new Vector3(data.RotationalInfluence[0], data.RotationalInfluence[1], data.RotationalInfluence[2]),
                    GenerateInvertibleAxes(data.RotationalInverts),
                    data.RandomSeed
                    );
                _shakeDataStructDic.Add(data.Id, new ShakeDataStruct(shakeData));
                _shakeDataDic.Add(data.Id, shakeData);
            }
            curveSerializer = null;
        }
    }

    public async void PlayCameraShakeForce(int id)
    {
        await UniTask.WaitUntil(() => true == isExistence);

#if UNITY_EDITOR
        ReLoadTableData();
#endif

        ShakeData data = GetShakeData(id);
        if (null == data)
            return;

        CameraShakerHandler.Stop();
        _shakeData.InitData(data);
        CameraShakerHandler.Shake(data);

        void ReLoadTableData()
        {
            _shakeDataStructDic.Clear();
            _shakeDataDic.Clear();
            curveSerializer = new();
            foreach (var data in CameraShakeData.table.Values)
            {
                ShakeData shakeData = new ShakeData();
                AnimationCurve maginutdeCurve = curveSerializer.DeserializeCurve(curveSerializer.stringToByteArray(data.MagnitudeCurve));
                AnimationCurve roughnessCurve = curveSerializer.DeserializeCurve(curveSerializer.stringToByteArray(data.RoughnessCurve));

                shakeData.SetInstancedWithProperties(
                    data.ScaledTime,
                    data.Camera,
                    data.Canvases,
                    data.Objects,
                    data.UnlimitedDuration,
                    data.Magnitude,
                    data.MagnitudeNoise,
                    maginutdeCurve,
                    data.Roughness,
                    data.RoughnessNoise,
                    roughnessCurve,
                    data.TotalDuration,
                    data.FadeInDuration,
                    data.FadeOutDuration,
                    new Vector3(data.PositionalInfluence[0], data.PositionalInfluence[1], data.PositionalInfluence[2]),
                    GenerateInvertibleAxes(data.PositionalInverts),
                    new Vector3(data.RotationalInfluence[0], data.RotationalInfluence[1], data.RotationalInfluence[2]),
                    GenerateInvertibleAxes(data.RotationalInverts),
                    data.RandomSeed
                    );
                _shakeDataStructDic.Add(data.Id, new ShakeDataStruct(shakeData));
                _shakeDataDic.Add(data.Id, shakeData);
            }
            curveSerializer = null;
        }
    }

    public async void SyncPlayCameraShake(int id)
    {
        await UniTask.WaitUntil(() => true == isExistence);

#if UNITY_EDITOR
        ReLoadTableData();
#endif

        ShakeData data = GetShakeData(id);
        if (null == data)
            return;

        CameraShakerHandler.Stop();
        _shakeData.InitData(data);
        CameraShakerHandler.Shake(data);

        void ReLoadTableData()
        {
            _shakeDataStructDic.Clear();
            _shakeDataDic.Clear();
            curveSerializer = new();
            foreach (var data in CameraShakeData.table.Values)
            {
                ShakeData shakeData = new ShakeData();
                AnimationCurve maginutdeCurve = curveSerializer.DeserializeCurve(curveSerializer.stringToByteArray(data.MagnitudeCurve));
                AnimationCurve roughnessCurve = curveSerializer.DeserializeCurve(curveSerializer.stringToByteArray(data.RoughnessCurve));

                shakeData.SetInstancedWithProperties(
                    data.ScaledTime,
                    data.Camera,
                    data.Canvases,
                    data.Objects,
                    data.UnlimitedDuration,
                    data.Magnitude,
                    data.MagnitudeNoise,
                    maginutdeCurve,
                    data.Roughness,
                    data.RoughnessNoise,
                    roughnessCurve,
                    data.TotalDuration,
                    data.FadeInDuration,
                    data.FadeOutDuration,
                    new Vector3(data.PositionalInfluence[0], data.PositionalInfluence[1], data.PositionalInfluence[2]),
                    GenerateInvertibleAxes(data.PositionalInverts),
                    new Vector3(data.RotationalInfluence[0], data.RotationalInfluence[1], data.RotationalInfluence[2]),
                    GenerateInvertibleAxes(data.RotationalInverts),
                    data.RandomSeed
                    );
                _shakeDataStructDic.Add(data.Id, new ShakeDataStruct(shakeData));
                _shakeDataDic.Add(data.Id, shakeData);
            }
            curveSerializer = null;
        }
    }

    public async void StopCameraShake()
    {
        await UniTask.WaitUntil(() => true == isExistence);

        CameraShakerHandler.Stop();
    }

    public ShakeData GetShakeData(int id)
    {
        if (true == _shakeDataDic.TryGetValue(id, out ShakeData data))
        {
            if (false == _shakeDataStructDic[id].IntegrityCompareData(data))
                _shakeDataStructDic[id].SetData(data);

            return data;
        }
        Debug.LogError($"{GetType()} Id is invalid.. please check \"CameraShakeData\" id..");
        return null;
    }

    public void SetOwnershipCamera(bool value)
    {
        _isMyCamera = value;
    }
    public void ResetOwnershipCamera()
    {
        _isMyCamera = _originIsMyCameraValue;
    }
    bool _originIsMyCameraValue = false;

    private AnimationCurve GenerateAnimatorCurve(float[][] keyframDatas)
    {
        List<Keyframe> KeyFramsList = new();
        foreach (var keys in keyframDatas)
        {
            Keyframe keyFrame = new Keyframe(keys[0], keys[1]);
            KeyFramsList.Add(keyFrame);
        }
        Keyframe[] KeyArray = KeyFramsList.ToArray();
        return new AnimationCurve(KeyArray);
    }
    private InvertibleAxes GenerateInvertibleAxes(int[] array)
    {
        InvertibleAxes currentInvert = 0;
        if (array[0] == 0 && array[1] == 0 && array[2] == 0)
        {
            currentInvert = 0; // Nothing
        }
        else if (array[0] == 1 && array[1] == 1 && array[2] == 1)
        {
            currentInvert = InvertibleAxes.X | InvertibleAxes.Y | InvertibleAxes.Z; // Everything
        }
        else
        {
            if (array[0] == 1)
            {
                currentInvert = InvertibleAxes.X;
            }
            else if (array[1] == 1)
            {
                currentInvert = InvertibleAxes.Y;
            }
            else if (array[2] == 1)
            {
                currentInvert = InvertibleAxes.Z;
            }
        }
        return currentInvert;
    }

    #region[Data Integrity Validation]
    public void DataIntegrityValidation()
    {
        bool isEquals = false;
        foreach (var data1 in CameraShakeData.table)
        {
            int key1 = data1.Key;
            foreach (var data2 in CameraShakeData.table)
            {
                int key2 = data2.Key;
                if (key1 == key2)
                    continue;

                isEquals = Equals(data1, data2);
            }
        }
        if (false == isEquals)
        {
            Debug.Log("CameraShakeData Validation.");
        }
    }
    bool Equals(CameraShakeData data, CameraShakeData other)
    {
        bool isEquals = AreBasicPropertiesEqual(data, other) && AreArrayPropertiesEqual(data, other);
        if (true == isEquals)
        {
            Debug.LogError($"Is exist same Data : ids - {data.Id} / {other.Id}");
            return isEquals;
        }
        return isEquals;
    }
    bool AreBasicPropertiesEqual(CameraShakeData data, CameraShakeData other)
    {
        return data.ScaledTime == other.ScaledTime &&
               data.Camera == other.Camera &&
               data.Canvases == other.Canvases &&
               data.Objects == other.Objects &&
               data.UnlimitedDuration == other.UnlimitedDuration &&
               data.TotalDuration == other.TotalDuration &&
               data.FadeInDuration == other.FadeInDuration &&
               data.FadeOutDuration == other.FadeOutDuration &&
               data.Magnitude == other.Magnitude &&
               data.MagnitudeNoise == other.MagnitudeNoise &&
               data.MagnitudeCurve == other.MagnitudeCurve &&
               data.Roughness == other.Roughness &&
               data.RoughnessNoise == other.RoughnessNoise &&
               data.RoughnessCurve == other.RoughnessCurve &&
               data.RandomSeed == other.RandomSeed;
    }
    bool AreArrayPropertiesEqual(CameraShakeData data, CameraShakeData other)
    {
        return true == EqualsArray(data.PositionalInfluence, other.PositionalInfluence) &&
               true == EqualsArray(data.PositionalInverts, other.PositionalInverts) &&
               true == EqualsArray(data.RotationalInfluence, other.RotationalInfluence) &&
               true == EqualsArray(data.RotationalInverts, other.RotationalInverts);
    }
    bool EqualsJaggedArray(float[][] array1, float[][] array2)
    {
        bool isEqual = true;
        if (array1.Length == array2.Length)
        {
            for (int i = 0; i < array1.Length; i++)
            {
                if (array1[i].Length != array2[i].Length)
                {
                    isEqual = false;
                    break;
                }

                for (int j = 0; j < array1[i].Length; j++)
                {
                    if (array1[i][j] != array2[i][j])
                    {
                        isEqual = false;
                        break;
                    }
                }

                if (!isEqual)
                    break;
            }
        }
        else
        {
            isEqual = false;
        }
        return isEqual;
    }
    bool EqualsArray(float[] array1, float[] array2)
    {
        bool isEqual = true;

        if (array1.Length == array2.Length)
        {
            for (int i = 0; i < array1.Length; i++)
            {
                if (array1[i] != array2[i])
                {
                    isEqual = false;
                    break;
                }
            }
        }
        else
        {
            isEqual = false;
        }

        return isEqual;
    }
    bool EqualsArray(int[] array1, int[] array2)
    {
        bool isEqual = true;

        if (array1.Length == array2.Length)
        {
            for (int i = 0; i < array1.Length; i++)
            {
                if (array1[i] != array2[i])
                {
                    isEqual = false;
                    break;
                }
            }
        }
        else
        {
            isEqual = false;
        }

        return isEqual;
    }
    #endregion
    public bool SetIgnore(bool value) => _isIgnored = value;
    private struct ShakeDataStruct
    {
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
        public AnimationCurve MagnitudeCurve;
        public float Roughness;
        public float RoughnessNoise;
        public AnimationCurve RoughnessCurve;
        public Vector3 PositionalInfluence;
        public InvertibleAxes PositionalInverts;
        public Vector3 RotationalInfluence;
        public InvertibleAxes RotationalInverts;
        public bool RandomSeed;
        public ShakeDataStruct(ShakeData data)
        {
            ScaledTime = data.ScaledTime;
            Camera = data.ShakeCameras;
            Canvases = data.ShakeCanvases;
            Objects = data.ShakeObjects;
            UnlimitedDuration = data.UnlimitedDuration;
            TotalDuration = data.TotalDuration;
            FadeInDuration = data.FadeInDuration;
            FadeOutDuration = data.FadeOutDuration;
            Magnitude = data.Magnitude;
            MagnitudeNoise = data.MagnitudeNoise;
            MagnitudeCurve = data.MagnitudeCurve;
            Roughness = data.Roughness;
            RoughnessNoise = data.RoughnessNoise;
            RoughnessCurve = data.RoughnessCurve;
            PositionalInfluence = data.PositionalInfluence;
            PositionalInverts = data.PositionalInverts;
            RotationalInfluence = data.RotationalInfluence;
            RotationalInverts = data.RotationalInverts;
            RandomSeed = data.RandomSeed;
        }
        public void InitData(ShakeData data)
        {
            ScaledTime = data.ScaledTime;
            Camera = data.ShakeCameras;
            Canvases = data.ShakeCanvases;
            Objects = data.ShakeObjects;
            UnlimitedDuration = data.UnlimitedDuration;
            TotalDuration = data.TotalDuration;
            FadeInDuration = data.FadeInDuration;
            FadeOutDuration = data.FadeOutDuration;
            Magnitude = data.Magnitude;
            MagnitudeNoise = data.MagnitudeNoise;
            MagnitudeCurve = data.MagnitudeCurve;
            Roughness = data.Roughness;
            RoughnessNoise = data.RoughnessNoise;
            RoughnessCurve = data.RoughnessCurve;
            PositionalInfluence = data.PositionalInfluence;
            PositionalInverts = data.PositionalInverts;
            RotationalInfluence = data.RotationalInfluence;
            RotationalInverts = data.RotationalInverts;
            RandomSeed = data.RandomSeed;
        }
        public bool IntegrityCompareData(ShakeData data)
        {
            if (ScaledTime == data.ScaledTime &&
                Camera == data.ShakeCameras &&
                Canvases == data.ShakeCanvases &&
                Objects == data.ShakeObjects &&
                UnlimitedDuration == data.UnlimitedDuration &&
                TotalDuration == data.TotalDuration &&
                FadeInDuration == data.FadeInDuration &&
                FadeOutDuration == data.FadeOutDuration &&
                Magnitude == data.Magnitude &&
                MagnitudeNoise == data.MagnitudeNoise &&
                MagnitudeCurve == data.MagnitudeCurve &&
                Roughness == data.Roughness &&
                RoughnessNoise == data.RoughnessNoise &&
                RoughnessCurve == data.RoughnessCurve &&
                PositionalInfluence == data.PositionalInfluence &&
                PositionalInverts == data.PositionalInverts &&
                RotationalInfluence == data.RotationalInfluence &&
                RotationalInverts == data.RotationalInverts &&
                RandomSeed == data.RandomSeed)
            {
                return true;
            }
            return false;
        }

        public void SetData(ShakeData data)
        {
            data.SetInstancedWithProperties(
                ScaledTime,
                Camera,
                Canvases,
                Objects,
                UnlimitedDuration,
                Magnitude,
                MagnitudeNoise,
                MagnitudeCurve,
                Roughness,
                RoughnessNoise,
                RoughnessCurve,
                TotalDuration,
                FadeInDuration,
                FadeOutDuration,
                PositionalInfluence,
                PositionalInverts,
                RotationalInfluence,
                RotationalInverts,
                RandomSeed
                );
        }
    }
}
//[System.Serializable]
//public class CurveSerializer
//{
//    public byte[] SerializeCurve(AnimationCurve curve)
//    {
//        BinaryFormatter formatter = new BinaryFormatter();
//        MemoryStream stream = new MemoryStream();

//        formatter.Serialize(stream, new SerializableCurve(curve));

//        return stream.ToArray();
//    }

//    public AnimationCurve DeserializeCurve(byte[] data)
//    {
//        BinaryFormatter formatter = new BinaryFormatter();
//        MemoryStream stream = new MemoryStream(data);

//        SerializableCurve serializableCurve = (SerializableCurve)formatter.Deserialize(stream);

//        return serializableCurve.ToCurve();
//    }

//    public byte[] stringToByteArray(string data)
//    {
//        List<byte> bytes = new();
//        string[] tokens = data.Split(',');
//        foreach (var token in tokens)
//        {
//            byte byteValue;
//            if (byte.TryParse(token, out byteValue))
//            {
//                bytes.Add(byteValue);
//            }
//            else
//            {
//                Console.WriteLine($"'{token}' is out of range for a Byte.");
//            }
//        }
//        return bytes.ToArray();
//    }
//}

//[System.Serializable]
//public class SerializableKeyframe
//{
//    public float time, value, inTangent, outTangent;

//    public SerializableKeyframe(Keyframe key)
//    {
//        time = key.time;
//        value = key.value;
//        inTangent = key.inTangent;
//        outTangent = key.outTangent;
//    }

//    public Keyframe ToKeyframe()
//    {
//        return new Keyframe(time, value, inTangent, outTangent);
//    }
//}

//[System.Serializable]
//public class SerializableCurve
//{
//    public SerializableKeyframe[] keys;

//    public SerializableCurve(AnimationCurve curve)
//    {
//        keys = new SerializableKeyframe[curve.keys.Length];

//        for (int i = 0; i < curve.keys.Length; i++)
//        {
//            keys[i] = new SerializableKeyframe(curve.keys[i]);
//        }
//    }

//    public AnimationCurve ToCurve()
//    {
//        AnimationCurve curve = new AnimationCurve();

//        foreach (SerializableKeyframe key in keys)
//        {
//            curve.AddKey(key.ToKeyframe());
//        }

//        return curve;
//    }
//}
