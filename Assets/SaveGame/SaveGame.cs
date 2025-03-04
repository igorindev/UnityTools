using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using UnityEngine;

public static class SaveGame
{   
    //Binary
    public static void Save<T>(T save)
    {
        BinaryFormatter formatter = new BinaryFormatter();

        SurrogateSelector surrogateSelector = new SurrogateSelector();

        Vector3SerializationSurrogate vector3SS = new Vector3SerializationSurrogate();
        surrogateSelector.AddSurrogate(typeof(Vector3), new StreamingContext(StreamingContextStates.All), vector3SS);

        QuaternionSerializationSurrogate quaternionSS = new QuaternionSerializationSurrogate();
        surrogateSelector.AddSurrogate(typeof(Quaternion), new StreamingContext(StreamingContextStates.All), quaternionSS);

        formatter.SurrogateSelector = surrogateSelector;

        string path = Application.persistentDataPath + "/" + save.GetType().ToString() + ".txt";
        FileStream stream = new FileStream(path, FileMode.OpenOrCreate);
        formatter.Serialize(stream, save);
        stream.Close();

        Debug.Log("Saved at path: " + path);
    }
    public static void Load<T>(ref T load)
    {
        string path = Application.persistentDataPath + "/" + load.GetType().ToString() + ".txt";
        
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();

            SurrogateSelector surrogateSelector = new SurrogateSelector();

            Vector3SerializationSurrogate vector3SS = new Vector3SerializationSurrogate();
            surrogateSelector.AddSurrogate(typeof(Vector3), new StreamingContext(StreamingContextStates.All), vector3SS);

            QuaternionSerializationSurrogate quaternionSS = new QuaternionSerializationSurrogate();
            surrogateSelector.AddSurrogate(typeof(Quaternion), new StreamingContext(StreamingContextStates.All), quaternionSS);

            formatter.SurrogateSelector = surrogateSelector;

            FileStream stream = new FileStream(path, FileMode.Open);

            load = (T)formatter.Deserialize(stream);
            stream.Close();

            
        }

        Debug.Log("Loaded at path: " + path);
    }

    //XML
    public static void SaveXml<T>(T save)
    {
        string path = Application.persistentDataPath + "/" + save.GetType().ToString() + ".txt";
        FileStream stream = new FileStream(path, FileMode.OpenOrCreate);
        XmlSerializer formatter = new XmlSerializer(save.GetType());
        formatter.Serialize(stream, save);
        stream.Close();

        Debug.Log("Saved at path: " + path);
    }
    public static void LoadXml<T>(ref T load)
    {
        string path = Application.persistentDataPath + "/" + load.GetType().ToString() + ".txt";

        if (File.Exists(path))
        {
            FileStream stream = new FileStream(path, FileMode.Open);
            XmlSerializer formatter = new XmlSerializer(load.GetType());
            load = (T)formatter.Deserialize(stream);
            stream.Close();
        }

        Debug.Log("Loaded at path: " + path);
    }

    //Json
    public static void SaveJson<T>(T save)
    {
        string path = Application.persistentDataPath + "/" + save.GetType().ToString() + ".txt";

        string json = JsonUtility.ToJson(save);
        File.WriteAllText(path, json);

        Debug.Log("Saved at path: " + path);
    }
    public static void LoadJson<T>(ref T load)
    {
        string path = Application.persistentDataPath + "/" + load.GetType().ToString() + ".txt";

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            load = JsonUtility.FromJson<T>(json);
        }

        Debug.Log("Loaded at path: " + path);
    }
}

public class Vector3SerializationSurrogate : ISerializationSurrogate
{

    // Method called to serialize a Vector3 object
    public void GetObjectData(System.Object obj, SerializationInfo info, StreamingContext context)
    {
        Vector3 v3 = (Vector3)obj;
        info.AddValue("x", v3.x);
        info.AddValue("y", v3.y);
        info.AddValue("z", v3.z);
    }

    // Method called to deserialize a Vector3 object
    public System.Object SetObjectData(System.Object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
    {
        Vector3 v3 = (Vector3)obj;
        v3.x = (float)info.GetValue("x", typeof(float));
        v3.y = (float)info.GetValue("y", typeof(float));
        v3.z = (float)info.GetValue("z", typeof(float));
        obj = v3;
        return obj;
    }
}
public class QuaternionSerializationSurrogate : ISerializationSurrogate
{

    // Method called to serialize a Vector3 object
    public void GetObjectData(System.Object obj, SerializationInfo info, StreamingContext context)
    {
        Quaternion q = (Quaternion)obj;
        info.AddValue("x", q.x);
        info.AddValue("y", q.y);
        info.AddValue("z", q.z);
        info.AddValue("w", q.w);
    }

    // Method called to deserialize a Vector3 object
    public System.Object SetObjectData(System.Object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
    {
        Quaternion q = (Quaternion)obj;
        q.x = (float)info.GetValue("x", typeof(float));
        q.y = (float)info.GetValue("y", typeof(float));
        q.z = (float)info.GetValue("z", typeof(float));
        q.w = (float)info.GetValue("w", typeof(float));
        obj = q;
        return obj;
    }
}
