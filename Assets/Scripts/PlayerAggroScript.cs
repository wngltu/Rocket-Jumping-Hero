using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAggroScript : MonoBehaviour
{
    float aggroCheckInterval = 1.5f;
    float aggroCheckTimer = 0f;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("enemy"))
        {
            if (other.gameObject.GetComponent<Enemy>().aggroed == false)
                other.gameObject.GetComponent<Enemy>().AggroEnemy();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("enemy"))
        {
            if (other.gameObject.GetComponent<Enemy>().aggroed == true)
                other.gameObject.GetComponent<Enemy>().DeaggroEnemy();
        }
    }
}
