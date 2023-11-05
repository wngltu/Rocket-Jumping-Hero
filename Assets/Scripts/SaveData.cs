using UnityEngine;

public class SaveData : MonoBehaviour
{
    public GameObject playerDefaultSpawn;
    public static CheckpointScript currentCheckpoint;
    public static float checkpointX;
    public static float checkpointY;

    private void Awake()
    {
        if (checkpointX == 0 && checkpointY == 0)
        {
            checkpointX = playerDefaultSpawn.transform.position.x;
            checkpointY = playerDefaultSpawn.transform.position.y;
        }
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
