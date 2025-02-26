using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{

    public static GameManager instance { get; private set; }

    [SerializeField] public static GameObject greenFlag, yellowFlag, blueFlag, redFlag;

    private static KeyValuePair<Color, string> red = new KeyValuePair<Color, string>(Color.red, "Red");
    private static KeyValuePair<Color, string> blue = new KeyValuePair<Color, string>(Color.blue, "Blue");
    private static KeyValuePair<Color, string> yellow = new KeyValuePair<Color, string>(Color.yellow, "Yellow");
    private static KeyValuePair<Color, string> green = new KeyValuePair<Color, string>(Color.green, "Green");


    //Singleton creation
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

        //Get and maintain mouse position
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;
        Debug.Log("Current Mouse Pos: " + mousePosition);
    }

    public static string GetColor(Color objectColor)
    {
        if (objectColor == red.Key)
            return red.Value;
        else if (objectColor == yellow.Key)
            return yellow.Value;
        else if (objectColor == green.Key)
            return green.Value;
        else if (objectColor == blue.Key)
            return blue.Value;
        else return "Unknown Color";
             
    }


}
