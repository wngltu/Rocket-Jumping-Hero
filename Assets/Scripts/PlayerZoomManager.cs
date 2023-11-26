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
            if (cam.orthographicSize > 5 && Input.mouseScrollDelta.y > 0) //zoom in if not at zoom limit 
            {
                cam.orthographicSize -= Input.mouseScrollDelta.y;
            }
            else if (cam.orthographicSize < 12.44f && Input.mouseScrollDelta.y < 0) //zoom out if not at zoom limit
            {
                cam.orthographicSize -= Input.mouseScrollDelta.y;
            }
        }

        if (cam.orthographicSize < 5)
            cam.orthographicSize = 5;
        else if (cam.orthographicSize > 12.44f)
            cam.orthographicSize = 12.44f;
    }
}
