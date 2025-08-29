using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DataHandler : MonoBehaviour
{
    public static DataHandler Instance { get; private set; }

    [Header("Plik zapisu")]
    [SerializeField] private string filename = "data.json";

    [Header("Zakres scen-poziomów (build indicates)")]
    [Tooltip("Pierwszy index poziomu")]
    [SerializeField] private int firstLevelIndex = 1;

    [Tooltip("Jeœli < 0, u¿yje automatycznie (sceneCountInBuildSettings-1).")]
    [SerializeField] private int lastLevelIndexInclusive = -1;

    private FileDataHandler _handler;
    [SerializeField] private GameData _gameData;

    public GameData GameData => _gameData;
    public bool HasSave => _handler != null && _handler.Exists();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        _handler = new FileDataHandler(filename);
        if (!_handler.TryLoad(out _gameData))
        {
            _gameData = new GameData();
        }

        if (lastLevelIndexInclusive < 0)
            lastLevelIndexInclusive = SceneManager.sceneCountInBuildSettings - 1;

        EnsureLevelsArraySized();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnApplicationQuit()
    {
        SaveNow();
    }

    private void EnsureLevelsArraySized()
    {
        int count = GetLevelCount();
        if (count <= 0) return;

        if (_gameData.levelsCompleted == null || _gameData.levelsCompleted.Length != count)
        {
            var newArr = new bool[count];
            if (_gameData.levelsCompleted != null)
            {
                Array.Copy(_gameData.levelsCompleted, newArr, Math.Min(_gameData.levelsCompleted.Length, count));
            }
            _gameData.levelsCompleted = newArr;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        int index = scene.buildIndex;
        if (IsLevelBuildIndex(index))
        {
            _gameData.currentSceneIndex = index;
            SaveNow(); // opcjonalnie zapisuj od razu po wejœciu na poziom
        }
    }

    private int GetLevelCount()
    {
        return Mathf.Max(0, lastLevelIndexInclusive - firstLevelIndex + 1);
    }

    private bool IsLevelBuildIndex(int buildIndex)
    {
        return buildIndex >= firstLevelIndex && buildIndex <= lastLevelIndexInclusive;
    }

    private int BuildIndexToLevelArrayIndex(int buildIndex)
    {
        return buildIndex - firstLevelIndex;
    }

    public void MarkCurrentLevelCompletedAndSave()
    {
        int buildIndex = SceneManager.GetActiveScene().buildIndex;
        MarkLevelCompletedByBuildIndex(buildIndex);
    }

    public void MarkLevelCompletedByBuildIndex(int buildIndex)
    {
        if (!IsLevelBuildIndex(buildIndex)) return;
        int idx = BuildIndexToLevelArrayIndex(buildIndex);
        EnsureLevelsArraySized();
        _gameData.levelsCompleted[idx] = true;
        SaveNow();
    }

    public bool IsLevelCompletedByBuildIndex(int buildIndex)
    {
        if (!IsLevelBuildIndex(buildIndex) || _gameData.levelsCompleted == null) return false;
        int idx = BuildIndexToLevelArrayIndex(buildIndex);
        return idx >= 0 && idx < _gameData.levelsCompleted.Length && _gameData.levelsCompleted[idx];
    }

    public void NewGame()
    {
        _gameData = new GameData();
        EnsureLevelsArraySized();
        SaveNow();
    }

    public void SaveNow()
    {
        _handler.Save(_gameData);
    }

    public bool AreAllLevelsCompleted()
    {
        if (_gameData.levelsCompleted == null) return false;
        foreach (bool level in _gameData.levelsCompleted)
        {
            if (!level)
                return false;
        }
        return true;
    }
}
