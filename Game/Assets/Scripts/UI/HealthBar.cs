using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{

    [SerializeField]
    private Image hpBackground;
    [SerializeField]
    private Text hpText;

    [SerializeField]
    private Color fullHpColor = Color.green;
    [SerializeField]
    private Color zeroHpColor = Color.red;
    int maxHp = 100;
    public static HealthBar main;
    void Awake() {
        main = this;
    }

    public void SetHp(float newHp) {
        int displayHp = (int)newHp;
        if (displayHp < 0) {
            displayHp = 0;
        }
        hpText.text = string.Format("{0}/{1}", displayHp, maxHp);
        float sizeFactor = (newHp) / (1.0f * maxHp);
        if (sizeFactor < 0) {
            sizeFactor = 0f;
        }
        hpBackground.color = Color.Lerp(zeroHpColor, fullHpColor, sizeFactor);
        RectTransform rt = hpBackground.GetComponent<RectTransform>();
        rt.localScale = new Vector3(sizeFactor, 1f, 1f);
    }
}
