using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldSpaceUIComponent : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Canvas canvas = GetComponent<Canvas>();
        canvas.worldCamera = GameObject.FindGameObjectWithTag("UICamera").GetComponent<Camera>();
        gameObject.layer = LayerMask.NameToLayer("UI");
        foreach(Transform child in transform) {
            child.gameObject.layer = gameObject.layer;
        }
        //canvas.eventCamera = GameObject.FindGameObjectWithTag("UICamera").GetComponent<Camera>;
    }

}
