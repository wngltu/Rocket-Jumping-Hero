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
        SceneManager.LoadScene("Level 1");
    }

    public static void StartNewGame()
    {
        SaveData.WipeCheckpointProgress();
        SaveData.currentCheckpoint = null;
        SceneManager.LoadScene("Level 1");
    }

    public static void StartTutorial()
    {
        SceneManager.LoadScene("Tutorial Level");
    }

    public static void StartTestingLevel()
    {
        SceneManager.LoadScene("Testing Level");
    }

    public static void StartLevel2()
    {
        SceneManager.LoadScene("Level 2");
    }

    public static void StartCredits()
    {
        SceneManager.LoadScene("Credits");
    }

    public static void StartMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }
}
