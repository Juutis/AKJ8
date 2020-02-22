using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Equipable : MonoBehaviour
{
    public bool readyToPickUp = true;
    public bool blocked = false;

    private Canvas infoCanvas;

    [SerializeField]
    Transform sprite;

    [SerializeField]
    Image notificationBackground;

    [SerializeField]
    Color colorTooFar;

    [SerializeField]
    Color colorCloseEnough;

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

    public void Hover(bool closeEnough)
    {
        infoCanvas.gameObject.SetActive(true);
        if (!blocked && closeEnough)
        {
            notificationBackground.color = colorCloseEnough;
        }
        else
        {
            notificationBackground.color = colorTooFar;
        }
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
