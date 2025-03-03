using UnityEngine;
using System.Collections.Generic;

public class MovementBehavior : MonoBehaviour
{
    public static MovementBehavior instance { get; private set; }

    private GameObject closestObject;




    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }


}
