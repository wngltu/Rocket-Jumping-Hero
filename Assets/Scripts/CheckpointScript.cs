using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointScript : MonoBehaviour
{
    public GameObject activeModel;
    public GameObject inactiveModel;
    public SaveData saveData;
    public CheckpointManager checkpointManager;

    bool activated = false;

    private void Start()
    {
        saveData = FindObjectOfType<SaveData>();
        checkpointManager = FindObjectOfType<CheckpointManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && activated == false)
        {
            ActivateCheckpoint();
            checkpointManager.SetNewCheckpoint(this.gameObject.GetComponent<CheckpointScript>());
        }
    }

    public void ActivateCheckpoint()
    {
        activated = true;
        SaveData.checkpointX = this.transform.position.x;
        SaveData.checkpointY = this.transform.position.y;
        activeModel.SetActive(true);
        inactiveModel.SetActive(false);
    }

    public void DeactivateCheckpoint()
    {
        activated = false;
        activeModel.SetActive(false);
        inactiveModel.SetActive(true);
    }
}
