using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeScript : MonoBehaviour
{
    public float baseDamage = 10;
    public SpikeType spiketype;
    public GameObject respawnPoint;
    public enum SpikeType
    {
        NoTeleport,
        Teleport
    }

    private void OnCollisionStay(Collision collision)
    {
        if (spiketype == SpikeType.NoTeleport && collision.gameObject.CompareTag("Player")) //damage the player, nothing else
            collision.gameObject.GetComponent<PlayerHealth>().TakeTrapDamage(10);
        else if (spiketype == SpikeType.Teleport && collision.gameObject.CompareTag("Player")) //teleport player back to respawn point
        {
            collision.gameObject.GetComponent<CharacterController>().enabled = false; //need to turn controller off to warp object
            collision.gameObject.transform.position = respawnPoint.transform.position;
            collision.gameObject.GetComponent<CharacterController>().enabled = true;
            collision.gameObject.GetComponent<PlayerHealth>().TakeTrapDamage(10);
        }

        if (spiketype == SpikeType.Teleport && collision.gameObject.CompareTag("droppedweapon"))
        {
            collision.gameObject.transform.position = respawnPoint.transform.position;
        }
    }
}