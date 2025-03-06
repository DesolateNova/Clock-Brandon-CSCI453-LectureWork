using UnityEngine;

public class BillionsBehavior : MonoBehaviour
{

    [SerializeField] float movementSpeed;
    [SerializeField] float timeToMaxVelocity;
    private static int billionNumber;


    public float billionRadius;
    private Transform myPos;
    private string color;

    private float velocityGain;
    private float currentVelocity;
    private float initialVelocity = 0.25f;

    private bool atWaypoint;
    private bool firstMove;
    private Waypoint waypoint;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        billionRadius = gameObject.GetComponent<CircleCollider2D>().radius;
        color = GameManager.GetColor(gameObject);
        name = "Team " + color + " Billion Unit " + billionNumber;
        billionNumber++;
        myPos = transform;
        atWaypoint = false;
        firstMove = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (ProxyManager.worldSpawnlings[color] != null && ProxyManager.worldSpawnlings[color].Count > 1)
            positionAdjuster();

        GameObject nearestWaypoint = ProxyManager.GetNearestRelevantObject(gameObject, color);
        if (nearestWaypoint != null)
            waypoint = nearestWaypoint.GetComponent<Waypoint>();

        if (waypoint != null)
        {
            if (waypoint.HasPackLeader() == false && atWaypoint == true)
                atWaypoint = false;

            if (atWaypoint == false)
                MoveTowards(nearestWaypoint);
        }

    }

    private void positionAdjuster()
    {
        
        foreach (string color in ProxyManager.worldSpawnlings.Keys)
        {
            foreach (GameObject billion in ProxyManager.worldSpawnlings[color])
            {   
                //Get other billions location if it is greater than the diameter of this billion then it is not close enough to consider
                Transform otherPos = billion.transform;
                float distanceBetweenCenters = Vector3.Distance((myPos.position), billion.transform.position);
                if (gameObject == billion || distanceBetweenCenters > (billionRadius * 2))
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
                Vector3 directionTo = (billion.transform.position - myPos.position).normalized;
                Vector3 directionFrom = -directionTo;

                Vector3 pointOnRadius = myPos.position + (directionTo * billionRadius);


                Vector3 otherPointOnRadius = billion.transform.position + (-directionTo * otherRadius);

                //Get the ratio of overlap. distanceBetweenCenters other point is from center point of this billion divided by the radius.
                //Dist(otherPontOnRadius, myPos) / billionRadius
                //this is so we can push the billion away from the overlapping object equal to the level of overlap the two objects have
                float distanceFromOtherPoint = Vector3.Distance(otherPointOnRadius, myPos.position);
                float distanceFromEdge = Vector3.Distance(pointOnRadius, otherPointOnRadius);
                float overlap = (distanceFromEdge / billionRadius);

                
                //Generate x and y offset for dynamic push directionTo
                float xOffset = Random.Range(-0.25f, 0.25f);
                float yOffset = Random.Range(-0.25f, 0.25f);


                //If the billions are on top of eachother, push them awayfrom eachother at an equal rate toward a the arena center
                if (distanceBetweenCenters == 0f)
                {
                    Vector3 arenaCenter = GameObject.Find("ArenaCenter").transform.position;
                    if (arenaCenter != null)
                    {
                        directionTo = (arenaCenter - myPos.position).normalized;
                        billion.transform.position += directionTo * (billionRadius + 0.0001f);
                        myPos.position -= directionTo * (billionRadius + 0.0001f);
                        return;
                    }
                }
                //If other billions center point is within the radius of this billion, push billion in a direction equal to that distance
                //plus the radius
                else if (distanceBetweenCenters < billionRadius && overlap > 1)
                {
                    float distanceToMove = Vector3.Distance(otherPos.position, pointOnRadius);
                    Vector3 direction = (new Vector3(xOffset, yOffset, 0f) + directionTo).normalized;
                    billion.transform.position = myPos.position + (direction * billionRadius) + (direction * (otherRadius + 0.0001f));
                    return;
                }
                else if (distanceBetweenCenters > billionRadius && overlap < 1)
                {
                    billion.transform.position = myPos.position + (directionTo * billionRadius) + (directionTo * (otherRadius + 0.0001f));
                }
            }
        }
    }

    private void MoveTowards(GameObject location)
    {
        float angle = Mathf.Atan2(location.transform.position.x, location.transform.position.y) * Mathf.Rad2Deg - 90;
        Vector3 direction = location.transform.position - transform.position;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

        if (firstMove == false && atWaypoint == false)
            velocityGain += Time.deltaTime;
        else
            velocityGain = 0f;

        if (velocityGain / timeToMaxVelocity > 1f)
            currentVelocity = 1f;
        else if (velocityGain / timeToMaxVelocity > initialVelocity)
            currentVelocity = velocityGain / timeToMaxVelocity;
        else
            currentVelocity = initialVelocity;
            
        if (Vector3.Distance(location.transform.position, myPos.position) > billionRadius)
            transform.position += direction.normalized * (Time.deltaTime * movementSpeed * currentVelocity);
        else if (Vector3.Distance(location.transform.position, myPos.position) < billionRadius)
        {
            waypoint.MakePackLeader(gameObject);
            atWaypoint = true;
        }

        firstMove = false;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (waypoint == null)
            return;

        if (other.CompareTag("Spawnling"))
        {
            BillionsBehavior otherBillion = other.GetComponent<BillionsBehavior>();
            atWaypoint = otherBillion.atWaypoint;
        }

    }
}
