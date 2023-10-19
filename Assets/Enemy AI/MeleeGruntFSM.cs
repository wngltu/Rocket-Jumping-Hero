using UnityEngine;
using MonsterLove.StateMachine; //use _Enter, _Exit, _Finally for functions after the state enum
using System.Collections;

public class MeleeGruntFSM : MonoBehaviour
{
    public enum States
    {
        Init,
        Patrol,
        Attack,
        AttackCooldown,
        Chasing
    }
    StateMachine<States> fsm;
    // Start is called before the first frame update
    private void Awake()
    {
        fsm = new StateMachine<States>(this);

        fsm.ChangeState(States.Init);
    }

    // Update is called once per frame
    void Update()
    {
    }
    IEnumerator Init_Enter()
    {
        yield return new WaitForSeconds(1);
        Debug.Log("enter init state");
        fsm.ChangeState(States.Patrol);
    }
    IEnumerator Init_Exit()
    {
        yield return new WaitForSeconds(1);
        Debug.Log("exit init state");
    }

    IEnumerator Patrol_Enter()
    {
        yield return new WaitForSeconds(1);
        Debug.Log("enter patrol state");
        fsm.ChangeState(States.Init);
    }
}
