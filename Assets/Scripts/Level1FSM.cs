using MonsterLove.StateMachine;
using UnityEngine;

public class Level1FSM : MonoBehaviour
{
    private StateMachine<States, StateDriverUnity> fsm;
    public GameObject winScreen;
    public PauseMenu pauseMenuScript;
    public bool bossKilled = false;
    public GameObject postBossObjects;
    public enum States
    {
        Init,
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
        fsm.Driver.Update.Invoke();
    }

    public bool initGoal = false;
    void Init_Update()
    {
        if (bossKilled)
            fsm.ChangeState(States.WinState, StateTransition.Safe);
    }
    void WinState_Enter()
    {
        postBossObjects.SetActive(true);
        //winScreen.SetActive(true);
        //pauseMenuScript.Pause();
        //pauseMenuScript.pauseMenu.SetActive(false);
        //pauseMenuScript.enabled = false;
    }
}
