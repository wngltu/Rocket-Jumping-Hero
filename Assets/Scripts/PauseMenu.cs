using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenu;
    public TMP_Text instructionsText;
    public TMP_Text instructionsButtonText;
    public GameObject pauseButton;

    public static PauseMenu instance;

    public bool paused = false;
    bool instructionsToggled = true;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) //open pause menu
        {
            CheckPause();
        }
    }

    public void CheckPause()
    {
        if (paused)
        {
            Unpause();
        }
        else if (!paused)
        {
            Pause();
        }
    }
    public void Pause() //pause game
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        pauseButton.GetComponent<PauseButtonScript>().ShowResumeIcon();
        paused = true;
    }
    public void Unpause() //resume game
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        pauseButton.GetComponent<PauseButtonScript>().ShowPauseIcon();
        paused = false;
    }

    public static void InstructionsPage() //instructions page on pause menu
    {

    }

    public void RestartLevel()
    {
        Unpause();
        SceneManager.LoadScene("Testing Level");
    }

    public static void RestartCheckpoint()
    {

    }
    public void ReturnToMainMenu() //return to main menu button on pause menu
    {
        Unpause();
        SceneManager.LoadScene("Main Menu");
    }

    public void ToggleInstructions()
    {
        instructionsToggled = !instructionsToggled;
        if (instructionsToggled)
        {
            instructionsText.gameObject.SetActive(true);
            instructionsButtonText.text = "Instructions: ON";
        }
        else if (!instructionsToggled)
        {
            instructionsText.gameObject.SetActive(false);
            instructionsButtonText.text = "Instructions: OFF";
        }
    }
}
