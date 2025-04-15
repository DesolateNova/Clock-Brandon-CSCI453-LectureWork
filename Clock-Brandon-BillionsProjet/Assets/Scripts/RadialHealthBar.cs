using UnityEngine;
using UnityEngine.UI;

public class RadialHealthBar : MonoBehaviour
{

    private BaseBehavior pHealth;
    private GameObject parent;
    private float maxValue;
    private float currentValue;
    private Image image;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        parent = transform.parent.gameObject;
        pHealth = parent.GetComponentInParent<BaseBehavior>();
        if (pHealth == null)
            Debug.Log("Parent is Not Valid");
        maxValue = pHealth.MAXHEALTH;

        image = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        currentValue= pHealth.curHealth;
        float fillAmount = (Mathf.Max(0, (0 + (maxValue - 0) * (currentValue / maxValue)) / maxValue));
        image.fillAmount = fillAmount;
    }
}
