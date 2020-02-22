using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelExit : MonoBehaviour
{
    [SerializeField]
    public bool locked = false;

    [SerializeField]
    Text infoText;

    [SerializeField]
    GameObject spriteLocked;

    [SerializeField]
    GameObject spriteOpen;

    Equipable eq;

    // Start is called before the first frame update
    void Start()
    {
        eq = GetComponent<Equipable>();

        if (locked)
        {
            infoText.text = "This hatch leads to the next level of the cellar but it seems to be locked. There must be a key around here somewhere.";
            spriteLocked.SetActive(true);
            spriteOpen.SetActive(false);
            eq.blocked = true;
        }
        else
        {
            infoText.text = "This hatch leads to the next level of the cellar.\n\nPress<E> to go deeper.";
            spriteLocked.SetActive(false);
            spriteOpen.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Exit()
    {
        Debug.Log("Level completed!");
    }

    public void Unlock()
    {
        locked = false;
        eq.blocked = false;
        infoText.text = "This hatch leads to the next level of the cellar.\n\nPress<E> to go deeper.";
    }
}
