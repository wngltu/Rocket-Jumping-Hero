using UnityEngine;
using System.Collections;
using MonsterLove.StateMachine;

public class ExampleBasic : MonoBehaviour
{
    //Declare which states we'd like use
    public enum States
    {
        Init,
        Test
    }

    private StateMachine<States, StateDriverUnity> fsm;

    private void Awake()
    {
        //Initialize State Machine Engine		
        fsm = new StateMachine<States, StateDriverUnity>(this);
        fsm.ChangeState(States.Init);
    }

    void Update()
    {
        fsm.Driver.Update.Invoke();
    }

    void Init_Enter()
    {
        Debug.Log("Waiting for start button to be pressed");
        fsm.ChangeState(States.Test);
    }

    void Test_Update()
    {
        Debug.Log("Test UPDATE");
    }
}