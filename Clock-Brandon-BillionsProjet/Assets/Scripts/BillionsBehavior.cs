using UnityEngine;

public class BillionsBehavior : MonoBehaviour
{

    private float colliderRadius;
    private Collider2D[] result;
    private Color color;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        colliderRadius = gameObject.GetComponent<CircleCollider2D>().radius;
        color = GetComponent<SpriteRenderer>().color;
        Debug.Log(GameManager.GetColor(color));
    }

    // Update is called once per frame
    void Update()
    {
        Collider2D collision = Physics2D.OverlapCircle(transform.position, colliderRadius);
        if (collision != null && collision.gameObject != gameObject) 
        {
            transform.position += new Vector3(Random.Range(-colliderRadius, colliderRadius), Random.Range(-colliderRadius, colliderRadius), 0f);
        }
    }

}
