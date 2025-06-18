using System;
using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

public class GameSettingsSaveModule<T> where T : ISettingsSaveData, new()
{
    private static Queue<T> s_saveQueue = new();

    private readonly string path = Application.persistentDataPath + "/" + typeof(T) + ".save";

    public void Save(T save)
    {
        string json = JsonConvert.SerializeObject(save, Formatting.Indented, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto });
        File.WriteAllText(path, json);
    }

    public void Load(out T load)
    {
        load = default;

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            load = JsonConvert.DeserializeObject<T>(json, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto });
            return;
        }

        Save(new T());
    }

    public async UniTask SaveAsync(T saveData)
    {
        s_saveQueue.Enqueue(saveData);

        while (s_saveQueue.Count > 0)
        {
            await InternalSaveAsync(s_saveQueue.Dequeue());
        }
    }

    public async UniTask<T> LoadAsync()
    {
        try
        {
            using StreamReader reader = new(path);

            string json = await reader.ReadToEndAsync();
            return JsonConvert.DeserializeObject<T>(json, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto });
        }
        catch (IOException e)
        {
            Console.WriteLine($"The file could not be read: \n{e.Message}");

            Save(new T());
            Load(out T loadGame);

            return loadGame;
        }
    }

    private async UniTask InternalSaveAsync(T saveData)
    {
        string path = Application.persistentDataPath + "/" + typeof(T) + ".save";
        // Set a variable to the Documents path.
        string docPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        // Write the specified text asynchronously to a new file named "WriteTextAsync.txt".
        using (StreamWriter outputFile = new StreamWriter(path))
        {
            string json = JsonConvert.SerializeObject(saveData, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto });
            await outputFile.WriteAsync(json);
        }
    }


}
