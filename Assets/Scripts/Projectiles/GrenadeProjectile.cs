
using UnityEngine;
using UnityEngine.Rendering;

public class GrenadeProjectile : MonoBehaviour
{
    Enemy enemyScript;
    public static GrenadeProjectile Instance;
    public Transform barrel;
    public GameObject explodeIndicator;
    public GameObject explosionObject;
    Rigidbody rigidbody;
    float damage = 100f;
    float timer = 0f;
    float explosionRadius = 5f;
    float explosionForce = 30f;
    int layerMask = ~((1 << 9) | (1 << 11) | (1 << 13));
    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        rigidbody = GetComponent<Rigidbody>();
        if (Input.mousePosition.x < Screen.width / 2f) //if mouse on left half of screen
        {
            if ((Mathf.Abs(Input.mousePosition.x - Screen.width / 2f)) < 360f)
            {
                rigidbody.AddRelativeForce(new Vector3(0, -100, 300));
            }
            else
            {
                float throwMultiplier = Mathf.Abs(Input.mousePosition.x - Screen.width / 2f)/360;
                rigidbody.AddRelativeForce(new Vector3(0, -100, 300 * throwMultiplier));
            }
        }
        else //if mouse on right half of screen
        {
            if ((Mathf.Abs(Input.mousePosition.x - Screen.width / 2f)) < 100f)
            {
                rigidbody.AddRelativeForce(new Vector3(0, 100, -300));
            }
            else
            {
                float throwMultiplier = Mathf.Abs(Input.mousePosition.x - Screen.width / 2f)/360;
                rigidbody.AddRelativeForce(new Vector3(0, 100, -300 * throwMultiplier));
            }
        }
        timer = 1.5f;
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
