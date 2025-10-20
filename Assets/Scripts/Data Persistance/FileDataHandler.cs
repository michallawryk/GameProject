using System.IO;
using System.Text;
using UnityEngine;

public class FileDataHandler
{
    private readonly string _path;

    public bool Exists() => File.Exists(_path);

    public FileDataHandler(string filename = "data.json")
    {
        _path = Path.Combine(Application.persistentDataPath, filename);
    }

    public void Save(GameData data)
    {
        var json = JsonUtility.ToJson(data, prettyPrint: true);
        File.WriteAllText(_path, json, Encoding.UTF8);
    }

    public bool TryLoad(out GameData data)
    {
        if (!File.Exists(_path)) { data = null; return false; }
        var json = File.ReadAllText(_path, Encoding.UTF8);
        data = JsonUtility.FromJson<GameData>(json);
        return data != null;
    }
}