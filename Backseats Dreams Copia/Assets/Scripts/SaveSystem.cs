using UnityEngine;
using System.IO;

public static class SaveSystem
{
    // Ruta segura donde se guardará el archivo
    private static string path = Path.Combine(Application.persistentDataPath, "savefile.json");

    public static void Save(PlayerData data)
    {
        try
        {
            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(path, json);

            // Debug.Log($"Guardado en: {path}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error al guardar: {e.Message}");
        }
    }

    public static PlayerData Load()
    {
        if (!File.Exists(path))
        {
            // Si no existe archivo, devolvemos datos nuevos (0 monedas, etc)
            return new PlayerData();
        }

        try
        {
            string json = File.ReadAllText(path);
            return JsonUtility.FromJson<PlayerData>(json);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error al cargar: {e.Message}");
            return new PlayerData(); // Si falla, reiniciamos para que el juego no se rompa
        }
    }
    public static void DeleteSave()
    {
        if (File.Exists(path)) File.Delete(path);
    }
}