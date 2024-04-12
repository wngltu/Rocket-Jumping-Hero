
using UnityEngine;
using UnityEngine.Rendering;

public class GrenadeProjectile : MonoBehaviour
{
    Enemy enemyScript;
    public static GrenadeProjectile Instance;
    public Transform barrel;
    public GameObject model;
    public GameObject explodeIndicator;
    public GameObject explosionObject;
    Rigidbody rigidbody;
    float damage = 100f;
    float timer = 0f;
    float explosiveDelay = 1.5f;
    float explosionRadius = 5f;
    float explosionForce = 30f;
    int layerMask = ~((1 << 9) | (1 << 11) | (1 << 13));
    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        rigidbody = GetComponent<Rigidbody>();
        /*
            if ((Mathf.Abs(Input.mousePosition.x - Screen.width / 2f)) < 360f)
            {
                rigidbody.AddRelativeForce(new Vector3(0, -100, 300));
            }
            else
            {
                float throwMultiplier = Mathf.Abs(Input.mousePosition.x - Screen.width / 2f)/360;
                rigidbody.AddRelativeForce(new Vector3(0, -100, 300 * throwMultiplier));
            }*/

        if ((Mathf.Abs(Input.mousePosition.x - Screen.width / 2f)) < 360f)
        {
            if (Input.mousePosition.x < Screen.width / 2f)
            {
                rigidbody.AddRelativeForce(new Vector3(0, -100, -300));
                model.transform.localScale = new Vector3(model.transform.localScale.x, -model.transform.localScale.y, model.transform.localScale.z);
            }
            else
                rigidbody.AddRelativeForce(new Vector3(0, 100, -300));
        }
        else
        {
            float throwMultiplier = Mathf.Abs(Input.mousePosition.x - Screen.width / 2f) / 360;
            if (Input.mousePosition.x < Screen.width / 2f)
            {
                rigidbody.AddRelativeForce(new Vector3(0, -100, -300 * throwMultiplier));
                model.transform.localScale = new Vector3(model.transform.localScale.x, -model.transform.localScale.y, model.transform.localScale.z);
            }
            else
                rigidbody.AddRelativeForce(new Vector3(0, 100, -300 * throwMultiplier));

        }
        timer = explosiveDelay;
    }

    private void Update()
    {
        Debug.Log(Mathf.Abs(Input.mousePosition.x - Screen.width / 2f)/100);
        if (timer > 0)
            timer -= Time.deltaTime;
        if (timer < 0)
            Explode();
    }

    public void Explode()
    {
        GameObject newObj = Instantiate(explosionObject, transform);
        newObj.GetComponent<ExplosionScript>().PlayerExplode(damage, explosionRadius, explosionForce);
        Delete();
    }
    void Delete()
    {
        Destroy(this.gameObject);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 5f);
    }
}
