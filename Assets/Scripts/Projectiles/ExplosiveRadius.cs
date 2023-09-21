using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ExplosiveRadius : MonoBehaviour
{
    public float explosionRadius = 1f;
    SpriteRenderer sprite;
    // Start is called before the first frame update
    void Start()
    { 
        Invoke("destroyObj", .25f);
        transform.localScale = new Vector3(explosionRadius, explosionRadius, 1);
        sprite = GetComponent<SpriteRenderer>();

    }

    private void Update()
    {
        transform.localScale += new Vector3(Time.deltaTime * explosionRadius, Time.deltaTime * explosionRadius, 1);
        sprite.color = new Vector4(sprite.color.r, sprite.color.g, sprite.color.b, sprite.color.a - Time.deltaTime*explosionRadius);
    }

    private void destroyObj()
    {
        Destroy(gameObject);
    }
}
