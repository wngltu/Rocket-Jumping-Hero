using System.Collections;
using UnityEngine;
using MonsterLove.StateMachine;
using TMPro;


public class TutorialFSM : MonoBehaviour
{
    public GameObject tutorialTextObject;
    public TextMeshProUGUI tutorialText;
    public States currentState;
    public float timer;
    private StateMachine<States, StateDriverUnity> fsm;
    bool bool1 = false; //bool variables used for input detection
    bool bool2 = false; //bool variables used for input detection
    public enum States
    {
        Walking,
        Sprinting,
        Jumping
    }

    private void Start()
    {
        fsm = new StateMachine<States, StateDriverUnity>(this);
        fsm.ChangeState(States.Walking, StateTransition.Safe);
    }
    private void Update()
    {
        fsm.Driver.Update.Invoke();
        if (timer > 0)
        {
            timer -= Time.deltaTime;
            StartPulsing();
        }
        else if (timer < 0)
        {
            timer = 0;
            StopPulsing();
        }
    }
    void Walking_Enter()
    {
        bool1 = false;
        bool2 = false;
        tutorialText.text = "Use A and D to move Left and Right";
        timer = 4;
    }
    void Walking_Update()
    {
        Debug.Log("swaws");
        if (Input.GetKeyDown(KeyCode.A))
            bool1 = true;
        else if (Input.GetKeyDown(KeyCode.D))
            bool2 = true;

        if (bool1 && bool2)
        {
            fsm.ChangeState(States.Sprinting, StateTransition.Safe);
        }
    }

    void Sprinting_Enter()
    {
        tutorialText.text = "Hold Shift (along with A and D) to sprint Left or Right";
        timer = 4;
    }
    void Sprinting_Update()
    {
        if (Input.GetKeyDown(KeyCode.RightShift) || Input.GetKeyDown(KeyCode.LeftShift))
            fsm.ChangeState(States.Jumping, StateTransition.Safe);

    }

    void Jumping_Enter()
    {
        tutorialText.text = "Use Space Bar to Jump up";
        timer = 4;
    }
    void Jumping_Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            tutorialText.text = "End of Tutorial text testing";
        }
    }

    void StopPulsing()
    {
        tutorialTextObject.GetComponent<TutorialTextPulse>().anim.Stop();
    }

    void StartPulsing()
    {
        tutorialTextObject.GetComponent<TutorialTextPulse>().anim.Play();
    }

}
