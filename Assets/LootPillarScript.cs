using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootPillarScript : MonoBehaviour
{
    ParticleSystem particle;
    // Start is called before the first frame update
    void Start()
    {
        particle = GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartEmittingLootPillar()
    {
        gameObject.SetActive(true);
    }

    public void StopEmittingLootPillar()
    {
        gameObject.SetActive(false);
    }
}
