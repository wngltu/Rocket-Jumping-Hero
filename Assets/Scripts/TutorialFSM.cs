using System.Collections;
using UnityEngine;
using MonsterLove.StateMachine;
using TMPro;

public class TutorialFSM : MonoBehaviour
{
    public GameObject prefabTable;
    public GameObject tutorialTextObject;
    public GameObject enemySpawn1;
    public GameObject enemy2Spawn1;
    public GameObject enemy2Spawn2;
    public GameObject enemy3Spawn1;
    public GameObject enemy3Spawn2;
    public GameObject enemy3Spawn3;
    public GameObject player;
    public TextMeshProUGUI tutorialText;
    public weaponManager weaponMgr;
    public States currentState;
    public float timer;
    public float pulseTime = 2;
    private StateMachine<States, StateDriverUnity> fsm;
    bool bool1 = false; //bool variables used for input detection
    bool bool2 = false; //bool variables used for input detection

    public enum States
    {
        Walking,
        Sprinting,
        WalkingGoal,
        Jumping,
        SingleJumpingGoal,
        DoubleJumpingGoal,
        SprintJumpingGoal,
        EquipWeapon,
        Enemy1,
        Inventory,
        EquipWeapon2,
        RJPullOut,
        RJShoot,
        RocketJump,
        RJVerticalGoal,
        RJHorizontalGoal,
        Platform,
        Enemy2,
        Inventory2,
        PressurePlate,
        ShootPressurePlate,
        DoubleRJ,
        Lever,
        TutorialFinished,


    }

    private void Start()
    {
        fsm = new StateMachine<States, StateDriverUnity>(this);
        player = FindObjectOfType<PlayerHealth>().gameObject;
        weaponMgr = FindObjectOfType<weaponManager>();
        prefabTable = FindObjectOfType<PrefabLoot>().gameObject;
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
        timer = pulseTime;
    }
    void Walking_Update()
    {
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
        timer = pulseTime;
    }
    void Sprinting_Update()
    {
        if (Input.GetKeyDown(KeyCode.RightShift) || Input.GetKeyDown(KeyCode.LeftShift))
            fsm.ChangeState(States.WalkingGoal, StateTransition.Safe);
    }

    public bool walkingGoal;
    void WalkingGoal_Enter()
    {
        tutorialText.text = "Approach the jump obstacle on the right";
        timer = pulseTime;
    }
    void WalkingGoal_Update()
    {
        if (walkingGoal)
            fsm.ChangeState(States.Jumping);
    }

    void Jumping_Enter()
    {
        tutorialText.text = "Use Space Bar to Jump up";
        timer = pulseTime;
    }
    void Jumping_Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            fsm.ChangeState(States.SingleJumpingGoal);
    }

    public bool jumpingGoal = false;
    void SingleJumpingGoal_Enter()
    {
        tutorialText.text = "Use the Jump(Space) to overcome these obstacles";
    }
    void SingleJumpingGoal_Update()
    {
        if (jumpingGoal)
            fsm.ChangeState(States.DoubleJumpingGoal);
    }

    public bool doubleJumpingGoal = false;
    void DoubleJumpingGoal_Enter()
    {
        tutorialText.text = "Use the Double Jump (Space while in Mid-Air) to overcome these obstacles";
        timer = pulseTime;
    }
    void DoubleJumpingGoal_Update()
    {
        if (doubleJumpingGoal)
            fsm.ChangeState(States.SprintJumpingGoal);
    }

    public bool sprintingGoal = false;
    void SprintJumpingGoal_Enter()
    {
        tutorialText.text = "Use both Sprint(shift) and/or Double Jump(space) to overcome this obstacle";
        timer = pulseTime;
    }
    void SprintJumpingGoal_Update()
    {
        if (sprintingGoal)
            fsm.ChangeState(States.EquipWeapon);
    }

    void EquipWeapon_Enter()
    {
        tutorialText.text = "A dropped knife is on the ground. Pick it up by looking in its direction and clicking E";
        timer = pulseTime;
    }
    void EquipWeapon_Update()
    {
        if (weaponMgr.weaponInventory.Count == 1)
            fsm.ChangeState(States.Enemy1);
    }

    void Enemy1_Enter()
    {
        tutorialText.text = "Watch out, an enemy! Enemies typically wind up for a brief period before attacking, try approaching and observing its movements. Defeat the enemy with Left Click attacks and pick up its loot";
        Instantiate(prefabTable.GetComponent<PrefabEnemy>().meleeEnemy, enemySpawn1.transform);
        timer = pulseTime;
    }
    void Enemy1_Update()
    {
        foreach (weaponScript weapon in player.GetComponent<weaponManager>().weaponInventory) //does the player have pistol in inventory?
            if (weapon.GetComponent<Pistol>() != null)
                fsm.ChangeState(States.Inventory);
    }

    void Inventory_Enter()
    {
        tutorialText.text = "Switch to the new weapon with scroll wheel or numbers (1, 2, 3, etc.)";
        timer = pulseTime;
    }
    void Inventory_Update()
    {
        if (weaponMgr.equippedWeapon.gameObject.GetComponent<Pistol>() != null) //is player currently holding the pistol?
            fsm.ChangeState(States.EquipWeapon2);
    }

    void EquipWeapon2_Enter()
    {
        tutorialText.text = "Guns have limited ammo (Bottom right). Guns can shoot with Left Click. Reload with R. Reload to continue";
        timer = pulseTime;
    }
    void EquipWeapon2_Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
            fsm.ChangeState(States.RJPullOut);
    }

    void RJPullOut_Enter()
    {
        tutorialText.text = "Now, pull out Rocket Launcher by holding Right Click. Notice the rocket count at the bottom right";
        timer = pulseTime;
    }
    void RJPullOut_Update()
    {
        if (Input.GetKey(KeyCode.Mouse1))
            fsm.ChangeState(States.RJShoot);
    }

    void RJShoot_Enter()
    {
        tutorialText.text = "Shoot Rocket Launcher by clicking Left Click while holding it";
        timer = pulseTime;
    }
    void RJShoot_Update()
    {
        if (Input.GetKey(KeyCode.Mouse1) || Input.GetKeyDown(KeyCode.Mouse0))
            fsm.ChangeState(States.RocketJump);
    }

    void RocketJump_Enter()
    {
        tutorialText.text = "The player reloads rockets while standing or walking on solid ground. Try shooting a rocket at the players feet";
        timer = pulseTime;
    }
    void RocketJump_Update()
    {
        if (Input.GetKey(KeyCode.Mouse1) && player.GetComponent<CharacterController>().velocity.y > 8f)
            fsm.ChangeState(States.RJVerticalGoal);
    }

    public bool rjGoal1;
    void RJVerticalGoal_Enter()
    {
        tutorialText.text = "Notice no damage is taken. Use the jump(Space) with the momentum of the rocket(M2+M1) to overcome this obstacle";
        timer = pulseTime;
    }
    void RJVerticalGoal_Update()
    {
        if (rjGoal1)
            fsm.ChangeState(States.RJHorizontalGoal);
    }

    public bool rjGoal2;
    void RJHorizontalGoal_Enter()
    {
        tutorialText.text = "Well done. Now try using the horizontal momentum of the rocket and movement keys (A and D) to overcome this gap";
        timer = pulseTime;
    }
    void RJHorizontalGoal_Update()
    {
        if (rjGoal2)
            fsm.ChangeState(States.Platform);
    }

    public bool platformGoal = false;
    void Platform_Enter()
    {
        tutorialText.text = "Great work. This will be your primary transversal tool. This gray block is a platform. Tap or hold S to fall through it, and prepare for a battle.";
        timer = pulseTime;
    }
    void Platform_Update()
    {
        if (platformGoal)
            fsm.ChangeState(States.Enemy2);
    }

    void Enemy2_Enter()
    {
        tutorialText.text = "Defeat the enemy, and pick up its weapon to proceed.";
        Instantiate(prefabTable.GetComponent<PrefabEnemy>().sniperEnemy, enemy2Spawn1.transform);
        Instantiate(prefabTable.GetComponent<PrefabEnemy>().sniperEnemy, enemy2Spawn2.transform);
        timer = pulseTime;
    }
    void Enemy2_Update()
    {
        foreach (weaponScript weapon in player.GetComponent<weaponManager>().weaponInventory) //does the player have pistol in inventory?
            if (weapon.GetComponent<Sniper>() != null)
                fsm.ChangeState(States.PressurePlate);
    }

    public GameObject pPlate;
    public bool pressurePlateGoal = false;
    void PressurePlate_Enter()
    {
        tutorialText.text = "A pressure plate has appeared. Step on it to open the Blue Door temporarily.";
        pPlate.SetActive(true);
        timer = pulseTime;
    }
    void PressurePlate_Update()
    {
        if (pressurePlateGoal == true)
        {
            fsm.ChangeState(States.ShootPressurePlate, StateTransition.Safe);
        }
    }

    public bool shootPressurePlateGoal = false;
    void ShootPressurePlate_Enter()
    {
        tutorialText.text = "To open a red door temporarily, shoot the target in the top left corner of the room.";
    }
    void ShootPressurePlate_Update()
    {
        if (shootPressurePlateGoal == true)
            fsm.ChangeState(States.DoubleRJ, StateTransition.Safe);
    }

    public bool doubleRJGoal = false;
    void DoubleRJ_Enter()
    {
        tutorialText.text = "Using the rocket launcher, shoot a rocket at the wall at the peak of the first rocket jump to go higher. This can be chained multiple times. Dont forget about the Double Jump(Space), which can be used after a rocket jump as well.";
    }
    void DoubleRJ_Update()
    {
        if (doubleRJGoal == true)
            fsm.ChangeState(States.Lever, StateTransition.Safe);
    }

    public bool leverGoal = false;
    void Lever_Enter()
    {
        tutorialText.text = "Click E on a lever to toggle a door to walk through. It may also toggle inactive doors. Reach the end to complete the tutorial.";
        Instantiate(prefabTable.GetComponent<PrefabEnemy>().meleeEnemy, enemy3Spawn1.transform);
        Instantiate(prefabTable.GetComponent<PrefabEnemy>().sniperEnemy, enemy3Spawn2.transform);
        Instantiate(prefabTable.GetComponent<PrefabEnemy>().grenadierEnemy, enemy3Spawn3.transform);
    }
    void Lever_Update()
    {
        if (leverGoal == true)
            fsm.ChangeState(States.TutorialFinished, StateTransition.Safe);
    }
    void TutorialFinished_Enter()
    {
        tutorialText.text = "Congratulations, you have beaten the tutorial. You can toggle the instructions at any time in the pause menu (ESC, or top right button). Return to the main menu to play the main game.";
        timer = pulseTime;
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
