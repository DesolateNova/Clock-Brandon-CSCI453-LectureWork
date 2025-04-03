using UnityEngine;
using System.Collections.Generic;

public class BaseBehavior : MonoBehaviour
{
    private string baseColor;
    private Vector3 arenaCenterPos;
    private float radius;
    private float timer;
    private float fTimer;
    private GameObject hardpoint;

    [SerializeField] private int reserves;
    [SerializeField] private GameObject spawnling;
    [SerializeField] private float spawnTime, fireRate;
    [SerializeField] private float range;
    [SerializeField] private GameObject projectile;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Get relevant data for variables
        baseColor = GameManager.GetColor(gameObject);
        timer = spawnTime;
        fTimer = fireRate;

        //Angle Base towards center of arena
        arenaCenterPos = GameManager.arenaCenter.transform.position - transform.position;
        float angle = Mathf.Atan2(arenaCenterPos.y, arenaCenterPos.x) * Mathf.Rad2Deg - 90;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        radius = GetRadius(gameObject);

        //Get hardpoint reference
        hardpoint = transform.GetChild(0).gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;
        fTimer -= Time.deltaTime;
        if (timer <= 0 && reserves > 0)
        {
            Spawn();
            timer = spawnTime;
        }

        ProxyManager.TargetClosestEnemy(baseColor, range, fTimer, hardpoint, projectile, gameObject);
    }

    private float GetRadius(GameObject item)
    {
        return item.GetComponent<CircleCollider2D>().radius;
    }

    private void Spawn()
    {
        GameObject spawn = Instantiate(spawnling, transform.position + transform.up, transform.rotation);
        string color = GameManager.GetColor(gameObject);
        SpriteRenderer spawnColorSetter = spawn.transform.GetChild(0).GetComponent<SpriteRenderer>();
        if (color == "Green")
            spawnColorSetter.color = Color.green;
        else if (color == "Yellow")
            spawnColorSetter.color = Color.yellow;
        else if (color == "Red")
            spawnColorSetter.color = Color.red;
        else if (color == "Blue")
            spawnColorSetter.color = Color.blue;

            if (!ProxyManager.worldSpawnlings.ContainsKey(color))
                 ProxyManager.worldSpawnlings[color] = new LinkedList<GameObject>();
        ProxyManager.AddToSide(color, spawn);
        reserves--;
    }

    public void Reload()
    {
        fTimer = fireRate;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Spawnling"))
            return;

        float otherRadius = other.GetComponent<CircleCollider2D>().radius;
        Vector3 direction = GameManager.GetDirectionTowards(other.gameObject, gameObject);
        other.transform.position = transform.position + (direction * radius) + (direction * otherRadius);
    }
}
