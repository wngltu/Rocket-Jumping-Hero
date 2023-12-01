using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillboxScript : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<PlayerHealth>().TakeDamage(99999);
        }
        if (other.gameObject.CompareTag("enemy"))
        {
            other.gameObject.GetComponent<Enemy>().takeDamage(99999);
        }
    }
}
