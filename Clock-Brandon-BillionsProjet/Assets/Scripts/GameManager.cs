using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{

    public static GameManager instance { get; private set; }

    [SerializeField] public static GameObject greenFlag, yellowFlag, blueFlag, redFlag;

    private static KeyValuePair<Color, string> red = new KeyValuePair<Color, string>(Color.red, "Red");
    private static KeyValuePair<Color, string> blue = new KeyValuePair<Color, string>(Color.blue, "Blue");
    private static KeyValuePair<Color, string> yellow = new KeyValuePair<Color, string>(Color.yellow, "Yellow");
    private static KeyValuePair<Color, string> green = new KeyValuePair<Color, string>(Color.green, "Green");
    public static GameObject mousePointer;
    public static GameObject arenaCenter;

    public static TilemapCollider2D borderWall;



    public static Vector3 mousePosition;


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

    void Start()
    {
        arenaCenter = GameObject.Find("Arena Center");

        mousePointer = Resources.Load<GameObject>("Prefabs/MousePointer");
        mousePointer = Instantiate(mousePointer, Vector3.zero, Quaternion.identity);
        mousePointer.name = "MousePointer";

    }

    // Update is called once per frame
    void Update()
    {
        SetMousePos();
    }

    private void SetMousePos()
    {
        //Get and maintain mouse position mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;
        mousePointer.transform.position = mousePosition;
    }

    public static Vector3 ReportMousePos()
    {
        return mousePosition;
    }

    public static string GetColor(GameObject item)
    {
        if (item.CompareTag("Spawnling"))
        {
            Color objectColor = item.transform.GetChild(0).GetComponent<SpriteRenderer>().color;
            if (objectColor == red.Key)
                return red.Value;
            else if (objectColor == yellow.Key)
                return yellow.Value;
            else if (objectColor == green.Key)
                return green.Value;
            else if (objectColor == blue.Key)
                return blue.Value;
            else return "Unkown Color";
        }

        string postFix = item.name.Substring(item.name.Length - 2);
        if (postFix == "_G")
            return "Green";
        else if (postFix == "_Y")
            return "Yellow";
        else if (postFix == "_B")
            return "Blue";
        else if (postFix == "_R")
            return "Red";
        else return "Unknown Color";
    }

    public static Vector3 GetDirectionTowards(GameObject to, GameObject from)
    {
        return (to.transform.position - from.transform.position).normalized;
    }
}
