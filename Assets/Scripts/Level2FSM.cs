using MonsterLove.StateMachine;
using UnityEngine;

public class Level2FSM : MonoBehaviour
{
    private StateMachine<States, StateDriverUnity> fsm;
    public GameObject winScreen;
    public GameObject nonBrokenDoorObj;
    public GameObject brokenDoorObj;
    public PauseMenu pauseMenuScript;
    public CompleteGameTele teleScript;
    public int bossKilled = 0;
    public enum States
    {
        Init,
        ReachBrokenDoor,
        WinState
    }
    void Start()
    {
        pauseMenuScript = FindObjectOfType<PauseMenu>();
        teleScript = FindObjectOfType<CompleteGameTele>();
        teleScript.gameObject.SetActive(false);
        fsm = new StateMachine<States, StateDriverUnity>(this);
        fsm.ChangeState(States.Init, StateTransition.Safe);
    }

    void Update()
    {
        if (bossKilled >= 2)
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
        brokenDoorObj.SetActive(true);
        nonBrokenDoorObj.SetActive(false);
    }

    void WinState_Enter()
    {
        teleScript.gameObject.SetActive(true);
    }


    public void WinScreen()
    {
        winScreen.SetActive(true);
        pauseMenuScript.Pause();
        pauseMenuScript.pauseMenu.SetActive(false);
        pauseMenuScript.enabled = false;
    }
}

