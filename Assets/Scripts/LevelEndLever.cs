using UnityEngine;

public class LevelEndLever : LeverInteractable
{
    [Header("Numer poziomu (0-5)")]
    [SerializeField ]private int levelIndex = 0;

    public override void Interact()
    {
        Debug.Log("Uko�czony!");
        base.Interact();
        
        // Ustaw w GameManagerze LevelCompleted[levelIndex] = true
        if (GameManager.Instance != null && levelIndex >= 0 && levelIndex < GameManager.Instance.LevelCompleted.Length)
        {
            GameManager.Instance.LevelCompleted[levelIndex] = isOn;
            Debug.Log($"Poziom {levelIndex} uko�czony!");
        }
    }

    private void Start()
    {
        if (GameManager.Instance != null && levelIndex >= 0 && levelIndex < GameManager.Instance.LevelCompleted.Length)
        {
            bool isCompleted = GameManager.Instance.LevelCompleted[levelIndex];
            // Ustaw stan d�wigni
            SetLeverState(isCompleted);
        }
    }

    // Pomocnicza metoda do ustawiania stanu d�wigni (np. sprite/animacja)
    public void SetLeverState(bool on)
    {
        isOn = on;

        Animate();
    }
}
