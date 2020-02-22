using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equipable : MonoBehaviour
{
    public bool readyToPickUp = true;

    private Canvas infoCanvas;

    [SerializeField]
    Transform sprite;

    // Start is called before the first frame update
    void Start()
    {
        infoCanvas = GetComponentInChildren<Canvas>();
        infoCanvas.gameObject.SetActive(false);

        if (readyToPickUp && sprite != null)
        {
            sprite.Rotate(Vector3.forward, Random.Range(0, 360));
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Hover()
    {
        infoCanvas.gameObject.SetActive(true);
    }
    
    public void UnHover()
    {
        infoCanvas.gameObject.SetActive(false);
    }
    
    public void Equip()
    {
        readyToPickUp = false;
        infoCanvas.gameObject.SetActive(false);
        if (sprite != null)
        {
            sprite.localRotation = Quaternion.Euler(Vector3.zero);
        }
    }

    public void Drop()
    {
        readyToPickUp = true;
        if (sprite != null)
        {
            sprite.Rotate(Vector3.forward, Random.Range(0, 360));
        }
        transform.rotation = Quaternion.Euler(Vector3.zero);
    }
}
