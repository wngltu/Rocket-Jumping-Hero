using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public static void ExitGame()
    {
        Application.Quit();
    }
    public static void StartGame()
    {
        SceneManager.LoadScene("Testing Level");
    }

    public static void StartNewGame()
    {
        SaveData.WipeCheckpointProgress();
        SaveData.currentCheckpoint = null;
        SceneManager.LoadScene("Testing Level");
    }

    public static void StartTutorial()
    {
        SceneManager.LoadScene("Tutorial Level");
    }
}
