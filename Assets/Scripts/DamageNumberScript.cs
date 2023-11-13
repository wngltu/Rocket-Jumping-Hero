using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamageNumberScript : MonoBehaviour
{
    public Canvas canvas;
    public TextMeshProUGUI text;
    public float damage = 10;
    private void Start()
    {
        canvas = GetComponentInParent<Canvas>();
        text = GetComponent<TextMeshProUGUI>();
        text.text = "-" + damage.ToString();
    }

    private void FixedUpdate()
    {
        transform.position += new Vector3(0, .05f, 0);
        text.alpha -= .025f;

        if (text.alpha < 0)
        {
            Destroy(canvas.gameObject);
        }
    }
}
