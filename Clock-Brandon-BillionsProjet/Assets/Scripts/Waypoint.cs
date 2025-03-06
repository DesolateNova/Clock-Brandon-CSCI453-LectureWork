using UnityEngine;

public class Waypoint : MonoBehaviour
{

    private GameObject packLeader;
    private BillionsBehavior billion;

    // Update is called once per frame
    void Update()
    {
        if (packLeader != null)
            billion = packLeader.GetComponent<BillionsBehavior>();

        if (packLeader != null && billion != null)
        {
            Debug.Log(packLeader.name + " is packleader of waypoint at " + gameObject.transform.position);
            if (Vector3.Distance(packLeader.transform.position, transform.position) > billion.billionRadius)
                packLeader = null;
        }
    }

    public void MakePackLeader(GameObject candidate)
    {
        Debug.Log(gameObject.name + " Making " + candidate.name + " Pack Leader");
        packLeader = candidate;
    }

    public bool HasPackLeader()
    {
        return packLeader != null;
    }
}
