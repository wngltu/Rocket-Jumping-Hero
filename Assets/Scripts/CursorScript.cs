using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CursorScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (SceneManager.GetActiveScene().name == "Main Menu" || SceneManager.GetActiveScene().name == "Credits")
            Cursor.visible = true;
        else
            Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
