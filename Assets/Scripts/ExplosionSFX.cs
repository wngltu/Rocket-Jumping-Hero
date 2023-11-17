using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionSFX : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<AudioSource>().Play();
        Invoke("DestroyGameobject", 3f);
    }

    void DestroyGameobject()
    {
        Destroy(gameObject);
    }
}
