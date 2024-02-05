using UnityEngine;

public class PlayerZoomManager : MonoBehaviour
{
    public float zoomMin = 5f;
    public float zoomMax = 12.44f;
    public Camera cam;
    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.mouseScrollDelta.y != 0)
        {
            if (cam.fieldOfView > 40f && Input.mouseScrollDelta.y > 0 && Input.GetKey(KeyCode.LeftControl)) //zoom in if not at zoom limit 
            {
                print("test camera fov");
                cam.fieldOfView -= 3* Input.mouseScrollDelta.y;
            }
            else if (cam.fieldOfView < 60f && Input.mouseScrollDelta.y < 0 && Input.GetKey(KeyCode.LeftControl)) //zoom out if not at zoom limit
            {
                cam.fieldOfView -= 3* Input.mouseScrollDelta.y;
            }
        }

        if (cam.fieldOfView < 40)
            cam.fieldOfView = 40;
        else if (cam.fieldOfView > 60f)
            cam.fieldOfView = 60f;
    }
}
