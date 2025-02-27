using UnityEngine;
using System.Collections.Generic;

public class MovementBehavior : MonoBehaviour
{
    public static MovementBehavior instance { get; private set; }

    public static Vector3 mousePosition;

    private GameObject mousePointer;
    private GameObject closestObject;

    private ProxyChecker proxyChecker;

    private bool greenCapped, yellowCapped, redCapped, blueCapped;


    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        proxyChecker = new ProxyChecker();
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mousePointer = Resources.Load<GameObject>("Prefabs/MousePointer");
        mousePointer = Instantiate(mousePointer, Vector3.zero, Quaternion.identity);
        mousePointer.name = "MousePointer";


        BaseBehavior[] bases = Object.FindObjectsByType<BaseBehavior>(FindObjectsSortMode.None);
        closestObject = bases[0].gameObject;
        foreach (BaseBehavior pBase in bases)
        {
            proxyChecker.Track(pBase.gameObject);
        }


    }

    // Update is called once per frame
    void Update()
    {
        //Get and maintain mouse position mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;
        mousePointer.transform.position = mousePosition;

        closestObject = proxyChecker.GetClosest();
        Debug.Log(proxyChecker.GetClosest().name);
    }

}
