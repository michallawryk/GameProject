using UnityEngine;

public class LevelEndLever : LeverInteractable
{
    [Header("Numer poziomu (0-5)")]
    [SerializeField ]private int levelIndex = 0;

    protected override void Awake()
    {
        base.Awake();
        if (GameManager.Instance != null && levelIndex >= 0 && levelIndex < GameManager.Instance.LevelCompleted.Length)
        {
            isOn = GameManager.Instance.LevelCompleted[levelIndex];               // Ustaw stan dŸwigni
            Animate();
        }
    }

    public override void Interact(VoltController player)
    {
        base.Interact(null);
        
        // Ustaw w GameManagerze LevelCompleted[levelIndex] = true
        if (GameManager.Instance != null && levelIndex >= 0 && levelIndex < GameManager.Instance.LevelCompleted.Length)
        {
            GameManager.Instance.LevelCompleted[levelIndex] = isOn;
            Debug.Log($"Poziom {levelIndex} ukoñczony!");
        }
    }
}
