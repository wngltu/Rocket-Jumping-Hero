using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseButtonScript : MonoBehaviour
{
    public GameObject pauseIcon;
    public GameObject resumeIcon;

    public void ShowResumeIcon()
    {
        pauseIcon.SetActive(false);
        resumeIcon.SetActive(true);
    }

    public void ShowPauseIcon()
    {
        resumeIcon.SetActive(false);
        pauseIcon.SetActive(true);
    }

}
