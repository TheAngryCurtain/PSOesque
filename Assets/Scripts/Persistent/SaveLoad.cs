using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public static class SaveLoad
{
    private const string m_CharacterPath = "/Character.data";
    private const string m_GamePath = "/Game.data";

    private static BinaryFormatter m_Formatter;

    public static void Init()
    {
        if (m_Formatter == null)
        {
            m_Formatter = new BinaryFormatter();
        }
    }

    public static void SaveCharacterProgress(CharacterProgress progress)
    {
        FileStream file = File.Create(Application.persistentDataPath + m_CharacterPath);
        m_Formatter.Serialize(file, progress);
        file.Close();
    }

    public static void SaveGameProgress(GameProgress progress)
    {
        FileStream file = File.Create(Application.persistentDataPath + m_GamePath);
        m_Formatter.Serialize(file, progress);
        file.Close();
    }

    public static CharacterProgress LoadCharacterProgress()
    {
        if (File.Exists(Application.persistentDataPath + m_CharacterPath))
        {
            FileStream file = File.Open(Application.persistentDataPath + m_CharacterPath, FileMode.Open);
            CharacterProgress progress = (CharacterProgress)m_Formatter.Deserialize(file);
            file.Close();

            return progress;
        }
        else
        {
            Debug.LogErrorFormat("No File exists at {0}", Application.persistentDataPath + m_CharacterPath);
            return null;
        }
    }

    public static GameProgress LoadGameProgress()
    {
        if (File.Exists(Application.persistentDataPath + m_GamePath))
        {
            FileStream file = File.Open(Application.persistentDataPath + m_GamePath, FileMode.Open);
            GameProgress progress = (GameProgress)m_Formatter.Deserialize(file);
            file.Close();

            return progress;
        }
        else
        {
            Debug.LogErrorFormat("No File exists at {0}", Application.persistentDataPath + m_GamePath);
            return null;
        }
    }
}
