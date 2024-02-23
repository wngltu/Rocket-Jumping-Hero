using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level2GoalScript : MonoBehaviour
{
    Level2FSM goalObject;
    public enum GoalName
    {
        BrokenDoor,
    }
    public GoalName goal; //assign in inspector

    private void Start()
    {
        goalObject = FindObjectOfType<Level2FSM>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            switch (goal)
            {
                case GoalName.BrokenDoor:
                    goalObject.brokenDoor = true; break;
            }
        }
    }
}
