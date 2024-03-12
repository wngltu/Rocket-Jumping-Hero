using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TeleporterScript : MonoBehaviour
{
    public enum Levelname
    {
        Tutorial,
        Level1,
        Level2
    }

    public Levelname levelname;

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            switch (levelname)
            {
                case Levelname.Tutorial:
                    {
                        SaveData.WipeCheckpointProgress();
                        SceneManager.LoadScene("Level 1");
                        break;
                    }
                case Levelname.Level1:
                    {
                        SaveData.WipeCheckpointProgress();
                        SceneManager.LoadScene("Level 2");
                        break;
                    }
            }
        }
    }
}
