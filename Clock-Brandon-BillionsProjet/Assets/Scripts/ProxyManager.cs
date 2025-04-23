using UnityEngine;
using System.Collections.Generic;

public class ProxyManager :  MonoBehaviour {

    public ProxyManager instance { get; set; }

    public static Dictionary<string, LinkedList<GameObject>> relevantObjects = new Dictionary<string, LinkedList<GameObject>>();
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
            worldSpawnlings[GameManager.GetColor(b.gameObject)] = new LinkedList<GameObject>();
            worldSpawnlings[GameManager.GetColor(b.gameObject)].AddLast(b.gameObject);
        }
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

    public static GameObject GetNearestRelevantObject(GameObject subject, string color)
    {
        Vector3 closestVector = new Vector3(int.MaxValue, int.MaxValue,int.MaxValue);
        GameObject closestObj = null;

        foreach (GameObject obj in relevantObjects[color])
        {
            if (obj.tag == "PlayerBase")
                continue;

            if (Vector3.Distance(subject.transform.position, obj.transform.position) < Vector3.Distance(subject.transform.position, closestVector))
            {
                closestVector = obj.transform.position;
                closestObj = obj;
            }
        }
        return closestObj;
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
    public static void TargetClosestEnemy(string team, float range, float reloadTime, GameObject breach, GameObject projectile,GameObject shooter)
    {
        Vector3 closestBillionVector = new Vector3(int.MaxValue, int.MaxValue, int.MaxValue);
        Vector3 position = shooter.transform.position;

        bool snapAiming = shooter.GetComponent<BillionsBehavior>() ? true : false;

        GameObject closestBillionObj = null;

        float angle = 0f;

        foreach (string color in ProxyManager.worldSpawnlings.Keys)
        {
            if (color == team)
                continue;

            foreach (GameObject enemy in ProxyManager.worldSpawnlings[color])
            {
                if (shooter.GetComponent<BaseBehavior>() && enemy.GetComponent<BaseBehavior>())
                    continue;

                if (Vector3.Distance(closestBillionVector, position) > Vector3.Distance(enemy.transform.position, position))
                {
                    closestBillionVector = enemy.transform.position;
                    closestBillionObj = enemy;
                }
            }
        }

        if (closestBillionObj == null)
            return;


        Vector3 relativeLocation = position;
        if (closestBillionObj != null)
        {
            relativeLocation = closestBillionObj.transform.position - position;
            angle = (Mathf.Atan2(relativeLocation.y, relativeLocation.x) * Mathf.Rad2Deg) - 90f;
        }
        if (snapAiming == true)
            breach.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        else
        {
            var fromAngle = Quaternion.Euler(0, 0, breach.transform.rotation.eulerAngles.z);
            var toAngle = Quaternion.Euler(0, 0, angle);

            breach.transform.rotation = Quaternion.RotateTowards(fromAngle, toAngle, 35.0f * Time.deltaTime);
        }

        if (closestBillionObj != null && Vector3.Distance(closestBillionObj.transform.position, position) < range)
            Shoot(shooter, team, reloadTime, breach, projectile);
    }

    private static void Shoot(GameObject shooter, string team, float reloadTime, GameObject breach, GameObject projectile)
    {
        if (reloadTime <= 0)
        {
            GameObject spawn = Instantiate(projectile, breach.transform.position, breach.transform.rotation);
            ProjectileBehavior projB = spawn.GetComponent<ProjectileBehavior>();
            SpriteRenderer spawnColorSetter = spawn.transform.GetComponent<SpriteRenderer>();
            if (team == "Green")
                spawnColorSetter.color = Color.green;
            else if (team == "Yellow")
                spawnColorSetter.color = Color.yellow;
            else if (team == "Red")
                spawnColorSetter.color = Color.red;
            else if (team == "Blue")
                spawnColorSetter.color = Color.blue;
            if (projB != null)
                projB.SetColor(team);

            if (shooter.GetComponent<BillionsBehavior>())
            {
                shooter.GetComponent<BillionsBehavior>().Reload();
            }
            else if (shooter.GetComponent<BaseBehavior>())
            {
                shooter.GetComponent<BaseBehavior>().Reload();
            }

        }
    }
}
