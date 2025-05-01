using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class BaseBehavior : MonoBehaviour
{
    private string baseColor;

    public float expToLevel;
    public int rank;

    private Vector3 arenaCenterPos;


    private float radius;
    private float timer;
    private float fTimer;
    public float curHealth;


    private GameObject hardpoint;
    private Image expBar;
    private TextMeshProUGUI rankText;
    public static UnityEvent<float> awardExp;
    private UnityEvent rankUp;
    private GameObject expGameObject;


    [SerializeField] int reserves;
    [SerializeField] GameObject spawnling;
    [SerializeField] float spawnTime, fireRate;
    [SerializeField] float range;
    [SerializeField] GameObject projectile;
    [SerializeField] public float MAXHEALTH;
    [SerializeField] public float currentExp;
    [SerializeField] public float expValue;


    int cycles;

    void Awake()
    {

        cycles = 0;
        //
        if (name.Contains("(Clone)"))
        {
            int cloneLen = "(Clone)".Length;
            name = name.Remove(name.Length - cloneLen, "(Clone)".Length);
        }
        if (GameObject.Find("MapGenerator"))
            gameObject.SetActive(false);
        //
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(Loading());
        Debug.Log($"Done in {cycles} cycles");

        //Get relevant data for variables
        baseColor = GameManager.GetColor(gameObject);
        timer = spawnTime;
        fTimer = fireRate;

        //Angle Base towards center of arena
        try
        {
            arenaCenterPos = GameManager.arenaCenter.transform.position - transform.position;
        }
        catch
        {
            arenaCenterPos = GameObject.Find("Arena Center").transform.position - transform.position;
        }


        float angle = Mathf.Atan2(arenaCenterPos.y, arenaCenterPos.x) * Mathf.Rad2Deg - 90;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        radius = GetRadius(gameObject);

        //Get hardpoint reference
        hardpoint = transform.GetChild(0).gameObject;
        curHealth = MAXHEALTH;

        if (awardExp == null)
            awardExp = new UnityEvent<float>();

        if (rankUp == null)
            rankUp = new UnityEvent();

        awardExp.AddListener(AwardExp);
        rankUp.AddListener(RankUp);
        expToLevel = 100;

        rank = 1;

        expGameObject = transform.GetChild(2).gameObject;
        expBar = transform.GetChild(2).GetChild(1).GetComponent<Image>();
        rankText = transform.GetChild(2).GetChild(2).GetComponent<TextMeshProUGUI>();

    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;
        fTimer -= Time.deltaTime;

        if (currentExp > 0 && currentExp / expToLevel >= 1)
        {
            rankUp.Invoke();
        }


        if (timer <= 0 && reserves > 0)
        {
            Spawn();
            timer = spawnTime;
        }

        ProxyManager.TargetClosestEnemy(baseColor, range, fTimer, hardpoint, projectile, gameObject);

        
        rankText.text = rank.ToString();
        expBar.fillAmount = currentExp / expToLevel;
        expGameObject.transform.rotation = Quaternion.identity;
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

    public string GetColor()
    {
        return baseColor;
    }

    public void TakeDamage(float damage)
    {
        curHealth -= damage;
        if (curHealth <= 0)
        {
            ProxyManager.worldSpawnlings[baseColor].Remove(gameObject);
            ProxyManager.relevantObjects[baseColor].Remove(gameObject);
            ProxyManager.startingBases.Remove(this);
            Destroy(gameObject);
        }
    }

    private void AwardExp(float expValue)
    {
        currentExp += expValue;
    }

    private void RankUp()
    {
        rank += 1;
        IncreaseExpToLevel();
    }

    private void IncreaseExpToLevel()
    {
        expToLevel *= 2;
    }
    public static int GetRank(string color)
    {
        foreach (GameObject o in ProxyManager.relevantObjects[color])
        {
            if (o.GetComponent<BaseBehavior>() != null)
            {
                return o.GetComponent<BaseBehavior>().rank;
            }
        }
        return 0;
    }

    IEnumerator Loading()
    {
        Debug.Log(ArenaGenerationBehavior.isLoaded);
        yield return new WaitUntil(() => ArenaGenerationBehavior.isLoaded);
    }
}
