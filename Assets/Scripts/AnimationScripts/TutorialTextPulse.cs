using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTextPulse : MonoBehaviour
{
    public AnimationClip pulse;
    public Animation anim;

    private void Start()
    {
        anim = gameObject.GetComponent<Animation>();
    }
    private void Update()
    {
        anim.clip = pulse;
    }
}
