using UnityEngine;

public class ProjectileBehavior : MonoBehaviour
{

    [SerializeField] int damage;
    [SerializeField] int moveSpeed;
    [SerializeField] float maxProjectileDist;
    Vector3 movementVector;
    private float y, x;
    private string color;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        float angle = transform.rotation.eulerAngles.z + 90f;
        y = Mathf.Abs(Mathf.Tan(angle * Mathf.Deg2Rad)) % 360f;

        if (angle > 180 && angle < 360)
            y =  -y;
    
        if (angle > 90 && angle < 270)
            x = -1;
        else if (angle == 90 || angle == 270)
            x = 0;
        else
            x = 1;

        movementVector = new Vector3(x, y, 0).normalized;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += movementVector * moveSpeed * Time.deltaTime;
        maxProjectileDist -= moveSpeed * Time.deltaTime;
        
        if (maxProjectileDist < 0)
            Destroy(gameObject);

    }

    private void OnTriggerEnter2D(Collider2D other)
    {

        if (other.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }

        if (other.gameObject.CompareTag("Spawnling"))
        {
            BillionsBehavior otherBillion = other.GetComponent<BillionsBehavior>();
            if (otherBillion.GetColor() != this.color)
            {
                otherBillion.TakeDamage(damage);
                Destroy(gameObject);
            } 
        }
    }

    public void SetColor(string color)
    {
        this.color = color;
    }
}
