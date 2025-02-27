using UnityEngine;

public class ProxyChecker
{

    bool isRoot;
    GameObject item;
    Vector3 relDist;

    ProxyChecker parent;
    ProxyChecker left;
    ProxyChecker right;

    public ProxyChecker()
    {
        if (parent == null)
            isRoot = true;
    }

    public void Track(GameObject item)
    {
        if (this.item == null)
        {
            this.item = item;
            relDist = item.transform.position -= MovementBehavior.mousePosition;
            return;
        }
        else if (left == null)
        {
            left = new ProxyChecker();
            left.parent = this;
            left.Track(item);
        }
        else if (right == null)
        {
            right = new ProxyChecker();
            right.parent = this;
            right.Track(item);
        }

        RuleCheck();

    }

    public GameObject GetClosest()
    {
        if (isRoot)
            return item;
        else return null;
    }

    public Vector3 GetRelDistance()
    {
        return relDist;
    }

    private void RuleCheck()
    {
        if (right != null && (Vector3.Distance(right.relDist, MovementBehavior.mousePosition) < Vector3.Distance(this.relDist, MovementBehavior.mousePosition)))
        {
            Swap(right);
        }
        else if (left != null && (Vector3.Distance(left.relDist, MovementBehavior.mousePosition) < Vector3.Distance(this.relDist, MovementBehavior.mousePosition)))
        {
            Swap(left);
        }
    }

    private void Swap(ProxyChecker child)
    {
        if (this.parent == null)
        {
            isRoot = false;
            child.isRoot = true;
        }
        ProxyChecker temp = child;
        child.parent = this.parent;
        this.parent = temp;
    }


    
}
