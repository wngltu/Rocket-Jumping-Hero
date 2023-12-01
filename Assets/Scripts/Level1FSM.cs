using MonsterLove.StateMachine;
using UnityEngine;

public class Level1FSM : MonoBehaviour
{
    private StateMachine<States, StateDriverUnity> fsm;
    public enum States
    {
        Init,
        Enemy1,
    }
    void Start()
    {
        fsm = new StateMachine<States, StateDriverUnity>(this);
        fsm.ChangeState(States.Init, StateTransition.Safe);
    }

    void Update()
    {
        fsm.Driver.Update.Invoke();
    }

    public bool initGoal = false;
    void Init_Update()
    {
        if (initGoal)
            fsm.ChangeState(States.Enemy1, StateTransition.Safe);
    }
    void Enemy1_Enter()
    {

    }
}
