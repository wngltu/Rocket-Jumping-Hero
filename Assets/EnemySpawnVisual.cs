using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnVisual : MonoBehaviour
{
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, .3f);
    }
}
