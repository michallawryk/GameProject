[System.Serializable]
public class GameData
{
    public string version;
    public bool[] levelsCompleted;
    public int currentSceneIndex;

    public GameData(string version = "1.0")
    {
        this.version = version;
    }
}
