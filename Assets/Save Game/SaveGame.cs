using System;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

public class SaveGame : MonoBehaviour
{
    public static SaveGame instance;
    const string CHAVE = "facesplanadoreleitoingredientes"; //Chave de 31 letras

    private Save save;
    private OptionsSave optionsSave;
    public Save SaveState { get => save; }
    public OptionsSave OptionsSaveFile { get => optionsSave; }

    public bool PlayedTutorial = false;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.Log("Ja existe um SaveGame manager");
            return;
        }
        instance = this;

        save = new Save();
        optionsSave = new OptionsSave();
    }

    //Player
    public void Save()
    {
        StreamWriter fileWriter = new StreamWriter("Savegame.xml");
        XmlSerializer obj = new XmlSerializer(typeof(Save));

        //Saves

        Debug.Log("Jogo Salvo");

        obj.Serialize(fileWriter.BaseStream, save);
        fileWriter.Close();

        StreamReader fileReader = new StreamReader("Savegame.xml");
        string text = fileReader.ReadToEnd();
        fileReader.Close();

        EncryptFile("Savegame.xml", text);
    }
    public void Load(string fileName = "Savegame.xml")
    {
        if (File.Exists(fileName))
        {
            DecryptFile(fileName);
            StreamReader fileReader = new StreamReader(fileName);
            XmlSerializer obj = new XmlSerializer(typeof(Save));
            string text = fileReader.ReadToEnd();
            fileReader.Close();

            fileReader = new StreamReader(fileName);

            save = obj.Deserialize(fileReader.BaseStream) as Save;

            PlayedTutorial = save.PlayedTutorial;

            //Saves

            fileReader.Close();
            EncryptFile(fileName, text);
        }
    }
    public void LoadMenu(out Save saveFile, string fileName = "Savegame.xml")
    {
        if (File.Exists(fileName))
        {
            DecryptFile(fileName);
            StreamReader fileReader = new StreamReader(fileName);
            XmlSerializer obj = new XmlSerializer(typeof(Save));
            string text = fileReader.ReadToEnd();
            fileReader.Close();

            fileReader = new StreamReader(fileName);

            save = obj.Deserialize(fileReader.BaseStream) as Save;

            saveFile = save;

            fileReader.Close();
            EncryptFile(fileName, text);
        }
        else
        {
            saveFile = null;
        }
    }
    public bool FileExists(string filename = "Savegame.xml")
    {
        if (File.Exists(filename))
        {
            return true;
        }
        return false;
    }
    public void DeleteSave(string filename = "Savegame.xml")
    {
        if (File.Exists(filename))
        {
            File.Delete(filename);
        }
        PlayedTutorial = false;
    }

    //Options
    public void SaveOptions(string fileName = "Configs.xml")
    {
        StreamWriter fileWriter = new StreamWriter(fileName);
        XmlSerializer obj = new XmlSerializer(typeof(OptionsSave));

        //Saves

        Debug.Log("Jogo Salvo");

        obj.Serialize(fileWriter.BaseStream, optionsSave);
        fileWriter.Close();

        StreamReader fileReader = new StreamReader(fileName);
        string text = fileReader.ReadToEnd();
        fileReader.Close();

        EncryptFile(fileName, text);
    }
    public void LoadOptions(string fileName = "Configs.xml")
    {
        if (File.Exists(fileName))
        {
            DecryptFile(fileName);

            StreamReader fileReader = new StreamReader(fileName);
            XmlSerializer obj = new XmlSerializer(typeof(OptionsSave));

            string text = fileReader.ReadToEnd();
            fileReader.Close();

            fileReader = new StreamReader(fileName);

            optionsSave = obj.Deserialize(fileReader.BaseStream) as OptionsSave;

            //Loads

            fileReader.Close();
            EncryptFile(fileName, text);
        }
        else
        {
            SaveOptions();
        }
    }


    private void EncryptFile(string fileName, string text)
    {
        StreamWriter fileWriter = new StreamWriter(fileName);

        for (int i = 0; i < text.Length; i++)
        {
            int posicao = i % CHAVE.Length; //Cada letra em uma determinada sequência é somada com a chave naquele index CIFRA DE VIGENERE;
            char newLetter = (char)(text[i] + CHAVE[posicao]);
            fileWriter.Write(newLetter);

        }

        fileWriter.Close();
    }
    private void DecryptFile(string fileName)
    {
        StreamReader fileReader = new StreamReader(fileName);
        string text = fileReader.ReadToEnd();
        fileReader.Close();

        StreamWriter fileWriter = new StreamWriter(fileName);

        for (int i = 0; i < text.Length; i++)
        {
            int posicao = i % CHAVE.Length;
            char newLetter = (char)(text[i] - CHAVE[posicao]);
            fileWriter.Write(newLetter);
        }

        fileWriter.Close();
    }
}
