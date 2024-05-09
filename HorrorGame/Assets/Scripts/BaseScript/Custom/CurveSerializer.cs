using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
[System.Serializable]
public class CurveSerializer
{
    public byte[] SerializeCurve(AnimationCurve curve)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        MemoryStream stream = new MemoryStream();

        formatter.Serialize(stream, new SerializableCurve(curve));

        return stream.ToArray();
    }

    public AnimationCurve DeserializeCurve(byte[] data)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        MemoryStream stream = new MemoryStream(data);

        SerializableCurve serializableCurve = (SerializableCurve)formatter.Deserialize(stream);

        return serializableCurve.ToCurve();
    }

    public byte[] stringToByteArray(string data)
    {
        List<byte> bytes = new();
        string[] tokens = data.Split(',');
        foreach (var token in tokens)
        {
            byte byteValue;
            if (byte.TryParse(token, out byteValue))
            {
                bytes.Add(byteValue);
            }
            else
            {
                Console.WriteLine($"'{token}' is out of range for a Byte.");
            }
        }
        return bytes.ToArray();
    }
}

[System.Serializable]
public class SerializableKeyframe
{
    public float time, value, inTangent, outTangent;

    public SerializableKeyframe(Keyframe key)
    {
        time = key.time;
        value = key.value;
        inTangent = key.inTangent;
        outTangent = key.outTangent;
    }

    public Keyframe ToKeyframe()
    {
        return new Keyframe(time, value, inTangent, outTangent);
    }
}

[System.Serializable]
public class SerializableCurve
{
    public SerializableKeyframe[] keys;

    public SerializableCurve(AnimationCurve curve)
    {
        keys = new SerializableKeyframe[curve.keys.Length];

        for (int i = 0; i < curve.keys.Length; i++)
        {
            keys[i] = new SerializableKeyframe(curve.keys[i]);
        }
    }

    public AnimationCurve ToCurve()
    {
        AnimationCurve curve = new AnimationCurve();

        foreach (SerializableKeyframe key in keys)
        {
            curve.AddKey(key.ToKeyframe());
        }

        return curve;
    }
}
