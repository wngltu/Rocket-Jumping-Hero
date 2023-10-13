

using UnityEngine;

public class SaveData : MonoBehaviour
{
    public static float checkpointX;
    public static float checkpointY;

    public static void WipeCheckpointProgress()
    {
        checkpointX = 0;
        checkpointY = 0;
    }
}
