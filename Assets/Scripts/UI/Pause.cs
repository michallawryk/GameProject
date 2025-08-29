using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public static bool IsPaused { get; private set; }

    [Header("UI")]
    [SerializeField] private GameObject pauseMenu;

    [Header("Input")]
    [SerializeField] private InputActionReference pauseAction;

    private string _prevActionMap;

    private void Awake()
    {
        ResumeImmediate();
    }

    private void OnEnable()
    {
        if (pauseAction != null)
        {
            pauseAction.action.started += OnPausePerformed;
            pauseAction.action.Enable();
        }
    }

    private void OnDisable()
    {
        if (pauseAction != null)
        {
            pauseAction.action.started -= OnPausePerformed;
            pauseAction.action.Disable();
        }
    }

    private void OnPausePerformed(InputAction.CallbackContext ctx)
    {
        TogglePause();
    }

    public void TogglePause()
    {
        if (IsPaused) Resume();
        else Pause();
    }

    public void Pause()
    {
        if (IsPaused) return;
        IsPaused = true;

        Time.timeScale = 0f;                          

        if (pauseMenu != null) pauseMenu.SetActive(true);    // poka¿ UI
    }

    public void Resume()
    {
        if (!IsPaused) return;
        ResumeImmediate();
    }

    private void ResumeImmediate()
    {
        IsPaused = false;
        Time.timeScale = 1f;

        if (pauseMenu != null) pauseMenu.SetActive(false);
    }

    // --- Handlery pod przyciski w menu pauzy ---
    public void OnResumeButton() => Resume();

    public void OnRestartButton()
    {
        ResumeImmediate();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void OnMainMenuButton()
    {
        ResumeImmediate();
        SceneManager.LoadScene("Main Menu");
    }
}
