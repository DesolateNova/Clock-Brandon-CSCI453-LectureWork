using UnityEngine;
using System.Collections.Generic;
using UnityEditor.UI;

public class MovementBehavior : MonoBehaviour
{
    private static MovementBehavior instance { get; set; }

    private static GameObject nearestBase, nearestFlag;
    private GameObject redFlag, blueFlag, greenFlag, yellowFlag;
    private static bool greenCapped, yellowCapped, redCapped, blueCapped;
    private static int nGreen = 0, nYellow = 0, nBlue = 0, nRed = 0;

    private GameObject selectedObject;
    private float timer, clickDelay;
    private LineRenderer traceLine;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        clickDelay = Time.deltaTime * 2;

    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        redFlag = Resources.Load<GameObject>("Prefabs/Flag_R");
        blueFlag = Resources.Load<GameObject>("Prefabs/Flag_B");
        greenFlag = Resources.Load<GameObject>("Prefabs/Flag_G");
        yellowFlag = Resources.Load<GameObject>("Prefabs/Flag_Y");
        timer = clickDelay;
    }

    // Update is called once per frame
    void Update()
    {
        enforceCap(nGreen, nYellow, nBlue, nRed);
        RaycastHit2D hit;
        if (selectedObject != null)
            Debug.Log("Selected Object: " + selectedObject.name);

        if (Input.GetMouseButtonDown(0))
        {
            hit = Physics2D.Raycast(new Vector2(GameManager.mousePointer.transform.position.x, GameManager.mousePointer.transform.position.y), Vector2.zero);
            if (hit)
            {
                BillionsBehavior temp;
                if (hit.collider.gameObject.CompareTag("Spawnling"))
                {
                    temp = hit.collider.gameObject.GetComponent<BillionsBehavior>();
                    temp.TakeDamage(temp.maxHealth / 4);
                }
                return;
            }
        }


        if (Input.GetAxis("Mouse0") == 0)
        {
            timer = clickDelay;
            if (selectedObject != null && selectedObject.tag == "Waypoint")
                selectedObject.transform.position = GameManager.mousePosition;


            if (selectedObject != null)
                RemoveTraceLine(selectedObject);
            selectedObject = null;
        }

        if (Input.GetAxis("Mouse0") == 1)
        {
            timer -= Time.deltaTime;
            if (timer >= 0f)
            {
                string flagOwner;
                hit = Physics2D.Raycast(new Vector2(GameManager.mousePointer.transform.position.x, GameManager.mousePointer.transform.position.y), Vector2.zero);
                if (hit)
                    selectedObject = hit.collider.gameObject;

                nearestBase = ProxyManager.GetNearestBase(GameManager.mousePointer);
                nearestFlag = ProxyManager.GetNearestToMouse();
                string baseOwner = GameManager.GetColor(nearestBase);


                if (nearestFlag != null)
                    flagOwner = GameManager.GetColor(nearestFlag);

                if (!FlagsAtCap(baseOwner) && hit == false)
                    spawnWaypoint(nearestBase);
                else if (FlagsAtCap(baseOwner) && hit == false)
                    nearestFlag.transform.position = GameManager.mousePosition;
            }
            if (timer < 0)
            {
                if (selectedObject != null && selectedObject.GetComponent<LineRenderer>() == false)
                {
                    AddTraceLine(selectedObject);
                }
                if (traceLine != null)
                    traceLine.SetPosition(1, GameManager.mousePointer.transform.position);
            }

        }
    }

    private void AddTraceLine(GameObject obj)
    {
        traceLine = obj.AddComponent<LineRenderer>();
        traceLine.startWidth = 0.05f;
        traceLine.endWidth = 0.05f;
        traceLine.positionCount = 0;
        traceLine.material = new Material(Shader.Find("Sprites/Default"));
        traceLine.startColor = Color.white;
        traceLine.endColor = Color.white;
        traceLine.positionCount = 2;
        traceLine.SetPosition(0, selectedObject.transform.position);
    }

    private void RemoveTraceLine(GameObject obj)
    {
        Destroy(obj.GetComponent<LineRenderer>());
    }

    private bool FlagsAtCap( string color)
    {
        switch (color)
        {
            case "Red":
                return redCapped;
            case "Yellow":
                return yellowCapped;
            case "Blue":
                return blueCapped;
            case "Green":
                return greenCapped;
            default:
                return false;
        }
    }


    private void FlagsAtCap(GameObject obj)
    {
        string color = GameManager.GetColor(obj);
        FlagsAtCap(color);
    }

    private void spawnWaypoint(GameObject ob)
    {
        string player = GameManager.GetColor(nearestBase);
        if (FlagsAtCap(player))
            return;

        GameObject flag = null;


        switch (player)
        {
            case "Red":
                flag = Instantiate(redFlag, GameManager.ReportMousePos(), Quaternion.identity);
                flag.name = "Flag_R";
                nRed++;
                break;
            case "Blue":
                flag = Instantiate(blueFlag, GameManager.ReportMousePos(), Quaternion.identity);
                flag.name = "Flag_B";
                nBlue++;
                break;
            case "Yellow":
                flag = Instantiate(yellowFlag, GameManager.ReportMousePos(), Quaternion.identity);
                nYellow++;
                flag.name = "Flag_Y";
                break;
            case "Green":
                flag =Instantiate(greenFlag, GameManager.ReportMousePos(), Quaternion.identity);
                nGreen++;
                flag.name = "Flag_G";
                break;
        }
        if (flag != null)
            ProxyManager.AddToSide(player, flag);
    }

    private void enforceCap(int nGreen, int nYellow, int nBlue, int nRed)
    {
        if (nGreen >= 2)
            greenCapped = true;
        if (nYellow >= 2)
            yellowCapped = true;
        if (nBlue >= 2)
            blueCapped = true;
        if (nRed >= 2)
            redCapped = true;
    }
}
