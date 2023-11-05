using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    CheckpointScript currentCheckpoint;

    public void SetNewCheckpoint(CheckpointScript newCheckpoint)
    {
        if (currentCheckpoint == null)
        {
            currentCheckpoint = newCheckpoint;
        }
        else
        {
            currentCheckpoint.GetComponent<CheckpointScript>().DeactivateCheckpoint();
            currentCheckpoint = newCheckpoint;
        }
    }
}
