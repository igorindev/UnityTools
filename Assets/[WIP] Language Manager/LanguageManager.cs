using LanguageController.Text;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace LanguageController
{
    [DisallowMultipleComponent]
    public class LanguageManager : MonoBehaviour
    {
        public static LanguageManager instance;

        TextDefiner[] inScene;
        [SerializeField] string filePath = "Assets/Language/LanguageFile.cvs";

        [SerializeField] [TextArea(5, 1000)] string fileContent;

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

            fileContent = Read();
        }

        #region Initialization
        //Read the file content and store insite the string
        [ContextMenu("Read")]
        void ReadContent()
        {
            fileContent = Read();
        }
        string Read()
        {
            if (File.Exists(filePath))
            {
                StreamReader fileReader = new StreamReader(filePath, Encoding.GetEncoding("iso-8859-1"));
                string file = fileReader.ReadToEnd();

                fileReader.Close();

                return file;
            }
            else
            {
                Debug.LogError("File not found");
                return "File not found or is empty.";
            }
        }
        #endregion

        [ContextMenu("Set Texts")]
        void ReadPreSavedFile()
        {
            if (fileContent != "")
            {
                languagesFoundInFile = GetLine(1).Split(splitValue);
                languagesFoundInFile[0] = "(Fake Position - Ignore)";

                //Set the text to all scene text component
                for (int i = 0; i < inScene.Length; i++)
                {
                    int id = i;
                    if (inScene[id].IndexType == TextIDType.IndexNumber)
                    {
                        inScene[i].TextComponent = GetContentAtIndex(inScene[id].TextNumIndex);
                    }
                    else
                    {
                        inScene[i].TextComponent = GetContentAtIndex(inScene[id].TextNameIndex);
                    }
                }
            }
            else
            {
                Debug.LogError("No pre saved file");
            }
        }
        public string GetContentAtIndex(int lineNo)
        {
            string[] lines = fileContent.Replace("\r", "").Split('\n');
            string temp = lines.Length >= lineNo ? lines[lineNo] : null;

            string result = temp.Split(splitValue)[SelectedLanguageIndex];

            if (result == "-")
            {
                Debug.LogWarning("The line " + lineNo + " of the colunm " + SelectedLanguageIndex + " is empty: -");
            }

            return result;
        }
        public string GetContentAtIndex(string textIdentifier)
        {
            string[] lines = fileContent.Replace("\r", "").Split('\n');

            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Split(splitValue)[0] == textIdentifier)
                {
                    string temp = lines[i].Split(splitValue)[SelectedLanguageIndex];

                    if (temp == "-")
                    {
                        Debug.LogWarning("The line " + i + " of the colunm " + SelectedLanguageIndex + " is empty: -");
                    }

                    return temp;
                }
            }

            Debug.LogError("This Line doesn't exist");
            return "";
        }
        string GetLine(int lineNo)
        {
            string[] lines = fileContent.Replace("\r", "").Split('\n');
            return lines.Length >= lineNo ? lines[lineNo - 1] : null;
        }

        [ContextMenu("Save-Create New File")]
        void SaveTextFile()
        {
            string path = filePath;
            while (File.Exists(path))
            {
                string temp = path.Split('.')[0];
                path = temp + "New.cvs";
            }

            FileStream stream = new FileStream(path, FileMode.CreateNew);

            StreamWriter fileWriter = new StreamWriter(stream, Encoding.GetEncoding("iso-8859-1"));

            fileWriter.Write(fileContent);

            fileWriter.Close();
        }

        public void SetSceneTexts(TextDefiner[] textDefinersInScene)
        {
            inScene = textDefinersInScene;

            ReadPreSavedFile();
        }
    }
}
