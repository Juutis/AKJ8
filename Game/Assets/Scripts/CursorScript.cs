using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorScript : MonoBehaviour
{
    [SerializeField]
    GameObject crosshair;

    [SerializeField]
    GameObject interact;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetInteract(bool yes)
    {
        if (yes)
        {
            interact.SetActive(true);
            crosshair.SetActive(false);
        }
        else
        {
            interact.SetActive(false);
            crosshair.SetActive(true);
        }
    }
}
