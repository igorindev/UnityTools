#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using static UnityEditor.Progress;

public class GenerateEnum
{
    [MenuItem("Tools/GenerateEnum")]
    public static void CreateEnum(string enumName, string[] enumEntries)
    {
        string filePathAndName = "Assets/Scripts/GeneratedEnum/" + enumName + ".cs";

        using (StreamWriter streamWriter = new StreamWriter(filePathAndName))
        {
            streamWriter.WriteLine("public enum " + enumName);
            streamWriter.WriteLine("{");
            for (int i = 0; i < enumEntries.Length; i++)
            {
                string entry = enumEntries[i];
                if (string.IsNullOrEmpty(entry))
                {
                    continue;
                }

                streamWriter.WriteLine($"\t{entry},");
            }

            streamWriter.WriteLine("}");
        }

        AssetDatabase.Refresh();
    }

    [MenuItem("Tools/GenerateEnum")]
    public static void CreateConsts(string enumName, IReadOnlyList<string> enumEntries)
    {
        string filePathAndName = "Assets/Resources/" + enumName + ".cs";

        using (StreamWriter streamWriter = new StreamWriter(filePathAndName))
        {
            streamWriter.WriteLine("public class " + enumName);
            streamWriter.WriteLine("{");
            try
            {
                for (int i = 0; i < enumEntries.Count; i++)
                {
                    string entry = enumEntries[i];
                    if (string.IsNullOrEmpty(entry))
                    {
                        continue;
                    }

                    string replaced = entry.Replace(' ', '_');
                    streamWriter.WriteLine($"\t public const string {replaced} = \"{entry}\";");
                }

                streamWriter.WriteLine("}");

            }
            catch
            {
                streamWriter.WriteLine("}");
            }
        }


        AssetDatabase.Refresh();
    }
}
#endif
