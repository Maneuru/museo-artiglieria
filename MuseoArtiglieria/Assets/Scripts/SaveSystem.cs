using System.IO;
using UnityEngine;

public static class SaveSystem
{
    private static string _basePath = $"{Application.persistentDataPath}/Saves";
    public static void SaveData<T>(T data, string key = "")
    {
        if (string.IsNullOrEmpty(key))
        {
            key = typeof(T).Name;
        }

        string jsonData = JsonUtility.ToJson(data);

        if (!Directory.Exists(_basePath))
        {
            Directory.CreateDirectory(_basePath);
        }

        File.WriteAllText($"{_basePath}/{key}.json", jsonData);
    }

    public static bool LoadData<T>(out T data, string key = "")
    {
        if (string.IsNullOrEmpty(key))
        {
            key = typeof(T).Name;
        }

        string filePath = $"{_basePath}/{key}.json";

        if (!File.Exists(filePath))
        {
            data = default;
            return false;
        }

        string jsonData = File.ReadAllText(filePath);
        try
        {
            data = JsonUtility.FromJson<T>(jsonData);
        }
        catch
        {
            data = default;
            return false;
        }

        return true;
    }
}
