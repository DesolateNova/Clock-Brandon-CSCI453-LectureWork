using UnityEngine;
using System.Collections.Generic;

public class BaseBehavior : MonoBehaviour
{
    private string baseColor;
    private Vector3 arenaCenterPos;
    private float baseDiameter;
    private float spawnlingDiameter;
    private float timer;

    [SerializeField] private int reserves;
    [SerializeField] private GameObject spawnling;
    [SerializeField] private float spawnTime;


    private Dictionary<int, Vector3> wayPoints = new Dictionary<int, Vector3>();



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Get relevant data for variables
        baseColor = GameManager.GetColor(gameObject);
        baseDiameter = GetDiameter(this.gameObject);
        spawnlingDiameter = GetDiameter(spawnling);
        timer = spawnTime;

        //Angle Base towards center of arena
        arenaCenterPos = GameObject.Find("ArenaCenter").transform.position - transform.position;
        float angle = Mathf.Atan2(arenaCenterPos.y, arenaCenterPos.x) * Mathf.Rad2Deg - 90;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        Debug.Log(GameManager.GetColor(gameObject) + " Base, Reporting for Duty!");
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0 && reserves > 0)
        {
            Spawn();
            timer = spawnTime;
        }
    }

    private float GetDiameter(GameObject item)
    {
        return item.GetComponent<CircleCollider2D>().radius;
    }

    private void Spawn()
    {
        GameObject spawn = Instantiate(spawnling, transform.position + transform.up, transform.rotation);
        string color = GameManager.GetColor(gameObject);
        if (color == "Green")
            spawn.GetComponent<SpriteRenderer>().color = Color.green;
        else if (color == "Yellow")
            spawn.GetComponent<SpriteRenderer>().color = Color.yellow;
        else if (color == "Red")
            spawn.GetComponent<SpriteRenderer>().color = Color.red;
        else if (color == "Blue")
            spawn.GetComponent<SpriteRenderer>().color = Color.blue;

        reserves--;
    }





}
