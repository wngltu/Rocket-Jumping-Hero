using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamageNumberScript : MonoBehaviour
{
    public Canvas canvas;
    public TextMeshProUGUI text;
    Rigidbody rb;
    public float damage = 10;
    private void Start()
    {
        rb = GetComponentInParent<Rigidbody>();
        gameObject.transform.position += new Vector3(Random.Range(-.5f,.5f), Random.Range(-.25f, .25f), 0);
        rb.velocity = new Vector3(Random.Range(-2.5f, 2.5f), Random.Range(1f, 5f), 0);
        canvas = GetComponentInParent<Canvas>();
        text = GetComponent<TextMeshProUGUI>();
        text.text = "-" + ((int)damage).ToString();
    }

    private void FixedUpdate()
    {
        //transform.position += new Vector3(0, .5f, 0);
        text.alpha -= .025f;

        if (text.alpha < 0)
        {
            Destroy(canvas.gameObject);
        }
    }
}
