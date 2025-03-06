using UnityEngine;
using System.Collections.Generic;

public class ProxyManager :  MonoBehaviour {

    public ProxyManager instance { get; set; }

    private static Dictionary<string, LinkedList<GameObject>> relevantObjects = new Dictionary<string, LinkedList<GameObject>>();
    public static Dictionary<string, LinkedList<GameObject>> worldSpawnlings = new Dictionary<string, LinkedList<GameObject>>();
    private static BaseBehavior[] startingBases;
    public void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    public void Start()
    {
        startingBases = GameObject.FindObjectsByType<BaseBehavior>(FindObjectsSortMode.None);

        foreach (BaseBehavior b in startingBases)
        {
            relevantObjects[GameManager.GetColor(b.gameObject)] = new LinkedList<GameObject>();
            relevantObjects[GameManager.GetColor(b.gameObject)].AddLast(b.gameObject);
        }
        worldSpawnlings = new Dictionary<string, LinkedList<GameObject>>();
    }

    public static GameObject GetNearestBase(GameObject subject)
    {
        Vector3 closestVector = new Vector3(int.MaxValue, int.MaxValue, int.MaxValue);
        GameObject closestObj = null;


        foreach (BaseBehavior b in startingBases)
        {
            if (Vector3.Distance(subject.transform.position, b.transform.position) < Vector3.Distance(subject.transform.position, closestVector))
            {
                closestVector = b.transform.position;
                closestObj = b.gameObject;
            }
        }
        return closestObj;
    }

    public static GameObject GetNearestToMouse()
    {

        Vector3 closestVector = new Vector3(int.MaxValue, int.MaxValue,int.MaxValue);
        GameObject closestObj = null;


        foreach (string catagory in relevantObjects.Keys)
        {
            foreach (GameObject obj in relevantObjects[catagory])
            {
                if (obj.tag == "PlayerBase")
                    continue;

                if (Vector3.Distance(GameManager.mousePosition, obj.transform.position) < Vector3.Distance(GameManager.mousePosition, closestVector))
                {
                    closestVector = obj.transform.position;
                    closestObj = obj;
                }
            }
        }

        return closestObj;
    }

    public static GameObject GetNearestRelevantObject(string color)
    {
        Vector3 closestVector = new Vector3(int.MaxValue, int.MaxValue,int.MaxValue);
        GameObject closestObj = null;

        foreach (GameObject obj in relevantObjects[color])
        {
            if (obj.tag == "PlayerBase")
                continue;

            if (Vector3.Distance(GameManager.mousePosition, obj.transform.position) < Vector3.Distance(GameManager.mousePosition, closestVector))
            {
                closestVector = obj.transform.position;
                closestObj = obj;
            }
        }
        return closestObj;
    }

    public static void GetNearestRelevantObject(GameObject subject)
    {

        string category = GameManager.GetColor(subject);
        GetNearestRelevantObject(category);
    }

    public static void AddToSide(string playersSide, GameObject obj) 
    {
        if (!relevantObjects[playersSide].Contains(obj) && obj.tag == "Waypoint")
        {
            relevantObjects[playersSide].AddLast(obj);
        }

        if (!worldSpawnlings[playersSide].Contains(obj) && obj.tag == "Spawnling")
        {
            worldSpawnlings[playersSide].AddLast(obj);
        }
    }

}
