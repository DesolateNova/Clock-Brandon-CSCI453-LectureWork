using UnityEngine;

public class ProjectileBehavior : MonoBehaviour
{

    [SerializeField] int damage;
    [SerializeField] int moveSpeed;
    Vector3 movementVector;
    private float y, x;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        y = Mathf.Abs(Mathf.Tan((transform.rotation.eulerAngles.z -90f))) * Mathf.Rad2Deg % 360f;
        if (y == 90 || y == 270)
            x = 0;
        else if (y > 90 && y < 270)
            x = -1;
        else
            x = 1;

        Debug.Log($"x value: {x}");
        Debug.Log($"y value: {y}");


        movementVector = new Vector3(x, y, 0).normalized;
        Debug.Log($"Transfrom rotation z = {transform.rotation.eulerAngles.z}");
        Debug.Log($"Movement Vector x: {movementVector.x}\nMovement Vector y: {movementVector.y}");
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += movementVector * moveSpeed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {

    }
}
