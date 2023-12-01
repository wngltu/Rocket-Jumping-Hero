using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level1GoalScript : MonoBehaviour
{
    Level1FSM goalObject;
    public enum GoalName
    {
        Room1,
    }
    public GoalName goal; //assign in inspector

    private void Start()
    {
        goalObject = FindObjectOfType<Level1FSM>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            switch (goal)
            {
                case GoalName.Room1:
                    goalObject.initGoal = true; break;
            }
        }
    }
}

