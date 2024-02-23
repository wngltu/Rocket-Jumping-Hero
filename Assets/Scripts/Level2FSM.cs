using MonsterLove.StateMachine;
using UnityEngine;

public class Level2FSM : MonoBehaviour
{
    private StateMachine<States, StateDriverUnity> fsm;
    public GameObject winScreen;
    public PauseMenu pauseMenuScript;
    public bool bossKilled = false;
    public enum States
    {
        Init,
        ReachBrokenDoor,
        WinState
    }
    void Start()
    {
        pauseMenuScript = FindObjectOfType<PauseMenu>();
        fsm = new StateMachine<States, StateDriverUnity>(this);
        fsm.ChangeState(States.Init, StateTransition.Safe);
    }

    void Update()
    {
        if (bossKilled)
            fsm.ChangeState(States.WinState, StateTransition.Safe);
        fsm.Driver.Update.Invoke();
    }

    public bool brokenDoor = false;
    void Init_Update()
    {
        if (brokenDoor)
            fsm.ChangeState(States.ReachBrokenDoor, StateTransition.Safe);
    }

    public GameObject brokenDoorSequenceObj;
    void ReachBrokenDoor_Enter()
    {
        foreach (Transform child in brokenDoorSequenceObj.transform)
        {
            child.gameObject.SetActive(!child.gameObject.activeSelf);
        }
    }

    void WinState_Enter()
    {
        winScreen.SetActive(true);
        pauseMenuScript.Pause();
        pauseMenuScript.pauseMenu.SetActive(false);
        pauseMenuScript.enabled = false;
    }
}

