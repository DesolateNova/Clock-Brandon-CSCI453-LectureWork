using UnityEngine;

public class BillionsBehavior : MonoBehaviour
{


    
    private float billionRadius;
    private Vector3 myPos;
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
        myPos = gameObject.transform.position;
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
                //Get other billions location if it is greater than the diameter of this billion then it is not close enough to consider
                Vector3 otherPos = billion.transform.position;
                float distance = Vector3.Distance((this.transform.position), billion.transform.position);
                if (billion.GetInstanceID().Equals(this.GetInstanceID()) || distance > billionRadius * 2f)
                    continue;

                //Get the radius of the other billion for use in operations
                CircleCollider2D other = billion.GetComponent<CircleCollider2D>();
    
                //Fail-safe
                if (other == null)
                    return;

                //Get relevant information needed for any operation:
                //size of the other billions radius
                float otherRadius = other.radius;


                //Direction to and from other
                Vector3 directionTo = (billion.transform.position - this.transform.position).normalized;
                Vector3 directionToFrom = -directionTo;

                Vector3 radiusPoint = transform.position + (directionTo * billionRadius);
                Vector3 otherPointOnRadius = billion.transform.position + (-directionTo * otherRadius);

                //Get the ratio of overlap. ditance that other point is from center point of this billion divided by the radius
                float ourOverlap = Vector3.Distance(radiusPoint, otherPointOnRadius);

                
                //Generate x and y offset for dynamic push directionTo
                float xOffset = Random.Range(-1f, 1f);
                float yOffset = Random.Range(-1f, 1f);


                //If the billions are on top of eachother, push them awayfrom eachother at an equal rate toward a the arena center
                if (distance == 0)
                {
                    Vector3 arenaCenter = GameObject.Find("ArenaCenter").transform.position;
                    if (arenaCenter != null)
                    {
                        directionTo = (arenaCenter - this.transform.position).normalized;
                        billion.transform.position += directionTo * billionRadius;
                        this.transform.position -= directionTo * billionRadius;
                    }
                    return;
                }



            }
        }
    }
}
