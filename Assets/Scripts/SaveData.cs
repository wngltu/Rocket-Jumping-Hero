using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveData : MonoBehaviour
{
    public GameObject playerDefaultSpawn;
    public static CheckpointScript currentCheckpoint;
    public static float checkpointX;
    public static float checkpointY;
    public static float defaultX;
    public static float defaultY;
    public static float savedLvl;
    public static float currentLvl;

    private void Awake()
    {
        if (checkpointX == 0 && checkpointY == 0)
        {
            checkpointX = playerDefaultSpawn.transform.position.x;
            checkpointY = playerDefaultSpawn.transform.position.y;
        }
        else if (currentLvl != savedLvl)
        {
            checkpointX = playerDefaultSpawn.transform.position.x;
            checkpointY = playerDefaultSpawn.transform.position.y;

        }
        defaultX = FindObjectOfType<PlayerSpawnVisual>().gameObject.transform.position.x;
        defaultY = FindObjectOfType<PlayerSpawnVisual>().gameObject.transform.position.y;
    }

    private void Start()
    {
        if (checkpointX == 0 && checkpointY == 0)
        {
            checkpointX = playerDefaultSpawn.transform.position.x;
            checkpointY = playerDefaultSpawn.transform.position.y;
        }
        defaultX = FindObjectOfType<PlayerSpawnVisual>().gameObject.transform.position.x;
        defaultY = FindObjectOfType<PlayerSpawnVisual>().gameObject.transform.position.y;
    }

    public static void WipeCheckpointProgress()
    {
        checkpointX = 0;
        checkpointY = 0;
    }

    public static void SaveCheckpoint(CheckpointScript checkpoint)
    {
        currentCheckpoint = checkpoint;
    }

    public static void DeactivateCheckpoint()
    {
        currentCheckpoint.gameObject.GetComponent<CheckpointScript>().DeactivateCheckpoint();
    }
}
