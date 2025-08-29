using UnityEngine;

public class LevelEndLever : LeverInteractable
{
    [Header("Numer poziomu (0-5)")]
    [SerializeField ]private int levelIndex = 0;

    protected override void Awake()
    {
        base.Awake();
        if (DataHandler.Instance != null && levelIndex >= 0 && levelIndex < DataHandler.Instance.GameData.levelsCompleted.Length)
        {
            isOn = DataHandler.Instance.GameData.levelsCompleted[levelIndex];               // Ustaw stan dŸwigni
            Animate();
        }
    }

    public override void Interact(VoltController player)
    {
        base.Interact(null);

        DataHandler.Instance.MarkCurrentLevelCompletedAndSave();
    }
}
