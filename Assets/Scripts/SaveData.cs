using UnityEngine;

public class SaveData : MonoBehaviour
{
    public GameObject playerDefaultSpawn;
    public static CheckpointScript currentCheckpoint;
    public static float checkpointX;
    public static float checkpointY;
    public static float defaultX;
    public static float defaultY;

    private void Awake()
    {
        if (checkpointX == 0 && checkpointY == 0)
        {
            checkpointX = playerDefaultSpawn.transform.position.x;
            checkpointY = playerDefaultSpawn.transform.position.y;
        }
    }

    private void Start()
    {
        defaultX = FindObjectOfType<PlayerSpawnVisual>().gameObject.transform.position.x;
        defaultY = FindObjectOfType<PlayerSpawnVisual>().gameObject.transform.position.y;
    }

    public static void WipeCheckpointProgress()
    {
        checkpointX = defaultX;
        checkpointY = defaultY;
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
