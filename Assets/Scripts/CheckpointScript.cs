using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointScript : MonoBehaviour
{
    public GameObject activeModel;
    public GameObject inactiveModel;

    private void Start()
    {
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            ActivateCheckpoint();
        }
    }

    public void ActivateCheckpoint()
    {
        SaveData.checkpointX = this.transform.position.x;
        SaveData.checkpointY = this.transform.position.y;
        activeModel.SetActive(true);
        inactiveModel.SetActive(false);
    }

    public void DeactivateCheckpoint()
    {
        activeModel.SetActive(false);
        inactiveModel.SetActive(false);
    }
}
