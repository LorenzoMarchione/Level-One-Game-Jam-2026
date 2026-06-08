using UnityEngine;

public class PigMovement : MonoBehaviour
{
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.position += new Vector3(0.1f, 0);
    }
}
