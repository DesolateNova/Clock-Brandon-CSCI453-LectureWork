using UnityEngine;

public class BillionsBehavior : MonoBehaviour
{


    
    private float billionRadius;
    //private Collider2D[] result;
    private string color;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        billionRadius = gameObject.GetComponent<CircleCollider2D>().radius;
        color = GameManager.GetColor(gameObject);
        //Debug.Log(GameManager.GetColor(gameObject));
    }

    // Update is called once per frame
    void Update()
    {
        /**Collider2D collision = Physics2D.OverlapCircle(transform.position, billionRadius);
        if (collision != null && collision.gameObject != gameObject) 
        {
            transform.position += new Vector3(Random.Range(-billionRadius, billionRadius), Random.Range(-billionRadius, billionRadius), 0f);
        }*/
        if (ProxyManager.worldSpawnlings[color] != null && ProxyManager.worldSpawnlings[color].Count > 1)
            positionAdjuster();

        GameObject nearestWaypoint = ProxyManager.GetNearestRelevantObject(color);
        //MoveTowards(nearestWaypoint);

    }

    private void positionAdjuster()
    {
        
        foreach (string color in ProxyManager.worldSpawnlings.Keys)
        {
            foreach (GameObject billion in ProxyManager.worldSpawnlings[color])
            {
                if (billion.GetInstanceID().Equals(this.GetInstanceID()))
                    continue;

                CircleCollider2D other = billion.GetComponent<CircleCollider2D>();

                if (other == null)
                    return;

                float distance = Vector3.Distance((this.transform.position), billion.transform.position);
                float otherRadius = other.radius;
                float xOffset = Random.Range(-1f, 1f);
                float yOffset = Random.Range(-1f, 1f);
                

                Vector3 direction = (billion.transform.position - this.transform.position).normalized;
                Vector3 otherDireciton = -direction;
                Vector3 radiusPoint = transform.position + (direction * billionRadius);
                Vector3 otherPointOnRadius = billion.transform.position + (-direction * otherRadius);
                float ourOverlap = Vector3.Distance(radiusPoint, otherPointOnRadius);

                if (distance == 0)
                {
                    Vector3 arenaCenter = GameObject.Find("ArenaCenter").transform.position;
                    if (arenaCenter != null)
                    {
                        direction = (arenaCenter - this.transform.position).normalized;
                        billion.transform.position += direction * billionRadius;
                        this.transform.position -= direction * billionRadius;
                    }
                    return;
                }

                if (distance <= billionRadius)
                {
                    direction = new Vector3(xOffset, yOffset, 0f);
                    PushOther(billion, direction, 0.01f, distance, 0);
                    return;
                }
                else if (Vector3.Distance(otherPointOnRadius, transform.position) <= billionRadius)
                {
                    direction = new Vector3(xOffset, yOffset, 0f);
                    PushOther(billion, direction, 0.01f, distance, 0);
                    return;
                }
            }
        }
    }

    private void PushOther(GameObject other, Vector3 direction, float force, float distance, int numberOfTimes)
    {
        if (distance <= 0f)
        {
            other.transform.position += (direction * force) * numberOfTimes;
            Debug.Log("Number Of Times: " + numberOfTimes);
            return;
        }
        numberOfTimes++;
        PushOther(other, direction, force, distance - force, numberOfTimes);
    }
}
