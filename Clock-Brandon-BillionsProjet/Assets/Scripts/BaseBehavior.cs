using UnityEngine;

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


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Get relevant data for variables
        baseColor = GetBaseColor(this.gameObject.name);
        baseDiameter = GetDiameter(this.gameObject);
        spawnlingDiameter = GetDiameter(spawnling);
        timer = spawnTime;

        //Angle Base towards center of arena
        arenaCenterPos = GameObject.Find("ArenaCenter").transform.position - transform.position;
        float angle = Mathf.Atan2(arenaCenterPos.y, arenaCenterPos.x) * Mathf.Rad2Deg - 90;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));


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

    private string GetBaseColor(string baseName)
    {
        if (baseName == "GreenBase")
            return "Green";
        else
            return "Yellow";
    }

    private float GetDiameter(GameObject item)
    {
        return item.GetComponent<CircleCollider2D>().radius;
    }

    private void Spawn()
    {
        GameObject spawn = Instantiate(spawnling, transform.position + transform.up, transform.rotation);
        string color = GetBaseColor(gameObject.name);
        if (color == "Green")
            spawn.GetComponent<SpriteRenderer>().color = Color.green;
        else if (color == "Yellow")
            spawn.GetComponent<SpriteRenderer>().color = Color.yellow;
        reserves--;
    }





}
