using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteParticleScript : MonoBehaviour
{
    public float deleteDuration = 1.5f;
    // Start is called before the first frame update
    void Start()
    {
        Invoke("DeleteObj", deleteDuration);
    }


    void DeleteObj()
    {
        Destroy(gameObject);
    }
}
