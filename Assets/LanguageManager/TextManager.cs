using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

public class TextManager : MonoBehaviour
{
    public static TextManager instance;

    [SerializeField] TextDefiner[] inScene;
    [SerializeField] string filePath = "Assets/LanguageManager/LanguageFile.cvs";

    [Header("Debug")]
    [SerializeField] string[] languagesFoundInFile;
    [SerializeField] [Min(1)] int selectedLanguageIndex = 1;

    const char splitValue = '~';

    public int SelectedLanguageIndex
    {
        get
        {
            if (selectedLanguageIndex > LanguagesFoundInFile.Length || selectedLanguageIndex < 1)
            {
                Debug.LogError("ERROR: trying to get a Language that is out of index, reseting to language 1");
                selectedLanguageIndex = 1;
            }
            return selectedLanguageIndex;
        }

        set
        {
            if (selectedLanguageIndex > LanguagesFoundInFile.Length || selectedLanguageIndex < 1)
            {
                Debug.LogError("ERROR: trying to set a Language that is out of index, reseting to language 1");
                selectedLanguageIndex = 1;
            }
            else
            {
                selectedLanguageIndex = value;
            }
        }
    }

    public string[] LanguagesFoundInFile 
    {
        get 
        {
            List<string> l = languagesFoundInFile.ToList();
            l.Remove(languagesFoundInFile[0]);
            return l.ToArray();
        } 
        set => languagesFoundInFile = value; 
    }

    void Awake()
    {
        if (instance != null)
        {
            Debug.Log("Já há um Language Manager declarado");
            Destroy(instance.gameObject);
            return;
        }

        instance = this;

        ReadTextFile();
    }

    [ContextMenu("Read")]
    void ReadTextFile()
    {
        if (File.Exists(filePath))
        {
            StreamReader fileReader = new StreamReader(filePath, Encoding.GetEncoding("iso-8859-1"));

            LanguagesFoundInFile = fileReader.ReadLine().Split(splitValue);
            LanguagesFoundInFile[0] = "(Fake Position - Ignore)";

            //Set the text to all scene text component
            for (int i = 0; i < inScene.Length; i++)
            {
                int id = i;
                inScene[i].Text = ReadText(inScene[id].TextIndex, fileReader);
            }

            fileReader.Close();
        }
        else
        {
            Debug.LogError("File not found");
        }
    }

    public string ReadText(int index, StreamReader file)
    {
        file.DiscardBufferedData();
        file.BaseStream.Seek(0, SeekOrigin.Begin);
        file.ReadLine();

        int id = 1;
        string temp = "-";
        while (id < index)
        {
            temp = file.ReadLine();
            id++;
        }

        temp = temp.Split(splitValue)[SelectedLanguageIndex];

        if (temp == "-")
        {
            Debug.LogError("This line is empty. Please check the Row " + id + " at Column " + (SelectedLanguageIndex + 1));
        }
        return temp;
    }

    public string ReadText(int index)
    {
        StreamReader file = new StreamReader(filePath, Encoding.GetEncoding("iso-8859-1"));
        file.DiscardBufferedData();
        file.BaseStream.Seek(0, SeekOrigin.Begin);
        file.ReadLine();

        int id = 1;
        string temp = "-";
        while (id < index)
        {
            temp = file.ReadLine();
            id++;
        }

        temp = temp.Split(splitValue)[SelectedLanguageIndex];

        if (temp == "-")
        {
            Debug.LogError("This line is empty. Please check the Row " + id + " at Column " + (SelectedLanguageIndex + 1));
        }
        file.Close();
        return temp;
    }
}
