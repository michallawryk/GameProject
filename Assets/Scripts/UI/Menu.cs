using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public void OnNewGameButton()
    {
        DataHandler.Instance.NewGame();
        SceneManager.LoadSceneAsync("Lobby");
    }

    public void OnContinueButton()
    {
        SceneManager.LoadSceneAsync(DataHandler.Instance.GameData.currentSceneIndex - 1);
    }

    public void OnControlButton()
    {

    }

    public void OnExitButton()
    {
        Application.Quit();
    }
}
