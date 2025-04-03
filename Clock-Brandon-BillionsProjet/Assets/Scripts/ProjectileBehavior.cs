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
        //Set the angle of the projectile and create x and y values to act as the slope
        float angle = transform.rotation.eulerAngles.z + 90f;
        y = Mathf.Abs(Mathf.Tan(angle * Mathf.Deg2Rad)) % 360f;

        
        //If the slope is calculated to be within Q3 or Q4 invert the y direction
        if (angle > 180 && angle < 360)
            y =  -y;
    
        //If the slope is calculated to be in Q2 or Q3 set the x direction to be left
        if (angle > 90 && angle < 270)
            x = -1;
        //else if the slope is calculated to be exactly up or down set the x direction to 0
        else if (angle == 90 || angle == 270)
            x = 0;
        //Anthing else set the x direction to right
        else
            x = 1;

        //Create movement vector using the calculated values
        movementVector = new Vector3(x, y, 0).normalized;
    }

    // Update is called once per frame
    void Update()
    {
        //Move the projectile along its calculated axis subtracting the distance from the max alloted ditance of the projectile
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

        //If the projectile hits another spawnling of a different team, damage them and destroy this projectile
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

    //Method to declare the color of this projectiles team. Inherited from parent billion.
    public void SetColor(string color)
    {
        this.color = color;
    }
}
