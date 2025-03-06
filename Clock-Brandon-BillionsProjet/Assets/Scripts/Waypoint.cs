using UnityEngine;

public class Waypoint : MonoBehaviour
{

    private GameObject packLeader ;
    BillionsBehavior billion;

    // Update is called once per frame
    void Update()
    {
        
        if (packLeader != null)
            billion = GetComponent<BillionsBehavior>();
        

        if (packLeader != null && ((Vector3.Distance(packLeader.transform.position, transform.position) > billion.billionRadius)))
            packLeader = null;

    }

    public void MakePackLeader(GameObject candidate)
    {
        packLeader = candidate;
    }

    public bool HasPackLeader()
    {
        return packLeader != null;
    }
}
