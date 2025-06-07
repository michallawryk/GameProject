using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // Przyk³adowe dane do zachowania:
    public bool[] LevelCompleted = new bool[6];

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public bool AreAllLevelsCompleted()
    {
        foreach (bool level in LevelCompleted)
        {
            if (!level)
                return false;
        }
        return true;
    }
}
