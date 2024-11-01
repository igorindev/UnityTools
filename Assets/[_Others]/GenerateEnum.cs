#if UNITY_EDITOR
using System.IO;
using UnityEditor;

public class GenerateEnum
{
    [MenuItem("Tools/GenerateEnum")]
    public static void Go()
    {
        string enumName = "EnumTest";
        var enumEntries = new[]{ "a", "b", "c" };
        string filePathAndName = "Assets/Resources/" + enumName + ".cs"; //The folder Scripts/Enums/ is expected to exist

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

                streamWriter.WriteLine($"\t{entry} = (1 << {i}),");
            }

            streamWriter.WriteLine("}");
        }

        AssetDatabase.Refresh();
    } 
}
#endif