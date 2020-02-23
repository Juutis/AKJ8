using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorScript : MonoBehaviour
{
    [SerializeField]
    GameObject crosshair;

    [SerializeField]
    GameObject interact;

    [SerializeField]
    GameObject interactPrompt;

    [SerializeField]
    GameObject pressE;

    [SerializeField]
    GameObject moveCloser;

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

    public void SetInteract(bool yes, bool closeEnough, bool usable)
    {
        if (yes)
        {
            interact.SetActive(true);
            crosshair.SetActive(false);
            if (usable)
            {
                interactPrompt.SetActive(true);
                if (closeEnough)
                {
                    pressE.SetActive(true);
                    moveCloser.SetActive(false);
                }
                else
                {
                    pressE.SetActive(false);
                    moveCloser.SetActive(true);
                }
            }
            else
            {
                interactPrompt.SetActive(false);
            }
        }
        else
        {
            interact.SetActive(false);
            interactPrompt.SetActive(false);
            crosshair.SetActive(true);
        }
    }
}
