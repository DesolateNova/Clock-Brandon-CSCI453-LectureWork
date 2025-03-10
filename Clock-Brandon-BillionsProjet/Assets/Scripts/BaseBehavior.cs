using UnityEngine;
using System.Collections.Generic;

public class BaseBehavior : MonoBehaviour
{
    private string baseColor;
    private Vector3 arenaCenterPos;
    private float radius;
    private float timer;

    [SerializeField] private int reserves;
    [SerializeField] private GameObject spawnling;
    [SerializeField] private float spawnTime;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Get relevant data for variables
        baseColor = GameManager.GetColor(gameObject);
        timer = spawnTime;

        //Angle Base towards center of arena
        arenaCenterPos = GameObject.Find("ArenaCenter").transform.position - transform.position;
        float angle = Mathf.Atan2(arenaCenterPos.y, arenaCenterPos.x) * Mathf.Rad2Deg - 90;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        radius = GetRadius(gameObject);
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

    private float GetRadius(GameObject item)
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

        if (!ProxyManager.worldSpawnlings.ContainsKey(color))
            ProxyManager.worldSpawnlings[color] = new LinkedList<GameObject>();
        ProxyManager.AddToSide(color, spawn);
        reserves--;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Spawnling"))
            return;

        float otherRadius = other.GetComponent<CircleCollider2D>().radius;
        Vector3 direction = (other.transform.position - this.transform.position).normalized;
        other.transform.position = transform.position + (direction * radius) + (direction * otherRadius);
    }
}
