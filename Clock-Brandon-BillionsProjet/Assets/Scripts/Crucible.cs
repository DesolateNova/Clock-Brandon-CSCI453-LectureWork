using UnityEngine;

public class Crucible : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Vector3 C = new Vector3(0, 0, 0);
        Vector3 T = new Vector3(-14, -11, 0);
        Vector3 B = new Vector3(-7, -10, 0);

        Vector3 CTNorm = (T - C).normalized;
        Vector3 CBNorm = (B - C).normalized;
        Vector3 CTCBNorm = (CTNorm - CBNorm).normalized;
        Debug.Log("*****************************************************************************************************************\n" +
            "*****************************************************************************************************************\n" +
            "*****************************************************************************************************************\n");
        Debug.Log($"CTNorm: {CTNorm}");
        Debug.Log($"CBNorm: {CBNorm}");
        Debug.Log($"CTCBNorm: {CTCBNorm}");
        Debug.Log("*****************************************************************************************************************\n" +
            "*****************************************************************************************************************\n" +
            "*****************************************************************************************************************\n");
        Debug.Break();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
