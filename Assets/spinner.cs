using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spinner : MonoBehaviour
{
    // Start is called before the first frame update
    public float speed = 1;
    public Vector3 rotation;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(rotation*speed);
    }
}
