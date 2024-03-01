using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialGoalScript : MonoBehaviour
{
    TutorialFSM tutorialObject;
    public enum GoalName
    {
        WalkingGoal,
        SingleJumpingGoal,
        DoubleJumpingGoal,
        SprintJumpingGoal,
        RJVerticalGoal,
        RJHorizontalGoal,
        PlatformGoal,
        PressurePlateGoal,
        PressurePlate,
        ShootPressurePlate,
        DoubleRJ,
        Lever,

        Lv1Room1,
    }
    public GoalName goal; //assign in inspector

    private void Start()
    {
        tutorialObject = FindObjectOfType<TutorialFSM>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            switch (goal)
            {
                case GoalName.WalkingGoal:
                    tutorialObject.walkingGoal = true; break;
                case GoalName.SingleJumpingGoal:
                    tutorialObject.jumpingGoal = true; break;
                case GoalName.DoubleJumpingGoal:
                    tutorialObject.doubleJumpingGoal = true; break;
                case GoalName.SprintJumpingGoal:
                    tutorialObject.sprintingGoal = true; break;
                case GoalName.RJVerticalGoal:
                    tutorialObject.rjGoal1 = true; break;
                case GoalName.RJHorizontalGoal:
                    tutorialObject.rjGoal2 = true; break;
                case GoalName.PressurePlateGoal:
                    tutorialObject.pressurePlateGoal = true; break;
                case GoalName.PlatformGoal:
                    tutorialObject.platformGoal = true; break;
                case GoalName.ShootPressurePlate:
                    tutorialObject.shootPressurePlateGoal = true; break;
                case GoalName.DoubleRJ:
                    tutorialObject.doubleRJGoal = true; break;
                case GoalName.Lever:
                    tutorialObject.leverGoal = true; break;
            }
        }
    }
}