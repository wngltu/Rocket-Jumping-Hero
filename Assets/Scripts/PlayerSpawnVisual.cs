using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawnVisual : MonoBehaviour
{
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(transform.position, .5f);
    }
}
