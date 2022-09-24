using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Localization
{
    [DisallowMultipleComponent]
    public class LocalizationManager : MonoBehaviour
    {
        public static LocalizationManager instance;

        [SerializeField] SOLocalizationConfig localizationConfig;

        [TextArea(5, 1000)] string fileContent;

        [Header("Languages")]
        string[] languagesFoundInFile;
        [SerializeField] [Min(0)] int selectedLanguageIndex = 0;
        
        [Header("Localizations")]
        List<Localization> localization = new List<Localization>();
        public List<Localization> Localization { get => localization; private set => localization = value; }

        public int SelectedLanguageIndex
        {
            get
            {
                if (selectedLanguageIndex > LanguagesFoundInFile.Length || selectedLanguageIndex < 0)
                {
                    Debug.LogError("ERROR: trying to get a Language that is out of index, reseting to language 1");
                    selectedLanguageIndex = 0;
                }
                return selectedLanguageIndex;
            }

            set
            {
                if (selectedLanguageIndex == value) { return; }

                if (selectedLanguageIndex > LanguagesFoundInFile.Length || selectedLanguageIndex < 0)
                {
                    Debug.LogError("ERROR: trying to set a Language that is out of index, reseting to language 1");
                    selectedLanguageIndex = 0;
                }
                else
                {
                    selectedLanguageIndex = value;
                }

                UpdateAllLocalizations();
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

        const char splitValue = ';';

        void Awake()
        {
            if (instance != null)
            {
                Debug.Log("Já há um Language Manager declarado");
                Destroy(instance.gameObject);
                return;
            }

            instance = this;

            ReadContent();
        }

        [ContextMenu("Update All")]
        public void UpdateAllLocalizations()
        {
            for (int i = 0; i < Localization.Count; i++)
            {
                Localization[i].UpdateLocalization();
            }
        }
        public void AddLocalization(Localization localization, Action initialize)
        {
            Localization.Add(localization);
            initialize.Invoke();
            localization.UpdateLocalization();
        }

        [ContextMenu("Read")] //Read the file content and store inside the string
        void ReadContent()
        {
            fileContent = Read(localizationConfig.filePath);
            if (fileContent != "")
            {
                languagesFoundInFile = GetLine(fileContent, 1).Split(splitValue);
                languagesFoundInFile[0] = "(Fake Position - Ignore)";
            }
        }
        static string Read(string filePath)
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

        public string GetContentAtIndex(int lineNo, GameObject caller = null)
        {
            string[] lines = fileContent.Replace("\r", "").Split('\n');
            string temp = lines.Length - 1 > lineNo ? lines[lineNo] : "Error";

            if (temp == "Error")
            {
                Debug.LogError("LocalizationManager: The line " + lineNo + " of the colunm " + (SelectedLanguageIndex + 1) + " is out of range. " + (!ReferenceEquals(caller, null) ? ("Called by: " + caller.name) : ""), caller);
                return temp;
            }

            string result = temp.Split(splitValue)[SelectedLanguageIndex + 1];

            if (result == "-")
            {
                Debug.LogWarning("LocalizationManager: The line " + lineNo + " of the colunm " + (SelectedLanguageIndex + 1) + " is empty: -");
            }

            return result;
        }
        public string GetContentAtIndex(string textIdentifier, GameObject caller = null)
        {
            string[] lines = fileContent.Replace("\r", "").Split('\n');

            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Split(splitValue)[0] == textIdentifier)
                {
                    string temp = lines[i].Split(splitValue)[SelectedLanguageIndex + 1];

                    if (temp == "-")
                    {
                        Debug.LogWarning("LocalizationManager: The line " + i + " of the colunm " + (SelectedLanguageIndex + 1) + " is empty: -");
                    }

                    return temp;
                }
            }

            Debug.LogError("LocalizationManager: The context <b><color=red>" + textIdentifier + "</color></b> of the colunm " + (SelectedLanguageIndex + 1) + " do not exists. " + (!ReferenceEquals(caller, null) ? ("Called by: " + caller.name) : ""), caller);
            return "Error";
        }
        static string GetLine(string fileContent, int lineNo)
        {
            string[] lines = fileContent.Replace("\r", "").Split('\n');
            return lines.Length >= lineNo ? lines[lineNo - 1] : null;
        }

        public static string[] GetLocalization()
        {
            string fileContent = Read(SOLocalizationConfig.editorFilePath);

            if (string.IsNullOrEmpty(fileContent))
            {
                return new string[] { "Empty" };
            }

            string[] languagesFoundInFile = new string[0];
            if (fileContent != "")
            {
                languagesFoundInFile = GetLine(fileContent, 1).Split(splitValue);
                languagesFoundInFile[0] = "(Fake Position - Ignore)";
            }

            List<string> temp = languagesFoundInFile.ToList();
            temp.RemoveAt(0);
            return temp.ToArray();
        }
        public static string[] GetContexts()
        {
            string fileContent = Read(SOLocalizationConfig.editorFilePath);
            if (fileContent == "File not found or is empty.")
            {
                return new string[] { };
            }

            string[] content = fileContent.Replace("\r", "").Split('\n');
            List<string> lines = new List<string>();
            for (int i = 0; i < content.Length; i++)
            {
                if (content[i].Contains(';') == false || content[i].Split(';')[0] == "" || content[i].Split(';')[0] == "-") { continue; }
                
                lines.Add(i + " - " + content[i].Split(';')[0]);
            }
            lines.RemoveAt(0);
            return lines.ToArray();
        }
    }
}
