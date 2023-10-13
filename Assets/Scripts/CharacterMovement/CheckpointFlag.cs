using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointFlag : MonoBehaviour
{
    public CheckpointScript checkpointMaster;

    private void Start()
    {
        checkpointMaster = FindObjectOfType<CheckpointScript>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
        }
    }
}
