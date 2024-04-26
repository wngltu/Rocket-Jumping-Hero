using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompleteGameTele : MonoBehaviour
{
    Level2FSM lv2fsm;

    private void Start()
    {
        lv2fsm = FindObjectOfType<Level2FSM>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerMovement player = FindObjectOfType<PlayerMovement>();

            player.isRespawning = true;
            player.controller.enabled = false;
            player.playerModel.SetActive(false);
            lv2fsm.WinScreen();
        }
    }
}
