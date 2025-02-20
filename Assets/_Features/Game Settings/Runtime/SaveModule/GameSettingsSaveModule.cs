using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public class GameSettingsSaveModule<T> where T : ISettingsSaveData
{
    public void Save(T save)
    {
        string path = Application.persistentDataPath + "/" + typeof(T) + ".txt";

        string json = JsonConvert.SerializeObject(save);
        File.WriteAllText(path, json);
    }

    public void Load(out T load)
    {
        load = default;

        string path = Application.persistentDataPath + "/" + typeof(T) + ".txt";

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            load = JsonConvert.DeserializeObject<T>(json);
        }
    }
}
